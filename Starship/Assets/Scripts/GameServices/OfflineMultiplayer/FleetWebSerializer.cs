using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Constructor.Ships;
using GameDatabase;
using Session.Content;
using Ionic.Zlib;
using Helpers = GameModel.Serialization.Helpers;

namespace GameServices.Multiplayer
{
    public static class FleetWebSerializer
    {
        public static string SerializeFleet(IEnumerable<IShip> ships)
        {
            var data = ZlibStream.CompressBuffer(SerializeFleetInternal(ships).ToArray());
            var base64 = EncodeUrlBase64(Convert.ToBase64String(data, Base64FormattingOptions.None));

            if (base64.Length > 16384)
            {
                UnityEngine.Debug.LogException(new OverflowException("SerializeFleet: Size is too big"));
                return string.Empty;
            }

            UnityEngine.Debug.LogWarning("SerializeFleet: " + base64.Length + " bytes");

            return base64;
        }

        public static IEnumerable<IShip> DeserializeFleet(IDatabase database, string base64)
        {
            var data = System.Convert.FromBase64String(DecodeUrlBase64(base64));
            var index = 0;

            try
            {
                data = ZlibStream.UncompressBuffer(data);
            }
            catch (Exception e)
            {
                UnityEngine.Debug.LogError("DeserializeFleet: " + e.Message);
            }

            var version = Helpers.DeserializeInt(data, ref index);
            if (version == 1)
                return DeserializeV1(database, data, index);
            if (version == 2)
                return DeserializeV2(database, data, index);
            if (version == 3)
                return DeserializeV3(database, data, index);
            if (version == 4)
                return DeserializeV4(database, data, index);

            return Enumerable.Empty<IShip>();
        }

        public static string EncodeUrlBase64(string base64)
        {
            var builder = new StringBuilder(base64);
            builder.Replace('+', '-');
            builder.Replace('/', '_');
            while (builder.Length > 0 && builder[builder.Length - 1] == '=')
                builder.Remove(builder.Length - 1, 1);

            return builder.ToString();
        }

        public static string DecodeUrlBase64(string urlBase64)
        {
            var builder = new StringBuilder(urlBase64);
            builder.Replace('-', '+');
            builder.Replace('_', '/');
            while (builder.Length % 4 != 0)
                builder.Append('=');

            return builder.ToString();
        }

        private static IEnumerable<IShip> DeserializeV1(IDatabase database, byte[] data, int index)
        {
            var count = Helpers.DeserializeInt(data, ref index);

            for (var i = 0; i < count; ++i)
            {
                var shipInfo = FleetData.ShipInfoV1.Deserialize(data, ref index);
                yield return ShipExtensions.FromShipInfoObsolete(database, shipInfo.ToShipInfo(database));
            }
        }

        private static IEnumerable<IShip> DeserializeV2(IDatabase database, byte[] data, int index)
        {
            var count = Helpers.DeserializeInt(data, ref index);

            for (var i = 0; i < count; ++i)
            {
                var shipInfo = FleetData.ShipInfoObsolete.DeserializeObsolete(data, ref index, database);
                yield return ShipExtensions.FromShipInfoObsolete(database, shipInfo);
            }
        }

        private static IEnumerable<IShip> DeserializeV3(IDatabase database, byte[] data, int index)
        {
            var count = Helpers.DeserializeInt(data, ref index);

            for (var i = 0; i < count; ++i)
            {
                var shipInfo = FleetData.ShipInfoObsolete.Deserialize(data, ref index);
                yield return ShipExtensions.FromShipInfoObsolete(database, shipInfo);
            }
        }

        private static IEnumerable<IShip> DeserializeV4(IDatabase database, byte[] data, int index)
        {
            var count = Helpers.DeserializeInt(data, ref index);

            for (var i = 0; i < count; ++i)
            {
                var shipInfo = ShipData.Deserialize(data, ref index);
                yield return ShipExtensions.FromShipData(database, shipInfo);
            }
        }

        private static IEnumerable<byte> SerializeFleetInternal(IEnumerable<IShip> ships)
        {
            foreach (var value in Helpers.Serialize(Version))
                yield return value;
            foreach (var value in Helpers.Serialize(ships.Count()))
                yield return value;

            foreach (var ship in ships)
                foreach (var value in ship.ToShipData().Serialize())
                    yield return value;
        }

        private const int Version = 4;
    }
}
