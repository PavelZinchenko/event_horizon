using Combat.Component.Body;
using Combat.Component.Ship;
using Combat.Component.Unit;
using Combat.Unit;
using UnityEngine;

namespace Combat.Scene
{
    public class ExplorationViewRect : IViewRect
    {
        public ExplorationViewRect()
        {
            _maxSize = new Vector2(MaxScreenHeight * Screen.width / Screen.height, MaxScreenHeight);
            _minSize = _maxSize * MinScale;
            _margins = Margins;
        }

        public float MaxWidth => _maxSize.x;
        public float MaxHeight => _maxSize.y;

        public Rect Rect => new Rect(_topLeft.x, _topLeft.y, _bottomRight.x - _topLeft.x, _bottomRight.y - _topLeft.y);
        public Vector2 Center => new Vector2(0.5f * (_topLeft.x + _bottomRight.x), 0.5f * (_topLeft.y + _bottomRight.y));

        public void Zoom()
        {
            _zoom = 0;
        }

        //public void Update(IUnit camera)
        //{
        //    var center = camera.Body.WorldPosition();
        //    var size = camera.Body.Scale;

        //    UpdateZoom();

        //    var maxSize = _maxSize * _zoom;

        //    var width = Mathf.Clamp(size * 2, _minSize.x, maxSize.x);
        //    var height = Mathf.Clamp(size * 2, _minSize.y, maxSize.y);

        //    LookAt(center, width, height);
        //}

        //public void Reset()
        //{
        //    _lastPlayerPosition = _lastEnemyPosition = Vector2.zero;
        //}

        private void LookAt(Vector2 position, float width, float height)
        {
            _topLeft.x = position.x - width / 2;
            _topLeft.y = position.y - height / 2;
            _bottomRight.x = position.x + width / 2;
            _bottomRight.y = position.y + height / 2;
        }

        public void Update(IShip playerShip, IShip enemyShip, bool playerInTheMiddle)
        {
            UpdateZoom();

            if (playerShip.IsActive())
                _lastPlayerPosition = playerShip.Body.WorldPosition() + _zoom*MaxScreenHeight*0.1f*RotationHelpers.Direction(playerShip.Body.WorldRotation());
            /*if (enemyShip == null)
                _lastEnemyPosition = _lastPlayerPosition;
            else */if (enemyShip.IsActive())
                _lastEnemyPosition = enemyShip.Body.WorldPosition();

            var sizeScale = playerShip.IsActive() ? 1f - Mathf.Pow(0.5f, playerShip.Body.Scale) : 1.0f;
            var maxSize = _maxSize * _zoom * sizeScale;

            //if (_fixedPlayerPosition)
            //{
            //    LookAt(_lastPlayerPosition, maxSize.x, maxSize.y);
            //    return;
            //}

            if (playerInTheMiddle)
            {
                var dir = _lastPlayerPosition.Direction(_lastEnemyPosition);

                _topLeft = _bottomRight = _lastPlayerPosition + dir;
                AddPointToViewRect(_lastPlayerPosition - dir);
            }
            else
            {
                _topLeft = _bottomRight = _lastPlayerPosition;
                AddPointToViewRect(_lastEnemyPosition);
            }

            var width = _bottomRight.x - _topLeft.x;
            var height = _bottomRight.y - _topLeft.y;

            _bottomRight.x += _margins * width;
            _bottomRight.y += _margins * height;
            _topLeft.x -= _margins * width;
            _topLeft.y -= _margins * height;
            width *= 1 + 2 * _margins;
            height *= 1 + 2 * _margins;

            if (width > maxSize.x)
            {
                _topLeft.x = _lastPlayerPosition.x - (_lastPlayerPosition.x - _topLeft.x) * maxSize.x / width;
                _bottomRight.x = _lastPlayerPosition.x + (_bottomRight.x - _lastPlayerPosition.x) * maxSize.x / width;
            }
            else if (width < _minSize.x)
            {
                _bottomRight.x += (_minSize.x - width) / 2;
                _topLeft.x -= (_minSize.x - width) / 2;
            }

            if (height > maxSize.y)
            {
                _topLeft.y = _lastPlayerPosition.y - (_lastPlayerPosition.y - _topLeft.y) * maxSize.y / height;
                _bottomRight.y = _lastPlayerPosition.y + (_bottomRight.y - _lastPlayerPosition.y) * maxSize.y / height;
            }
            else if (height < _minSize.y)
            {
                _bottomRight.y += (_minSize.y - height) / 2;
                _topLeft.y -= (_minSize.y - height) / 2;
            }

            var margins = new Vector2((_bottomRight.x - _topLeft.x) * _margins, (_bottomRight.y - _topLeft.y) * _margins);

            var width1 = _bottomRight.x - _lastPlayerPosition.x;
            if (width1 < margins.x)
            {
                _bottomRight.x += margins.x - width1;
                _topLeft.x += margins.x - width1;
            }

            var width2 = _lastPlayerPosition.x - _topLeft.x;
            if (width2 < margins.x)
            {
                _bottomRight.x -= margins.x - width2;
                _topLeft.x -= margins.x - width2;
            }

            var height1 = _bottomRight.y - _lastPlayerPosition.y;
            if (height1 < margins.y)
            {
                _bottomRight.y += margins.y - height1;
                _topLeft.y += margins.y - height1;
            }

            var height2 = _lastPlayerPosition.y - _topLeft.y;
            if (height2 < margins.y)
            {
                _bottomRight.y -= margins.y - height2;
                _topLeft.y -= margins.y - height2;
            }
        }

