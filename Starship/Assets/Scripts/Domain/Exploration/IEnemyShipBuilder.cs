using Combat.Factory;
using UnityEngine;

namespace Game.Exploration
{
    public interface IEnemyShipBuilder
    {
        Combat.Component.Ship.Ship Build(ShipFactory shipFactory, SpaceObjectFactory objectFactory, Vector2 position, float rotation);
    }
}
