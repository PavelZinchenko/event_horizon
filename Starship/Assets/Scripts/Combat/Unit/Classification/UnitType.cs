using System;
using Combat.Component.Collider;
using Combat.Component.Ship;
using Combat.Unit;
using UnityEngine;

namespace Combat.Component.Unit.Classification
{
    public class UnitType
    {
        public UnitType(UnitClass type, UnitSide side, IShip owner)
        {
            _class = type;
            _side = side;
            _owner = owner;
        }

        public UnitClass Class { get { return _class; } }
        public UnitSide Side { get { return _owner != null ? _owner.Type.Side : _side; } }
        public IShip Owner { get { return _owner; } set { _owner = value; } }

        public Layer CollisionLayer
        {
            get
            {
                Layer layer;

                switch (Class)
                {
                    case UnitClass.SpaceObject:
                        return Layer.Default;
                    case UnitClass.Ship:
                    case UnitClass.Shield:
                    case UnitClass.Limb:
                        layer = Layer.Ship1;
                        break;
                    case UnitClass.Missile:
                        layer = Layer.Missile1;
                        break;
                    case UnitClass.EnergyBolt:
                    case UnitClass.AreaOfEffect:
                    case UnitClass.Camera:
                    case UnitClass.BackgroundObject:
                        layer = Layer.Energy1;
                        break;
                    case UnitClass.Decoy:
                    case UnitClass.Drone:
                    case UnitClass.Loot:
                        layer = Layer.Drone1;
                        break;
                    case UnitClass.Platform:
                        layer = Layer.Platform1;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                switch (Side)
                {
                    case UnitSide.Player:
                    case UnitSide.Ally:
                        break;
                    case UnitSide.Enemy:
                        layer += 1;
                        break;
                    case UnitSide.Neutral:
                        layer += 2;
                        break;
                    case UnitSide.Undefined:
                        layer += 3;
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return layer;
            }
        }

        public int CollisionMask { get { return Physics2D.GetLayerCollisionMask((int)CollisionLayer); } }

        private IShip _owner;
        private readonly UnitClass _class;
        private readonly UnitSide _side;

        public static readonly UnitType Default = new UnitType(UnitClass.SpaceObject, UnitSide.Undefined, null);
    }
}
