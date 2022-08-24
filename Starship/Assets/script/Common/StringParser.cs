using System;

public struct StringParser
{
    public StringParser(string data, char separator)
    {
        _data = data;
        _separator = separator;
        _current = 0;
        _next = data.IndexOf(_separator);
    }

    public bool IsLast { get { return _next < 0; } }

    public bool TryMoveNext()
    {
        if (_next < 0)
            return false;

        _current = _next + 1;
        _next = _data.IndexOf(_separator, _current);
        return true;
    }

    public StringParser MoveNext()
    {
        if (_next < 0)
        {
            _current = _data.Length;
        }
        else
        {
            _current = _next + 1;
            _next = _data.IndexOf(_separator, _current);
        }

        return this;
    }

    public string CurrentString
    {
        get
        {
            if (_next > _current)
                return _data.Substring(_current, _next - _current);
            return _data.Substring(_current);
        }
    }

    public int CurrentInt
    {
        get
        {
            int value;
            return int.TryParse(CurrentString, out value) ? value : 0;
        }
    }

    private readonly string _data;
    private readonly char _separator;
    private int _current;
    private int _next;
}
