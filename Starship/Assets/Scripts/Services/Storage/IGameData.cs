using System.Collections.Generic;

namespace Services.Storage
{
    public interface IGameData
    {
        long GameId { get; }
        long TimePlayed { get; }
        long DataVersion { get; }
        string ModId { get; }

        IEnumerable<byte> Serialize();
        bool TryDeserialize(long gameId, long timePlayed, long dataVersion, string modId, byte[] data, int startIndex);
        void CreateNewGame(string modId, bool keepPurchases = true);
    }
}
