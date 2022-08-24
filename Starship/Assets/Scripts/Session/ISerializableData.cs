using System.Collections.Generic;

namespace Session
{
    public interface ISerializableData
    {
        string FileName { get; }
        bool IsChanged { get; }
        IEnumerable<byte> Serialize();
    }
}
