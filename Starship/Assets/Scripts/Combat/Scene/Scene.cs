using System;
using Combat.Component.Body;
using Combat.Component.Ship;
using Combat.Component.Unit;
using Combat.Component.Unit.Classification;
using Combat.Unit;
using GameServices.Settings;
using ModestTree;
using UnityEngine;
using Zenject;

namespace Combat.Scene
{
    public class Scene : IScene, ITickable, IFixedTickable, IDisposable
    {
        [Inject] private readonly ShipCreatedSignal.Trigger _shipCreatedTrigger;
        [Inject] private readonly ShipDestroyedSignal.Trigger _shipDestroyedTrigger;

        [Inject]
        public Scene(GameSettings gameSettings, SceneSettings settings, IViewRect viewRect)
        {
            _viewRect = viewRect;
            Assert.That(settings.AreaWidth >= _viewRect.MaxWidth && settings.AreaHeight >= _viewRect.MaxHeight);

            if (gameSettings.CenterOnPlayer)
                settings.PlayerAlwaysInCenter = true;

            _settings = settings;

            _unitList.UnitAdded += OnUnitAdded;
            _unitList.UnitRemoved += OnUnitRemoved;
        }

        public SceneSettings Settings => _settings;

        public void AddUnit(IUnit unit)
        {
            _unitList.Add(unit);
        }

        public IUnitList<IShip> Ships => _shipList;
        public IUnitList<IUnit> Units => _unitList;

        public Vector2 ViewPoint => _viewRect.Center;

        public Rect ViewRect
        {
            get
            {
                var offset = _disturbance * RotationHelpers.Direction(_random.Next(360));
                var rect = _viewRect.Rect;
                rect.x += offset.x;
                rect.y += offset.y;
                return rect;
            }
        }

        public IShip PlayerShip => _activePlayerShip;
        public IShip EnemyShip => _nearestEnemyShip;
        
        public void Tick()
        {
            _unitList.UpdateCollection();
            _unitList.UpdateItems(UpdateUnitView);

            _viewRect.Update(_activePlayerShip, _nearestEnemyShip, _playerInCenter);
            _disturbance *= 1 - 2 * Time.unscaledDeltaTime;
        }

        public void FixedTick()
        {
            _unitList.UpdateItems(UpdateUnitPhysics);
            UpdateEnemies();
            CheckBounds();

            _lastUpdateTime = Time.fixedTime;
        }

        private Vector2 RandomPoint(Vector2 center)
        {
            return new Vector2(center.x + _random.Next(-_settings.AreaWidth / 2, _settings.AreaWidth / 2),
                center.y + _random.Next(-_settings.AreaHeight / 2, _settings.AreaHeight / 2));
        }

        public Vector2 FindFreePlace(float minDistance, UnitSide unitSide)
        {
            var squaredDistance = minDistance * minDistance;
            var center = _activePlayerShip?.Body.WorldPosition() ?? Vector2.zero;
            var maxRadius =
                Math.Sqrt(_settings.AreaWidth * _settings.AreaWidth + _settings.AreaHeight * _settings.AreaHeight) / 2;
            // If required free area is larger than a field, just return a random point on a map 
            if (minDistance >= maxRadius)
                return RandomPoint(center);

            for (var i = 0; i < 100; ++i)
            {
                var position = RandomPoint(center);

                var isFree = true;

                lock (_shipList.LockObject)
                {
                    foreach (var ship in _shipList.Items)
                    {
                        if (ship.Type.Side.IsAlly(unitSide))
                            continue;
                        if (ship.Body.WorldPosition().SqrDistance(position) >= squaredDistance)
                            continue;

                        isFree = false;
                        break;
                    }
                }

                if (isFree)
                    return position;
            }

            // Free space was not found, so spawn ship away from player/center
            var targetPos = center;
            // In case of very large radius, aka so we can't find a suitable
            // center, we just pick the farthest point we've got so far
            var maxSquaredDist = 0f;
            var tries = 1000;
            do
            {
                var newPos = RandomPoint(center);
                var sqrDist = center.SqrDistance(newPos);
                if (sqrDist < maxSquaredDist) continue;
                targetPos = newPos;
                maxSquaredDist = sqrDist;
            } while (maxSquaredDist < squaredDistance && tries-- > 0);
            return targetPos;
        }

        public void Shake(float amplitude)
        {
            _disturbance = Mathf.Max(_disturbance, amplitude);
        }

        public void Dispose()
        {
            _unitList.Clear();
            _shipList.Clear();
        }

        private bool UpdateUnitView(IUnit unit)
        {
            unit.UpdateView(Time.deltaTime);
            return unit.IsActive();
        }

