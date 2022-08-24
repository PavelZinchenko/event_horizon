using System;

public struct ObscuredLong
{
    public ObscuredLong(long value)
    {
        _index = (int)(value ^ _mask) % 7;
        _value1 = _value2 = _value3 = _value4 = _value5 = _value6 = _value7 = 0;
        Value = value;
    }

    public static implicit operator ObscuredLong(long value)  // explicit byte to digit conversion operator
    {
        return new ObscuredLong(value);
    }

    public static implicit operator long(ObscuredLong data)  // implicit digit to byte conversion operator
    {
        return data.Value;
    }

    public static ObscuredLong operator --(ObscuredLong data)
    {
        return data - 1;
    }

    public static ObscuredLong operator ++(ObscuredLong data)
    {
        return data + 1;
    }

    public override string ToString()
    {
        return Value.ToString();
    }

    private long Value
    {
        get
        {
            switch (_index)
            {
                case 0:
                    return _value1 ^ _mask;
                case 1:
                    return _value2 ^ _mask;
                case 2:
                    return _value3 ^ _mask;
                case 3:
                    return _value4 ^ _mask;
                case 4:
                    return _value5 ^ _mask;
                case 5:
                    return _value6 ^ _mask;
                case 6:
                default:
                    return _value7 ^ _mask;
            }
        }
        set
        {
            switch (_index)
            {
                case 0:
                    _value1 = value ^ _mask;
                    break;
                case 1:
                    _value2 = value ^ _mask;
                    break;
                case 2:
                    _value3 = value ^ _mask;
                    break;
                case 3:
                    _value4 = value ^ _mask;
                    break;
                case 4:
                    _value5 = value ^ _mask;
                    break;
                case 5:
                    _value6 = value ^ _mask;
                    break;
                case 6:
                default:
                    _value7 = value ^ _mask;
                    break;
            }
        }
    }

    private long _value1;
    private long _value2;
    private long _value3;
    private long _value4;
    private long _value5;
    private long _value6;
    private long _value7;
    private readonly int _index;
    private static readonly long _mask = DateTime.UtcNow.Ticks;
}