        private void UpdateZoom()
        {
            if (_zoom < 1.0f)
                _zoom = Mathf.Clamp01(_zoom + 0.3f * Time.deltaTime);
        }

        //public void Update(IList<IShip> ships)
        //{
        //    if (_zoom < 1.0f)
        //        _zoom = Mathf.Clamp01(_zoom + 0.3f * Time.deltaTime);

        //    var count = ships.Count;
        //    var first = true;
        //    var enemiesCount = 0;

        //    for (var i = 0; i < count; ++i)
        //    {
        //        var ship = ships[i];
        //        if (ship.Type.Class == UnitClass.Drone || ship.Type.Class == UnitClass.Decoy)
        //            continue;

        //        var position = ship.Body.Position;

        //        if (ship.Type.Side == UnitSide.Player)
        //            _lastPlayerPosition = position;
        //        else if (ship.Type.Side == UnitSide.Enemy)
        //        {
        //            _lastEnemyPosition = position;
        //            enemiesCount++;
        //        }

        //        if (first)
        //        {
        //            _topLeft = position;
        //            _bottomRight = position;
        //            first = false;
        //            continue;
        //        }

        //        AddPointToViewRect(position);
        //    }

        //    AddPointToViewRect(_lastEnemyPosition);
        //    AddPointToViewRect(_lastPlayerPosition);

        //    if (enemiesCount > 1)
        //    {
        //        var halfWidth = Mathf.Max(_lastPlayerPosition.x - _topLeft.x, _bottomRight.x - _lastPlayerPosition.x);
        //        var halfHeight = Mathf.Max(_lastPlayerPosition.y - _topLeft.y, _bottomRight.y - _lastPlayerPosition.y);
        //        _topLeft.x = _lastPlayerPosition.x - halfWidth;
        //        _topLeft.y = _lastPlayerPosition.y - halfHeight;
        //        _bottomRight.x = _lastPlayerPosition.x + halfWidth;
        //        _bottomRight.y = _lastPlayerPosition.y + halfHeight;
        //    }

        //    var width = _bottomRight.x - _topLeft.x;
        //    var height = _bottomRight.y - _topLeft.y;

        //    _bottomRight.x += _margins * width;
        //    _bottomRight.y += _margins * height;
        //    _topLeft.x -= _margins * width;
        //    _topLeft.y -= _margins * height;
        //    width *= 1 + 2 * _margins;
        //    height *= 1 + 2 * _margins;

        //    var maxSize = _maxSize * _zoom;

        //    if (width > maxSize.x)
        //    {
        //        _topLeft.x = _lastPlayerPosition.x - (_lastPlayerPosition.x - _topLeft.x) * maxSize.x / width;
        //        _bottomRight.x = _lastPlayerPosition.x + (_bottomRight.x - _lastPlayerPosition.x) * maxSize.x / width;
        //    }
        //    else if (width < _minSize.x)
        //    {
        //        _bottomRight.x += (_minSize.x - width) / 2;
        //        _topLeft.x -= (_minSize.x - width) / 2;
        //    }

        //    if (height > maxSize.y)
        //    {
        //        _topLeft.y = _lastPlayerPosition.y - (_lastPlayerPosition.y - _topLeft.y) * maxSize.y / height;
        //        _bottomRight.y = _lastPlayerPosition.y + (_bottomRight.y - _lastPlayerPosition.y) * maxSize.y / height;
        //    }
        //    else if (height < _minSize.y)
        //    {
        //        _bottomRight.y += (_minSize.y - height) / 2;
        //        _topLeft.y -= (_minSize.y - height) / 2;
        //    }

        //    var margins = new Vector2((_bottomRight.x - _topLeft.x) * _margins, (_bottomRight.y - _topLeft.y) * _margins);

        //    var width1 = _bottomRight.x - _lastPlayerPosition.x;
        //    if (width1 < margins.x)
        //    {
        //        _bottomRight.x += margins.x - width1;
        //        _topLeft.x += margins.x - width1;
        //    }

        //    var width2 = _lastPlayerPosition.x - _topLeft.x;
        //    if (width2 < margins.x)
        //    {
        //        _bottomRight.x -= margins.x - width2;
        //        _topLeft.x -= margins.x - width2;
        //    }

        //    var height1 = _bottomRight.y - _lastPlayerPosition.y;
        //    if (height1 < margins.y)
        //    {
        //        _bottomRight.y += margins.y - height1;
        //        _topLeft.y += margins.y - height1;
        //    }

        //    var height2 = _lastPlayerPosition.y - _topLeft.y;
        //    if (height2 < margins.y)
        //    {
        //        _bottomRight.y -= margins.y - height2;
        //        _topLeft.y -= margins.y - height2;
        //    }
        //}

        private void AddPointToViewRect(Vector2 position)
        {
            if (position.x < _topLeft.x) _topLeft.x = position.x;
            if (position.y < _topLeft.y) _topLeft.y = position.y;
            if (position.x > _bottomRight.x) _bottomRight.x = position.x;
            if (position.y > _bottomRight.y) _bottomRight.y = position.y;
        }

        private Vector2 _topLeft;
        private Vector2 _bottomRight;
        private Vector2 _lastPlayerPosition;
        private Vector2 _lastEnemyPosition;
        //private readonly bool _fixedPlayerPosition;
        //private readonly bool _takePlayerDirection;

        private float _zoom = 1.0f;

        private readonly Vector2 _maxSize;
        private readonly Vector2 _minSize;
        private readonly float _margins;

        private const float MaxScreenHeight = 50f;
        private const float MinScale = 0.3f;
        private const float Margins = 0.2f;
    }
}