        private bool UpdateUnitPhysics(IUnit unit)
        {
            var deltaTime = _lastUpdateTime > 0 ? Time.fixedTime - _lastUpdateTime : Time.fixedDeltaTime;
            unit.UpdatePhysics(deltaTime);
            return unit.IsActive();
        }

        private void OnUnitAdded(IUnit unit)
        {
            if (!(unit is IShip ship))
                return;

            _shipList.Add(ship);

            if (ship.Type.Class == UnitClass.Ship)
            {
                switch (ship.Type.Side)
                {
                    case UnitSide.Player:
                        if (!_activePlayerShip.IsActive())
                            _activePlayerShip = ship;
                        _viewRect.Zoom();
                        break;
                    case UnitSide.Enemy:
                        if (!_nearestEnemyShip.IsActive())
                            _nearestEnemyShip = ship;
                        break;
                }
            }

            _shipCreatedTrigger.Fire(ship);
        }

        private void OnUnitRemoved(IUnit unit)
        {
            if (!(unit is IShip ship))
                return;

            _shipList.Remove(ship);
            _shipDestroyedTrigger.Fire(ship);
        }

        private void UpdateEnemies()
        {
            if (_activePlayerShip == null)
                return;

            var enemyCount = 0;
            var position = _activePlayerShip.Body.Position;
            var minDistance = float.MaxValue;
            _nearestEnemyShip = null;

            lock (_shipList.LockObject)
            {
                foreach (var ship in _shipList.Items)
                {
                    if (ship.IsActive() && ship.Type.Side == UnitSide.Enemy && ship.Type.Class == UnitClass.Ship)
                    {
                        enemyCount++;
                        var distance = Vector2.SqrMagnitude(ship.Body.Position - position);
                        if (distance < minDistance)
                        {
                            minDistance = distance;
                            _nearestEnemyShip = ship;
                        }
                    }
                }
            }

            _playerInCenter = _settings.PlayerAlwaysInCenter || enemyCount > 1;
        }

        private void CheckBounds()
        {
            if (_activePlayerShip == null)
                return;

            if (_cooldown > 0)
            {
                _cooldown -= Time.deltaTime;
                return;
            }
            _cooldown = 0.5f;

            lock (_unitList.LockObject)
            {
                const float maxDistancePlayerDistance = 1e5f;
                var center = _activePlayerShip.Body.Position;
                var extraOffset = Vector2.zero;
                if (center.sqrMagnitude > maxDistancePlayerDistance * maxDistancePlayerDistance)
                {
                    extraOffset = -center;
                    center = Vector2.zero;
                    _activePlayerShip.Body.ShiftWithDependants(extraOffset);
                    MainCamera.transform.Translate(extraOffset);
                }

                foreach (var unit in _unitList.Items)
                {
                    if (unit == _activePlayerShip)
                        continue;
                    if (unit.Body.Parent != null)
                        continue;
                    if (unit.Type.Class == UnitClass.Limb)
                        continue;

                    var changed = extraOffset != Vector2.zero;

                    var position = unit.Body.Position;
                    var offset = new Vector2();

                    while (position.x + offset.x - center.x > 0.5f * _settings.AreaWidth)
                    {
                        offset.x -= _settings.AreaWidth;
                        changed = true;
                    }
                    while (position.x + offset.x - center.x < -0.5f * _settings.AreaWidth)
                    {
                        offset.x += _settings.AreaWidth;
                        changed = true;
                    }

                    while (position.y + offset.y - center.y > 0.5f * _settings.AreaHeight)
                    {
                        offset.y -= _settings.AreaHeight;
                        changed = true;
                    }
                    while (position.y + offset.y - center.y < -0.5f * _settings.AreaHeight)
                    {
                        offset.y += _settings.AreaHeight;
                        changed = true;
                    }

                    if (changed)
                    {
                        unit.Body.ShiftWithDependants(offset + extraOffset);
                    }
                }
            }
        }

        private float _cooldown;
        private float _disturbance;
        private float _lastUpdateTime;

        private bool _playerInCenter;
        private IShip _activePlayerShip;
        private IShip _nearestEnemyShip;
        private UnityEngine.Camera _mainCamera;
        // ReSharper disable once Unity.NoNullCoalescing
        private UnityEngine.Camera MainCamera => _mainCamera ?? (_mainCamera = UnityEngine.Camera.main);

        private readonly UnitList<IUnit> _unitList = new UnitList<IUnit>();
        private readonly ShipList _shipList = new ShipList();

        private readonly IViewRect _viewRect;
        private readonly SceneSettings _settings;
        private readonly System.Random _random = new System.Random();
    }
}
