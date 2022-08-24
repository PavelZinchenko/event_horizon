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
            _layout = new char[_size*_size];
            Initialize();
        }

        public Layout BuildLayout()
        {
            return new Layout(new string(_layout));
        }

        public bool TryAddCell(int x, int y, CellType cellType)
        {
            if (!IsCellValid(x, y, cellType))
                return false;

            _layout[x + y*_size] = (char)cellType;
            DataChangedEvent.Invoke();

            return true;
        }

        public int TotalExtraCells()
        {
            var shipLayout = _ship.Layout.Data;
            return _layout.Where((t, i) => t != shipLayout[i]).Count();
        }

        public int ExtraCells()
        {
            var shipLayout = _ship.Layout.Data;
            return _layout.Where((t, i) => t != shipLayout[i] && t != (char)CellType.Custom).Count();
        }

        public void Reset()
        {
            Initialize();
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
            if (ExtraCells() == 0)
                yield break;

            var shipLayout = _ship.Layout.Data;
            byte emptyCells = 0;
            for (var i = 0; i < _layout.Length; ++i)
            {
                var isEmpty = _layout[i] == (byte)CellType.Custom || _layout[i] == (byte)CellType.Empty || _layout[i] == shipLayout[i];
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
            var layout = _ship.Layout;
            var size = layout.Size;

            Assert.AreEqual(size*size, _layout.Length);

            for (var i = 0; i < size; ++i)
            {
                for (var j = 0; j < size; ++j)
                {
                    var x = j;
                    var y = i;

                    var cellType = layout[x, y];
                    if (cellType != (char)CellType.Empty)
                    {
                        _layout[i * size + j] = cellType;
                    }
                    else if (layout[x, y - 1] != (char)CellType.Empty || layout[x - 1, y] != (char)CellType.Empty ||
                             layout[x + 1, y] != (char)CellType.Empty || layout[x, y + 1] != (char)CellType.Empty)
                    {
                        _layout[i * size + j] = (char)CellType.Custom;
                    }
                    else
                    {
                        _layout[i * size + j] = (char)CellType.Empty;
                    }
                }
            }
        }

        public bool IsCellValid(int x, int y, CellType type)
        {
            if (x < 0 || y < 0 || x >= _size || y >= _size)
                return false;

            if (type == CellType.Empty || type == CellType.Custom)
                return false;

            var layout = _ship.Layout;
            if (layout[x,y] != (char)CellType.Empty)
                return false;

            var index = x + y*layout.Size;
            if (_layout[index] == (char)CellType.Empty)
                return false;

            if (type == CellType.Outer)
                return true;

            var l = (CellType)layout[x-1,y];
            var r = (CellType)layout[x +1,y];
            var t = (CellType)layout[x,y-1];
            var b = (CellType)layout[x,y+1];

            if (type != l && type != r && type != t && type != b) return false;
            if (type != CellType.Weapon) return true;

            l = GetCell(x-1, y);
            r = GetCell(x+1, y);
            t = GetCell(x, y-1);
            b = GetCell(x, y+1);
            var tl = GetCell(x-1,y-1);
            var tr = GetCell(x+1,y-1);
            var bl = GetCell(x-1,y+1);
            var br = GetCell(x+1,y+1);

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

            return (CellType)_layout[y*_size + x];
        }

        private readonly int _size;
        private readonly char[] _layout;
        private readonly Ship _ship;
    }
}
