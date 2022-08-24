using System;
using System.Collections.Generic;
using System.Linq;
using GameModel.Serialization;

namespace Services.Storage
{
    public class CloudDataAdapter : IDataStorage
    {
        public CloudDataAdapter(byte[] data)
        {
            _data = data;
        }
        
        public void Save(IGameData gameData)
        {
            throw new System.NotSupportedException();
        }

        public bool TryLoad(IGameData gameData, string mod)
        {
            try
            {
                int index = 0;
                var formatId = Helpers.DeserializeInt(_data, ref index);
                if (formatId < 2 && string.IsNullOrEmpty(mod))
                {
                    LoadLegacyData(gameData);
                    return true;
                }

                var gameId = Helpers.DeserializeLong(_data, ref index);
                var time = Helpers.DeserializeLong(_data, ref index);
                var version = formatId >= 3 ? Helpers.DeserializeLong(_data, ref index) : 0;  // TODO: remove condition
                var modId = formatId >= 4 ? Helpers.DeserializeString(_data, ref index) : null;

                if (!LocalStorage.IsModsEqual(mod, modId))
                {
                    UnityEngine.Debug.LogException(new Exception("CloudDataAdapter.TryLoad: Invalid mod id"));
                    return false;
                }

                if (!gameData.TryDeserialize(gameId, time, version, modId, _data, index))
                {
                    UnityEngine.Debug.Log("Game load failed");
                    return false;
                }

                UnityEngine.Debug.Log("Game loaded: " + gameData.GameId);

                return true;
            }
            catch (System.Exception e)
            {
                UnityEngine.Debug.Log(e.Message);
                return false;
            }
        }

        public bool TryImportOriginalSave(IGameData gameData, string mod)
        {
            throw new NotImplementedException();
        }

        public void LoadLegacyData(IGameData gameData)
        {
            var data = ConvertFromLegacyGameData();

            int index = 0;
            if (!gameData.TryDeserialize(System.DateTime.UtcNow.Ticks, 0, 0, string.Empty, data, index))
            {
                UnityEngine.Debug.Log("Legacy game loading failed");
                throw new ArgumentException("Legacy game loading failed");
            }

            UnityEngine.Debug.Log("Legacy game loaded: " + gameData.GameId);
        }

        private byte[] ConvertFromLegacyGameData()
        {
            var index = 0;
            var version = Helpers.DeserializeInt(_data, ref index);
            if (version > 1)
                return new byte[] {};

            var converted = new List<byte>();

            converted.AddRange(Helpers.Serialize(1)); // formatId
            converted.AddRange(Helpers.Serialize(13)); // count
            converted.AddRange(Helpers.Serialize(Session.Content.GameData.Name)); CopySubArray(converted, _data, ref index);
            converted.AddRange(Helpers.Serialize(Session.Content.StarMapData.Name)); CopySubArray(converted, _data, ref index);
            converted.AddRange(Helpers.Serialize(Session.Content.ShopData.Name)); CopySubArray(converted, _data, ref index);
            converted.AddRange(Helpers.Serialize(Session.Content.EventData.Name)); CopySubArray(converted, _data, ref index);
            converted.AddRange(Helpers.Serialize(Session.Content.BossData.Name)); CopySubArray(converted, _data, ref index);
            converted.AddRange(Helpers.Serialize(Session.Content.RegionData.Name)); CopySubArray(converted, _data, ref index);
            converted.AddRange(Helpers.Serialize(Session.Content.WormholeData.Name)); CopySubArray(converted, _data, ref index);
            converted.AddRange(Helpers.Serialize(Session.Content.CommonObjectData.Name)); CopySubArray(converted, _data, ref index);
            converted.AddRange(Helpers.Serialize(Session.Content.ResearchData.Name)); CopySubArray(converted, _data, ref index);
            converted.AddRange(Helpers.Serialize(Session.Content.StatisticsData.Name)); CopySubArray(converted, _data, ref index);
            converted.AddRange(Helpers.Serialize(Session.Content.ResourcesData.Name)); CopySubArray(converted, _data, ref index);
            converted.AddRange(Helpers.Serialize(Session.Content.UpgradesData.Name)); CopySubArray(converted, _data, ref index);

            return converted.ToArray();
        }

        private static void CopySubArray(List<byte> target, byte[] source, ref int index)
        {
            if (index >= source.Length)
                return;

            var size = Helpers.DeserializeInt(source, ref index);
            target.AddRange(Helpers.Serialize(size));
            target.AddRange(source.Skip(index).Take(size));
            index += size;
        }

        public static byte[] Serialize(IGameData data, string mod = null)
        {
            var buffer = new List<byte>();

            buffer.AddRange(Helpers.Serialize(4)); // formatId
            buffer.AddRange(Helpers.Serialize(data.GameId));
            buffer.AddRange(Helpers.Serialize(data.TimePlayed));
            buffer.AddRange(Helpers.Serialize(data.DataVersion));
            buffer.AddRange(Helpers.Serialize(mod));
            buffer.AddRange(data.Serialize());

            return buffer.ToArray();
        }

        private byte[] _data;
    }
}
