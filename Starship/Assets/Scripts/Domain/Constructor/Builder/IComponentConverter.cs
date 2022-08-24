using GameDatabase.Enums;
using UnityEngine;

namespace Constructor
{
    public struct ComponentSpec
    {
        public ComponentSpec(ComponentInfo info, int barrelId, int keyBinding, int behaviour)
        {
            Info = info;
            BarrelId = barrelId;
            KeyBinding = keyBinding;
            Behaviour = behaviour;
        }

        public readonly ComponentInfo Info;
        public readonly int BarrelId;
        public readonly int KeyBinding;
        public readonly int Behaviour;
    }

    public interface IComponentConverter
    {
        ComponentSpec Process(IntegratedComponent component, int firstBarrelId);
    }

    public class DefaultComponentConverter : IComponentConverter
    {
        public ComponentSpec Process(IntegratedComponent component, int firstBarrelId)
        {
            return new ComponentSpec(component.Info, component.BarrelId >= 0 ? component.BarrelId + firstBarrelId : -1, component.KeyBinding, component.Behaviour);
        }

        public static readonly DefaultComponentConverter Instance = new DefaultComponentConverter();
    }

    public class EnemyComponentConverter : IComponentConverter
    {
        public EnemyComponentConverter(int level, System.Random random)
        {
            _level = level;
            _random = random;
        }

        public ComponentSpec Process(IntegratedComponent component, int firstBarrelId)
        {
            var barrelId = component.BarrelId >= 0 ? component.BarrelId + firstBarrelId : -1;

            var minQuality = _level < 150 ? 0 : 1;
            var maxQuality = Mathf.Min(_level / 75, 3);
            var quality = _random.SquareRange(minQuality, maxQuality);

            ComponentInfo info;
            if (quality <= 0 || component.Info.ModificationQuality >= ModificationQuality.N1 + quality)
                info = component.Info;
            else if (component.Info.ModificationType != ComponentModType.Empty)
                info = new ComponentInfo(component.Info.Data, component.Info.ModificationType, ModificationQuality.N1 + quality);
            else
                info = ComponentInfo.CreateRandomModification(component.Info.Data, _random, ModificationQuality.N1 + quality, ModificationQuality.N1 + quality);

            return new ComponentSpec(info, barrelId, component.KeyBinding, component.Behaviour);
        }

        private readonly int _level;
        private readonly System.Random _random;
    }
}
