using System;
using System.Collections.Generic;
using Constructor.Ships;

namespace GameServices.Multiplayer
{
    public interface IPlayerInfo
    {
        int Id { get; }
        string Name { get; }
        int Rating { get; }

        IObservable<bool> LoadFleetObservable();
        IEnumerable<IShip> Fleet { get; }
        float PowerMultiplier { get; }
    }
}
