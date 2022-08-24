using System;
using System.Linq;
using System.Text;
using GameDatabase.Enums;

namespace GameDatabase.Model
{
    public struct Layout
    {
        public Layout(string data)
        {
            _data = null;
            _cellcount = 0;
            _size = 0;

            Data = data;
        }

        public string Data
        {
            get => _data?.ToString() ?? string.Empty;
            private set
            {
                _data = new StringBuilder();
                if (string.IsNullOrEmpty(value))
                {
                    _data.Append(_defaultValue);
                    _size = 1;
                    _cellcount = 0;
                    return;
                }

                _data.Append(value);
                _cellcount = value.Count(cell => cell != _defaultValue && cell != _customCell);

                var length = value.Length;
                _size = (int)Math.Sqrt(length);
                if (_size * _size == length)
                    return;

                _size++;
                _data.Append(_defaultValue, _size * _size - length);
            }
        }

        public char this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= _size || y < 0 || y >= _size)
                    return _defaultValue;

                return _data[y * _size + x];
            }
            private set
            {
                if (x < 0 || x >= _size || y < 0 || y >= _size)
                    return;

                _data[y * _size + x] = value;
            }
        }

        public int CellCount => _cellcount;

        public int Size
        {
            get => _size;
            set
            {
                if (value == _size || value < 1)
                    return;

                if (_data == null) _data = new StringBuilder();

                if (value < _size)
                {
                    _data.Length = value * value;
                }
                else
                {
                    _data.Append(_defaultValue, value * value - _size * _size);
                }

                _size = value;
            }
        }

        private int _size;
        private int _cellcount;
        private StringBuilder _data;
        private const char _defaultValue = (char)CellType.Empty;
        private const char _customCell = (char)CellType.Custom;
    }
}
