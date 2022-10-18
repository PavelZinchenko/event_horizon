using System.Collections.Generic;
using Constructor;
using DataModel.Technology;
using GameDatabase.DataModel;
using GameDatabase.Model;

namespace GameServices.Database
{
    public interface ITechnologies
    {
        ITechnology Get(ItemId<Technology> id);
        IEnumerable<ITechnology> All { get; }
        IEnumerable<ITechnology> Dependants(ITechnology root);
        bool TryGetComponentTechnology(Component component, out ITechnology technology);
        bool TryGetShipTechnology(ItemId<Ship> ship, out ITechnology technology);
    }
}
