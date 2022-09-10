using System;
using System.Collections.Generic;
using System.Linq;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Model;
using UnityEngine.Assertions;

namespace Constructor.Ships.Modification
{
    public class LayoutModifications
    {
        public event Action DataChangedEvent = () => { };

        public LayoutModifications(Ship ship)
        {
            _ship = ship;
            _size = ship.Layout.Size;
            _layout = new char[_size * _size];
            _ready = false;
            _clean = true;
        }

        public Layout BuildLayout()
        {
            if (_clean)
            {
                return _ship.Layout;
            }
            EnsureReady();
            return GameDatabase.Model.Layout.FromRawDataUnchecked(_layout);
        }

        public bool TryAddCell(int x, int y, CellType cellType)
        {
            if (!IsCellValid(x, y, cellType))
                return false;

            _clean = false;
            Layout[x + y * _size] = (char)cellType;
            DataChangedEvent.Invoke();

            return true;
        }

        public int TotalExtraCells()
        {
            var shipLayout = _ship.Layout.Data;
            var count = 0;
            for (var i = 0; i < _layout.Length; i++)
            {
                var t = _layout[i];
                if (t != shipLayout[i]) count++;
            }

            return count;
        }

        public int ExtraCells()
        {
            var shipLayout = _ship.Layout.Data;
            var count = 0;
            for (var i = 0; i < _layout.Length; i++)
            {
                var t = _layout[i];
                if (t != shipLayout[i] && t != (char)CellType.Custom) count++;
            }

            return count;
        }

        public void Reset()
        {
            Initialize();
            _clean = true;
            DataChangedEvent.Invoke();
        }

        public void Deserialize(byte[] data)
        {
            if (data == null || data.Length == 0)
            {
                Reset();
                return;
            }

            Initialize();
            _clean = false;

            var index = 0;
            var dataIndex = 0;

            while (dataIndex < data.Length && index < _layout.Length)
            {
                if (data[dataIndex] == (byte)CellType.Empty)
                {
                    dataIndex++;
                    index += data[dataIndex++];
                    continue;
                }

                var x = index % _size;
                var y = index / _size;

                if (_layout[index] == (char)CellType.Custom && IsCellValid(x, y, (CellType)data[dataIndex]))
                    _layout[index] = (char)data[dataIndex];

                index++;
                dataIndex++;
            }

            DataChangedEvent.Invoke();
        }

        public IEnumerable<byte> Serialize()
        {
            EnsureReady();

            if (ExtraCells() == 0)
                yield break;

            var shipLayout = _ship.Layout.Data;
            byte emptyCells = 0;
            for (var i = 0; i < _layout.Length; ++i)
            {
                var isEmpty = _layout[i] == (byte)CellType.Custom || _layout[i] == (byte)CellType.Empty ||
                              _layout[i] == shipLayout[i];
                if (isEmpty)
                {
                    if (emptyCells == 0)
                        yield return (byte)CellType.Empty;

                    if (emptyCells == 0xff)
                    {
                        yield return emptyCells;
                        emptyCells = 0;
                        yield return (byte)CellType.Empty;
                    }

                    emptyCells++;
                }
                else
                {
                    if (emptyCells > 0)
                    {
                        yield return emptyCells;
                        emptyCells = 0;
                    }

                    yield return (byte)_layout[i];
                }
            }

            if (emptyCells > 0)
                yield return emptyCells;
        }

        private void Initialize()
        {
            var layout = _ship.Layout.GetRawData();
            var size = _ship.Layout.Size;
            
            const int px = 1;
            const int nx = -1;
            var py = -size;
            var ny = -size;

            Assert.AreEqual(size * size, _layout.Length);

            for (var i = 0; i < size; ++i)
            {
                for (var j = 0; j < size; ++j)
                {
                    var x = j;
                    var y = i;

                    var index = x + y * size;
                    var cellType = layout[index];
                    if (cellType != (char)CellType.Empty)
                    {
                        _layout[i * size + j] = cellType;
                    }
                    else if ((y != 0 && layout[index + ny] != (char)CellType.Empty) ||
                             (x != 0 && layout[index + nx] != (char)CellType.Empty) ||
                             (x != _size - 1 && layout[index + px] != (char)CellType.Empty) ||
                             (y != _size - 1 && layout[index + py] != (char)CellType.Empty))
                    {
                        _layout[i * size + j] = (char)CellType.Custom;
                    }
                    else
                    {
                        _layout[i * size + j] = (char)CellType.Empty;
                    }
                }
            }

            _ready = true;
        }

        public bool IsCellValid(int x, int y, CellType type)
        {
            EnsureReady();
            if (x < 0 || y < 0 || x >= _size || y >= _size)
                return false;

            if (type == CellType.Empty || type == CellType.Custom)
                return false;

            var layout = _ship.Layout;
            if (layout[x, y] != (char)CellType.Empty)
                return false;

            var index = x + y * layout.Size;
            if (_layout[index] == (char)CellType.Empty)
                return false;

            if (type == CellType.Outer)
                return true;

            var l = (CellType)layout[x - 1, y];
            var r = (CellType)layout[x + 1, y];
            var t = (CellType)layout[x, y - 1];
            var b = (CellType)layout[x, y + 1];

            if (type != l && type != r && type != t && type != b) return false;
            if (type != CellType.Weapon) return true;

            l = GetCell(x - 1, y);
            r = GetCell(x + 1, y);
            t = GetCell(x, y - 1);
            b = GetCell(x, y + 1);
            var tl = GetCell(x - 1, y - 1);
            var tr = GetCell(x + 1, y - 1);
            var bl = GetCell(x - 1, y + 1);
            var br = GetCell(x + 1, y + 1);

            if (t == CellType.Weapon && l == CellType.Weapon && tl != CellType.Weapon) return false;
            if (t == CellType.Weapon && r == CellType.Weapon && tr != CellType.Weapon) return false;
            if (b == CellType.Weapon && l == CellType.Weapon && bl != CellType.Weapon) return false;
            if (b == CellType.Weapon && r == CellType.Weapon && br != CellType.Weapon) return false;

            if (t == CellType.Weapon && b == CellType.Weapon)
                return l == CellType.Weapon || r == CellType.Weapon;
            if (l == CellType.Weapon && r == CellType.Weapon)
                return t == CellType.Weapon || b == CellType.Weapon;

            return true;
        }

        private CellType GetCell(int x, int y)
        {
            if (x < 0 || x >= _size || y < 0 || y >= _size)
                return CellType.Empty;

            return (CellType)_layout[y * _size + x];
        }

        private bool _ready;
        // Clean means that this layout modification is unchanged
        private bool _clean;
        private readonly int _size;
        private readonly char[] _layout;


        private void EnsureReady()
        {
            if (_ready) return;
            Initialize();
        }

        private char[] Layout
        {
            get
            {
                EnsureReady();

                return _layout;
            }
        }

        private readonly Ship _ship;
    }
}
