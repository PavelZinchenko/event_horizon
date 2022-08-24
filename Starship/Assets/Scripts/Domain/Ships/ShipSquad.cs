using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using GameDatabase.Enums;
using GameServices.Player;

namespace Constructor.Ships
{
    public class ShipSquad : IEnumerable<IShip>
    {
        public IShip this[int index]
        {
            get { return index >= 0 && index < _ships.Count ? _ships[index] : null; }
            set
            {
                if (index < 0)
                    throw new IndexOutOfRangeException();

                while (_ships.Count <= index)
                    _ships.Add(null);

                _ships[index] = value;

                IsChanged = true;
            }
        }

        public bool IsChanged { get; set; }

        public int Count { get { return _ships.Count; } }

        public void Add(IShip ship)
        {
            var index = -1;
            for (var i = 0; i < _ships.Count; ++i)
            {
                if (index < 0 && _ships[i] == null)
                    index = i;
                if (_ships[i] == ship)
                    return;
            }

            if (index < 0)
                _ships.Add(ship);
            else
                _ships[index] = ship;

            IsChanged = true;
        }

        public void Remove(IShip ship)
        {
            var index = _ships.IndexOf(ship);
            if (index < 0)
                return;

            _ships[index] = null;
            IsChanged = true;
        }

        public void Clear()
        {
            _ships.Clear();
            IsChanged = true;
        }

        public IEnumerable<IShip> Ships
        {
            get { return _ships.Where(ship => ship != null); }
        }

        public bool IsOrderValid(IEnumerable<SizeClass> order)
        {
            var index = 0;
            foreach (var slotSize in order)
            {
                if (index >= _ships.Count)
                    break;

                var shipSize = _ships[index] == null ? SizeClass.Undefined : _ships[index].Model.SizeClass;

                if (shipSize > slotSize)
                    return false;

                index++;
            }

            return true;
        }

        public void Rearrange(IEnumerable<SizeClass> newOrder)
        {
            var order = newOrder.ToArray();
            var oldShips = Ships.ToArray();
            Clear();

            foreach (var ship in oldShips)
            {
                var position = -1;
                for (var i = 0; i < order.Length; ++i)
                {
                    if (order[i] == ship.Model.SizeClass)
                    {
                        position = i;
                        break;
                    }

                    if (order[i] > ship.Model.SizeClass && (position < 0 || order[i] < order[position]))
                        position = i;
                }

                if (position >= 0)
                {
                    order[position] = (SizeClass)(-1);
                    this[position] = ship;
                }
                else
                {
                    UnityEngine.Debug.Log("Rearrange(): ship removed - " + ship.Name);
                }
            }

            IsChanged = true;
        }

        public bool CheckIfValid(PlayerSkills playerSkills, bool removeInvalid = false)
        {
            var slots = new ShipsSlots(playerSkills);

            var valid = true;

            for (var i = 0; i < _ships.Count; ++i)
            {
                var ship = _ships[i];
                if (ship == null)
                    continue;

                if (slots.TryTakeBestSlot(ship.Model.SizeClass))
                    continue;

                valid = false;
                if (removeInvalid)
                    _ships[i] = null;
                else
                    break;
            }

            IsChanged |= removeInvalid && !valid;

            return valid;
        }

        public IEnumerator<IShip> GetEnumerator()
        {
            return _ships.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private readonly List<IShip> _ships = new List<IShip>();

        private struct ShipsSlots
        {
            public ShipsSlots(PlayerSkills playerSkills)
            {
                _slots = Enum.GetValues(typeof(SizeClass)).OfType<SizeClass>().Where(size => size != SizeClass.Undefined).
                    ToDictionary<SizeClass,SizeClass,int>(size => size, playerSkills.GetAvailableHangarSlots);
            }

            public bool TryTakeBestSlot(SizeClass sizeClass)
            {
                var size = sizeClass;

                int count;
                while (_slots.TryGetValue(size, out count))
                {
                    if (count > 0)
                    {
                        _slots[size] = count - 1;
                        return true;
                    }

                    size++;
                }

                return false;
            }

            private readonly Dictionary<SizeClass, int> _slots;
        }
    }
}
