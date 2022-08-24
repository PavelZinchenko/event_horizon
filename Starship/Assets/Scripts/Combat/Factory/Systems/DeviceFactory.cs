using Combat.Component.Ship;
using Combat.Component.Systems;
using Combat.Component.Systems.Devices;
using Combat.Component.Triggers;
using Combat.Effects;
using Combat.Scene;
using Constructor;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using Services.Audio;
using Services.ObjectPool;
using UnityEngine;
using Zenject;

namespace Combat.Factory
{
    public class DeviceFactory
    {
        [Inject] private readonly IScene _scene;
        [Inject] private readonly ISoundPlayer _soundPlayer;
        [Inject] private readonly IObjectPool _objectPool;
        [Inject] private readonly SpaceObjectFactory _spaceObjectFactory;
        [Inject] private readonly SatelliteFactory _satelliteFactory;
        [Inject] private readonly EffectFactory _effectFactory;
        [Inject] private readonly ShipFactory _shipFactory;

        public ISystem Create(IDeviceData deviceData, IShip ship, IShipSpecification shipSpec)
        {
            var stats = deviceData.Device;

            SystemBase device;
            ConditionType soundEffectCondition = ConditionType.OnActivate;

            switch (stats.DeviceClass)
            {
                case DeviceClass.ClonningCenter:
                    device = new ClonningDevice(ship, stats, _shipFactory, shipSpec, _effectFactory, deviceData.KeyBinding);
                    break;
                case DeviceClass.TimeMachine:
                    {
                        device = new InfinityStone(ship, stats, deviceData.KeyBinding);

                        var effect = CreateEffect(stats, ship);
                        if (effect != null)
                            device.AddTrigger(new FlashEffect(effect, ship.Body, 0.2f, Vector2.zero, ConditionType.OnActivate));
                    }
                    break;
                case DeviceClass.Accelerator:
                    {
                        device = new AcceleratorDevice(ship, stats, deviceData.KeyBinding);
                        if (stats.EffectPrefab)
                        {
                            foreach (var engine in shipSpec.Stats.Engines)
                                device.AddTrigger(new FlashEffect(CreateEffect(stats, ship), ship.Body, 0.5f, engine.Position * 0.5f,
                                    ConditionType.OnRemainActive | ConditionType.OnActivate));
                        }
                    }
                    break;
                case DeviceClass.Decoy:
                    device = new DecoyDevice(ship, stats, shipSpec.Stats.ArmorMultiplier.Value * 10, deviceData.KeyBinding, _spaceObjectFactory);
                    break;
                case DeviceClass.Ghost:
                    {
                        device = new GhostDevice(ship, stats, deviceData.KeyBinding);
                        soundEffectCondition = ConditionType.OnActivate | ConditionType.OnDeactivate;
                        var effect = CreateEffect(stats, ship);
                        if (effect != null)
                            device.AddTrigger(new FlashEffect(effect, ship.Body, 0.2f, Vector2.zero, ConditionType.OnDeactivate | ConditionType.OnActivate));
                    }
                    break;
                case DeviceClass.PointDefense:
                    {
                        var pointDefense = new PointDefenseSystem(ship, stats, deviceData.KeyBinding);
                        device = pointDefense;
                        device.AddTrigger(new PointDefenseAction(ship, pointDefense, stats.Size + ship.Body.Scale/2f, 
                            stats.Power*shipSpec.Stats.DamageMultiplier.Value, stats.EnergyConsumption, stats.Cooldown, stats.Color, _satelliteFactory, stats.Sound));
                        soundEffectCondition = ConditionType.None;
                    }
                    break;
                case DeviceClass.GravityGenerator:
                    device = new GravityGenerator(ship, stats, deviceData.KeyBinding);
                    break;
                case DeviceClass.EnergyShield:
                    {
                        var energyShield = _satelliteFactory.CreateEnergyShield(ship, 1f / (stats.Power * shipSpec.Stats.ShieldMultiplier.Value), stats.Size, stats.Color);
                        var energyShieldDevice = new EnergyShieldDevice(ship, stats, deviceData.KeyBinding);
                        device = energyShieldDevice;
                        device.AddTrigger(new AuxiliaryUnitAction(energyShieldDevice, energyShield));
                    }
                    break;
                case DeviceClass.PartialShield:
                    {
                        var energyShield = _satelliteFactory.CreateFrontalShield(ship, 1f / (stats.Power * shipSpec.Stats.ShieldMultiplier.Value), stats.Offset, stats.Size, stats.Color);
                        var energyShieldDevice = new FrontalShieldDevice(ship, stats, deviceData.KeyBinding);
                        device = energyShieldDevice;
                        device.AddTrigger(new AuxiliaryUnitAction(energyShieldDevice, energyShield));
                    }
                    break;
                case DeviceClass.RepairBot:
                    device = new RepairSystem(ship, stats, deviceData.KeyBinding);
                    device.AddTrigger(new RepairBotAction(ship, device, _satelliteFactory, stats.Power * ship.Stats.Armor.MaxValue / 100, stats.Size, stats.Color, stats.Sound));
                    soundEffectCondition = ConditionType.None;
                    break;
                case DeviceClass.Detonator:
                    device = new DetonatorDevice(ship, stats, deviceData.KeyBinding, _spaceObjectFactory, shipSpec.Stats.Layout.CellCount*shipSpec.Stats.DamageMultiplier.Value);
                    break;
                case DeviceClass.Stealth:
                    device = new StealthDevice(ship, stats, deviceData.KeyBinding, false);
                    break;
                case DeviceClass.Teleporter:
                    device = new TeleporterDevice(ship, stats, deviceData.KeyBinding);
                    if (stats.EffectPrefab)
                        device.AddTrigger(new FlashMultipleEffect(stats.EffectPrefab, _effectFactory, ship.Body, 0.5f, stats.Size*ship.Body.Scale, 0f, stats.Color, ConditionType.OnActivate | ConditionType.OnDeactivate));
                    break;
                case DeviceClass.Brake:
                    device = new BrakeDevice(ship, stats, ship.Body.Weight);
                    break;
                case DeviceClass.SuperStealth:
                    device = new StealthDevice(ship, stats, deviceData.KeyBinding, true);
                    break;
                case DeviceClass.Fortification:
                    device = new FortificationDevice(ship, stats, deviceData.KeyBinding);
                    break;
                case DeviceClass.ToxicWaste:
                    device = new ToxicWaste(ship, stats, _spaceObjectFactory, shipSpec.Stats.DamageMultiplier.Value);
                    break;
                case DeviceClass.WormTail:
                    device = new WormTailDevice(stats, _spaceObjectFactory.CreateWormTail(ship, Mathf.FloorToInt(stats.Size), 0.1f,
                        ship.Stats.Armor.MaxValue * stats.Power, stats.ObjectPrefab, stats.Offset.x, stats.Offset.y, 0.15f, shipSpec.Stats.ShipColor));
                    break;
                default:
                    return null;
            }

            if (stats.Sound && soundEffectCondition != ConditionType.None)
                device.AddTrigger(CreateSoundEffect(stats, soundEffectCondition));

            return device;
        }

        private IEffect CreateEffect(DeviceStats stats, IShip ship)
        {
            if (!stats.EffectPrefab)
                return null;

            var effect = _effectFactory.CreateEffect(stats.EffectPrefab);
            effect.Color = stats.Color;
            effect.Size = stats.Size * ship.Body.Scale;

            return effect;
        }

        private SoundEffect CreateSoundEffect(DeviceStats stats, ConditionType condition)
        {
            return stats.Sound ? new SoundEffect(_soundPlayer, stats.Sound, condition) : null;
        }
    }
}
