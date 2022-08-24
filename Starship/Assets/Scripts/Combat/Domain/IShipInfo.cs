using Combat.Ai;
using Combat.Component.Unit.Classification;
using UnityEngine;

namespace Combat.Domain
{
    public enum ShipStatus
    {
        Ready,
        Active,
        Destroyed,
    }

    public interface IShipInfo
    {
        ShipStatus Status { get; }

        Component.Ship.IShip ShipUnit { get; }
        Constructor.Ships.IShip ShipData { get; }
        float Condition { get; }
        float ActivationTime { get; }
        UnitSide Side { get; }

        void Create(Factory.ShipFactory factory, IControllerFactory controllerFactory, Vector2 position);
        void Destroy();
    }
}
