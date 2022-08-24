using System.Collections.Generic;
using GameDatabase.DataModel;
using GameDatabase.Model;

namespace GameDatabase
{
    public partial interface IDatabase
    {
        void LoadDefault();
        bool TryLoad(string id, out string error);
        IEnumerable<ModInfo> AvailableMods { get; }

        string Id { get; }
        string Name { get; }
        bool IsEditable { get; }

        #region temporary members
        // TODO: The database editor has to be able toedit builds, techs, skills, etc. himself. After is's implemented, remove these lines
        void SaveShipBuild(ItemId<ShipBuild> id);
        void SaveSatelliteBuild(ItemId<SatelliteBuild> id);
        #endregion
    }

    public struct ModInfo
    {
        public ModInfo(string name, string id, string path)
        {
            Id = id;
            Name = name;
            Path = path;
        }

        public readonly string Id;
        public readonly string Name;
        public readonly string Path;

        public static readonly ModInfo Default = new ModInfo(string.Empty, string.Empty, string.Empty);
    }
}
