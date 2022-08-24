using GameDatabase.Serializable;

namespace GameDatabase.DataModel
{
    public partial class InstalledComponent
    {
        public InstalledComponent(Constructor.IntegratedComponent component)
        {
            Component = component.Info.Data;
            Modification = component.Info.ModificationType;
            Quality = component.Info.ModificationQuality;
            Locked = true;
            X = component.X;
            Y = component.Y;
            BarrelId = component.BarrelId;
            Behaviour = component.Behaviour;
            KeyBinding = component.KeyBinding;
        }

        public InstalledComponentSerializable Serialize()
        {
            return new InstalledComponentSerializable
            {
                ComponentId = Component.Id.Value,
                Modification = Modification,
                Quality = Quality,
                Locked = Locked,
                X = X,
                Y = Y,
                BarrelId = BarrelId,
                Behaviour = Behaviour,
                KeyBinding = KeyBinding,
            };
        }
    }
}
