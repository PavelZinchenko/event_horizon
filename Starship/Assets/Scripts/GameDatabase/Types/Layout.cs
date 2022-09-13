using System;
using System.Linq;
using System.Text;
using GameDatabase.Enums;
using UnityEngine;

namespace GameDatabase.Model
{
    public struct Layout
    {
        public Layout(string data)
        {
            _data = null;
            CellCount = 0;
            _size = 0;
            _stringCache = null;

            Data = data;
        }
        public Layout(char[] data):this(new string(data))
        {
        }
        private Layout(char[] data, int cellsCount)
        {
            _data = data;
            CellCount = cellsCount;
            _size = (int) Math.Sqrt(data.Length);
            _stringCache = null;
        }

        /// <summary>
        /// Constructs new Layout from provided char[] without performing any checks on it
        /// </summary>
        /// <param name="data">Raw layout data</param>
        /// <returns>Constructed Layout</returns>
        public static Layout FromRawDataUnchecked(char[] data, int cellsCount)
        {
            return new Layout(data, cellsCount);
        }

        public string Data
        {
            get
            {
                if (_stringCache != null) return _stringCache;
                if (_data == null) return _stringCache = string.Empty;
                return _stringCache = new string(_data);
            }
            private set
            {
                if (string.IsNullOrEmpty(value))
                {
                    _data = new[] { DefaultValue };
                    _size = 1;
                    CellCount = 0;
                    return;
                }

                var length = value.Length;
                _size = (int)Math.Sqrt(length);

                _stringCache = null;

                if (_size * _size == length)
                {
                    _stringCache = value;
                    _data = new char[length];
                }
                else
                {
                    _size++;
                    var actualLength = _size * _size;
                    _data = new char[actualLength];
                    for (var i = length; i < actualLength; i++)
                    {
                        _data[i] = DefaultValue;
                    }
                }

                value.CopyTo(0, _data, 0, length);

                CellCount = 0;
                foreach (var cell in value)
                {
                    if (cell != DefaultValue && cell != CustomCell) CellCount++;
                }
            }
        }

        public char[] GetRawData()
        {
            return _data;
        }

        public char this[int x, int y]
        {
            get
            {
                if (x < 0 || x >= _size || y < 0 || y >= _size)
                    return DefaultValue;

                return GetUnchecked(x, y);
            }
            private set
            {
                if (x < 0 || x >= _size || y < 0 || y >= _size)
                    return;

                _stringCache = null;
                _data[y * _size + x] = value;
            }
        }

        public char GetUnchecked(int x, int y) => _data[y * _size + x];

        public int CellCount { get; private set; }

        public int Size
        {
            get => _size;
            set
            {
                if (value == _size || value < 1)
                    return;

                var length = _size * _size;
                if (_data == null)
                {
                    _data = new char[length];
                    for (var i = 0; i < length; i++)
                    {
                        _data[i] = DefaultValue;
                    }
                }

                var oldData = _data;
                _data = new char[length];
                Array.Copy(oldData, _data, length);
                // If new size is larger then we fill the rest of layout with empty cells, otherwise we do nothign
                if (value >= _size)
                {
                    for (var i = oldData.Length; i < length; i++)
                    {
                        _data[i] = DefaultValue;
                    }
                }

                _size = value;
            }
        }

        private int _size;
        private char[] _data;
        private string _stringCache;
        private const char DefaultValue = (char)CellType.Empty;
        private const char CustomCell = (char)CellType.Custom;
    }
}
