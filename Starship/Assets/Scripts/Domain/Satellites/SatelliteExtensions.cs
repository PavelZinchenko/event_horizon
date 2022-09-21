﻿using System.Collections.Generic;
using System.Linq;
using Database.Legacy;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Model;
using Session.Content;

namespace Constructor.Satellites
{
    public static class SatelliteExtensions
    {
        public static ISatellite CreateCopy(this ISatellite satellite)
        {
            if (satellite == null)
                return null;

            return new CommonSatellite(satellite.Information, satellite.Components);
        }

        public static ISatellite FromSatelliteData(IDatabase database, SatelliteData data,
            List<ShipComponentsData.Component> orphanedComponents = null)
        {
            var satellite = database.GetSatellite(new ItemId<Satellite>(data.Id));
            if (satellite == null)
            {
                orphanedComponents?.AddRange(data.Components.Components);
                return null;
            }

            return new CommonSatellite(satellite, data.Components.FromShipComponentsData(database));
        }

        public static SatelliteData ToSatelliteData(this ISatellite satellite)
        {
            if (satellite == null)
                return new SatelliteData { Id = 0, Components = new ShipComponentsData() };

            var info = new SatelliteData
            {
                Id = satellite.Information.Id.Value,
                Components = satellite.Components.ToShipComponentsData()
            };

            return info;
        }

        public static bool IsSuitable(this Satellite satellite, SizeClass shipClass, float shipModelScale)
        {
            if (satellite.SizeClass != SizeClass.Undefined)
                return shipClass >= satellite.SizeClass;

            return shipModelScale >= satellite.ModelScale * 2;
        }

        public static IEnumerable<SatelliteBuild> SuitableFor(this IEnumerable<SatelliteBuild> satellites, Ship ship)
        {
            return satellites.Where(item => item.Satellite.IsSuitable(ship.SizeClass, ship.ModelScale));
        }

        public static IEnumerable<SatelliteBuild> LimitClass(this IEnumerable<SatelliteBuild> satellites,
            DifficultyClass shipClass)
        {
            return satellites.Where(item => item.DifficultyClass <= shipClass);
        }
        
        #region Obsolete
        public static ISatellite FromSatelliteInfo(IDatabase database, FleetData.SatelliteInfoV2 info)
        {
            if (string.IsNullOrEmpty(info.Id))
                return null;

            int id;
            return new CommonSatellite(database.GetSatellite(int.TryParse(info.Id, out id) ? new ItemId<Satellite>(id) : LegacySatelliteNames.GetId(info.Id)),
                info.Components.Select(item => ComponentExtensions.Deserialize(database, item)));
        }

        public static ISatellite FromSatelliteInfoObsolete(IDatabase database, FleetData.SatelliteInfoV2 info)
        {
            if (string.IsNullOrEmpty(info.Id))
                return null;

            int id;
            return new CommonSatellite(database.GetSatellite(int.TryParse(info.Id, out id) ? new ItemId<Satellite>(id) : LegacySatelliteNames.GetId(info.Id)),
                info.Components.Select(item => ComponentExtensions.DeserializeObsolete(database, item)));
        }
        #endregion
    }
}
