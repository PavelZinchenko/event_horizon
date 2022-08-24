using System;
using System.Collections.Generic;
using GameModel.Serialization;

namespace Services.Storage
{
    public class GameDataStub : IGameData
    {
        public long GameId { get; private set; }
        public long TimePlayed { get; private set; }
        public long DataVersion { get; private set; }
        public string ModId { get; private set; }

        public IEnumerable<byte> Serialize()
        {
            foreach (var value in Helpers.Serialize(1)) // formatId
                yield return value;

            foreach (var value in Helpers.Serialize(_data.Count))
                yield return value;

            foreach (var item in _data)
            {
                foreach (var value in Helpers.Serialize(item.Key))
                    yield return value;
                foreach (var value in Helpers.Serialize(item.Value))
                    yield return value;
            }
        }

        public bool TryDeserialize(long gameId, long timePlayed, long dataVersion, string modId, byte[] data, int startIndex)
        {
            try
            {
                var index = startIndex;
                var formatId = Helpers.DeserializeInt(data, ref index);
                if (formatId != 1)
                    return false;

                var count = Helpers.DeserializeInt(data, ref index);
                for (var i = 0; i < count; ++i)
                {
                    var id = Helpers.DeserializeString(data, ref index);
                    var content = GetSubArray(data, ref index);
                    _data.Add(id, content);
                }

                GameId = gameId;
                TimePlayed = timePlayed;
                DataVersion = dataVersion;
                ModId = modId;
                return true;
            }
            catch (Exception e)
            {
                UnityEngine.Debug.Log(e.Message);
                return false;
            }
        }

        public void CreateNewGame(string modId, bool keepPurchases = true)
        {
            throw new System.NotImplementedException();
        }

        private static byte[] GetSubArray(byte[] data, ref int index)
        {
            if (index >= data.Length)
                return new byte[0];

            int size = Helpers.DeserializeInt(data, ref index);
            var newData = new byte[size];
            Array.Copy(data, index, newData, 0, size);
            index += size;
            return newData;
        }

        private readonly Dictionary<string, byte[]> _data = new Dictionary<string, byte[]>();
    }
}