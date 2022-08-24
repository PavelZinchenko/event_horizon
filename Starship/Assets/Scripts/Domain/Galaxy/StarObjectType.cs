using System;
using System.Collections.Generic;
using System.Linq;

namespace Galaxy
{
    public enum StarObjectType
    {
        Undefined   = 0x0000,
		StarBase    = 0x0001,
		Survival    = 0x0002,
		Boss        = 0x0004,
		Wormhole    = 0x0008,
//		Multiplayer = 0x0010,
		Arena       = 0x0020,
		Challenge   = 0x0040,
		BlackMarket = 0x0080,
		Ruins       = 0x0100,
        Military    = 0x0200,
        Event       = 0x0400,
        Xmas        = 0x0800,
        Hive        = 0x1000,
    }

    public static class StarObjectTypeExtensions
    {
        public static bool IsActive(this StarObjectType type, Star star)
        {
            switch (type)
            {
                case StarObjectType.Event:
                    return star.LocalEvent.IsActive;
                case StarObjectType.Boss:
                    return !star.Boss.IsDefeated;
                case StarObjectType.Challenge:
                    return !star.Challenge.IsCompleted;
                case StarObjectType.Ruins:
                    return !star.Ruins.IsDefeated;
                case StarObjectType.Hive:
                    return !star.Pandemic.IsDefeated;
                default:
                    return true;
            }
        }
    }

    public struct StarObjects
    {
        public static StarObjects Create(params StarObjectType[] types)
        {
            var objects = new StarObjects();
            foreach (var type in types)
                objects.Add(type);

            return objects;
        }

        public void Add(StarObjectType type) { _objects |= (uint)type; }
        public bool Contain(StarObjectType type) { return (_objects & (uint)type) == (uint)type;  }

        public IEnumerable<StarObjectType> ToEnumerable()
        {
            var objects = _objects;
            return Enum.GetValues(typeof(StarObjectType)).OfType<StarObjectType>().Where(type => (objects & (uint)type) != 0);
        }

        private uint _objects;
    }
}
