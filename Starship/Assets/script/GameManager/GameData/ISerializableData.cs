using System.Collections.Generic;

namespace GameModel.GameData 
{
    public interface ISerializableData
    {
        string FileName { get; }
        bool IsChanged { get; }
        IEnumerable<byte> Serialize();
    }
}
