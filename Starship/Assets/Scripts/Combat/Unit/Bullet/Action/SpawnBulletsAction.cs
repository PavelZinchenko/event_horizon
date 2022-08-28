using Combat.Collision;
using Combat.Component.Body;
using Combat.Component.Platform;
using Combat.Component.Unit;
using Combat.Component.Unit.Classification;
using Combat.Component.View;
using Combat.Factory;
using Combat.Unit.HitPoints;
using GameDatabase.Model;
using Services.Audio;
using UnityEngine;

namespace Combat.Component.Bullet.Action
{
    public class SpawnBulletsAction : IAction, IWeaponPlatform
    {
        public SpawnBulletsAction(IBulletFactory factory, int magazine, float initialOffset, float rotation, float spread, Vector2 InitialPosition, float rechargeTime, IUnit parent, ISoundPlayer soundPlayer, AudioClipId audioClip, ConditionType condition)
        {
            Type = parent.Type;
            _body = new BodyWrapper(parent.Body);
            _factory = factory;
            _magazine = magazine;
            _rechargeTime = rechargeTime > 0 ? rechargeTime : float.MaxValue;
            _offset = initialOffset;
            _rotation = rotation;
            _spread = spread;
            _initialPosition = InitialPosition;
            _soundPlayer = soundPlayer;
            _audioClipId = audioClip;
            Condition = condition;
            EnergyPoints = new UnlimitedEnergy();
        }

        public ConditionType Condition { get; private set; }

        public CollisionEffect Invoke()
        {
            var time = Time.fixedTime;

            if (_lastSpawnTime > 0 && time - _lastSpawnTime < _rechargeTime)
                return CollisionEffect.None;

            if (_magazine <= 1)
                _factory.Create(this, _spread, _rotation, _offset, _initialPosition);
            else
            {
                for (var i = 0; i < _magazine; ++i)
                    _factory.Create(this, _spread, _rotation, _offset, _initialPosition);
                    // _factory.Create(this, 0, Random.Range(0, 360), _offset);
            }

            if (_audioClipId) _soundPlayer.Play(_audioClipId, GetHashCode());

            _lastSpawnTime = time;
            return CollisionEffect.None;
        }

        public void Dispose()
        {
            //_soundPlayer.Stop(GetHashCode());
        }

        public UnitType Type { get; private set; }
        public IBody Body { get { return _body; } }
        public IResourcePoints EnergyPoints { get; private set; }
        public bool IsTemporary { get { return true; } }
        public float FixedRotation { get { return 0; } }
        public bool IsReady { get { return true; } }
        public float Cooldown { get { return 0; } }
        public float AutoAimingAngle { get { return 0; } }
        public void Aim(float bulletVelocity, float weaponRange, bool relative) {}
        public void OnShot() {}
        public void SetView(IView view, Color color) { }

        public void UpdatePhysics(float elapsedTime) {}
        public void UpdateView(float elapsedTime) {}

        private float _lastSpawnTime;
        private readonly AudioClipId _audioClipId;
        private readonly IBulletFactory _factory;
        private readonly float _rechargeTime;
        private readonly float _offset;
        private readonly float _rotation;
        private readonly float _spread;
        private readonly Vector2 _initialPosition;
        private readonly int _magazine;
        private readonly BodyWrapper _body;
        private readonly ISoundPlayer _soundPlayer;
    }
}
