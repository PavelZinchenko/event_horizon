using System;

public class BinaryData
{
    public BinaryData(byte[] data)
    {
        Data = data;
    }

    public override int GetHashCode()
    {
        if (!_hash.HasValue)
        {
            int hash = 1;
            foreach (var value in Data)
                hash *= (int)value + 1;
            _hash = hash;
        }

        return _hash.Value;
    }

    public override bool Equals(object obj)
    {
        if (obj == this)
            return true;
        var binaryData = obj as BinaryData;
        if (binaryData == null)
            return false;
        var length = binaryData.Data.Length;
        if (length != Data.Length)
            return false;
        for (int i = 0; i < length; ++i)
            if (Data[i] != binaryData.Data[i])
                return false;
        return true;
    }

    private int? _hash;
    public readonly byte[] Data;
}
