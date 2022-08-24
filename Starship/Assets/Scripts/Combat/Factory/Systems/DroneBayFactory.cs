using Combat.Component.Platform;
using Combat.Component.Ship;
using Combat.Component.Systems;
using Combat.Component.Systems.DroneBays;
using Combat.Component.Triggers;
using Combat.Scene;
using Constructor;
using GameDatabase;
using Services.Audio;
using Services.ObjectPool;
using UnityEngine;
using Zenject;

namespace Combat.Factory
{
    public class DroneBayFactory
    {
        [Inject] private readonly IScene _scene;
        [Inject] private readonly ISoundPlayer _soundPlayer;
        [Inject] private readonly IObjectPool _objectPool;
        [Inject] private readonly SpaceObjectFactory _spaceObjectFactory;
        [Inject] private readonly IDatabase _database;
        [Inject] private readonly ShipFactory _shipFactory;
        [Inject] private readonly EffectFactory _effectFactory;

        public ISystem Create(IWeaponPlatform platform, IDroneBayData droneBayData, IShip mothership, IDroneReplicator droneReplicator)
        {
            var dronebay = new DroneBay(platform, mothership, droneBayData, _shipFactory, _database.ShipSettings, droneReplicator);
            var stats = droneBayData.DroneBay;

            if (stats.LaunchEffectPrefab)
            {
                var effect = _effectFactory.CreateEffect(stats.LaunchEffectPrefab);
                effect.Color = Color.white;
                effect.Size = 1.0f;
                dronebay.AddTrigger(new FlashEffect(effect, platform.Body, 0.2f, Vector2.zero, ConditionType.OnActivate));
            }

            if (stats.LaunchSound)
                dronebay.AddTrigger(new SoundEffect(_soundPlayer, stats.LaunchSound, ConditionType.OnActivate));

            return dronebay;
        }
    }
}
