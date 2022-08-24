using Combat.Component.Unit;

namespace Combat.Unit.Auxiliary
{
    public interface IAuxiliaryUnit : IUnit
    {
        bool Active { get; set; }
        bool Enabled { get; set; }

        void Destroy();
    }
}
