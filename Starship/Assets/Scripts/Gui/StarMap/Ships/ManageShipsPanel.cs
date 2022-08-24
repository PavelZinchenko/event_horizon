using System;
using System.Collections.Generic;
using System.Linq;
using GameServices.Player;
using Services.Gui;
using Constructor.Ships;
using GameDatabase.Enums;
using UnityEngine;
using UnityEngine.UI;
using Zenject;

namespace Gui.StarMap
{
    public class ManageShipsPanel : MonoBehaviour
    {
        [Inject] private readonly PlayerFleet _playerFleet;
        [Inject] private readonly PlayerSkills _playerSkills;

        [SerializeField] private ShipListContentFiller ContentFiller;
        [SerializeField] private ListScrollRect ShipList;
        [SerializeField] private LayoutGroup HangarLayout;
        [SerializeField] private HangarSlotInfo SlotInfoPanel;
        [SerializeField] private ToggleGroup HangarToggleGroup;

        public void InitializeWindow(WindowArgs args)
        {
            UpdateSlots();
            UpdateHangar();
            HangarToggleGroup.SetAllTogglesOff();
            Slots.First().GetComponent<Toggle>().isOn = true;
        }

        public void OnBeginShipDrag(ShipListItem item)
        {
            UnityEngine.Debug.Log("OnBeginDrag: " + item.Ship.Name);
            _draggableShip = item;
        }

        public void OnShipDrag(Vector2 position)
        {
            var corners = new Vector3[4];

            foreach (var cell in Slots)
            {
                cell.GetComponent<RectTransform>().GetWorldCorners(corners);
                cell.Highlight(IsPointInsideCorners(position, corners), _draggableShip.Ship.Model.SizeClass);
            }
        }

        public void OnEndShipDrag(Vector2 position)
        {
            UnityEngine.Debug.Log("OnEndDrag");

            var selectedSlotId = -1;
            var corners = new Vector3[4];
            for (var i = 0; i < Slots.Count; ++i)
            {
                var slot = Slots[i];
                slot.GetComponent<RectTransform>().GetWorldCorners(corners);
                slot.Highlight(false);
                if (IsPointInsideCorners(position, corners))
                    selectedSlotId = i;
            }

            if (selectedSlotId >= 0)
                TryInstallShip(selectedSlotId, _draggableShip.Ship);

            _draggableShip = null;
        }

        public void OnShipClicked(ShipListItem item)
        {
            var ship = item.Ship;
            var canInstall = ship != null && SelectedSlotId >= 0 && !Slots[SelectedSlotId].Locked && Slots[SelectedSlotId].Size >= ship.Model.SizeClass;
            SlotInfoPanel.Initialize(ship, canInstall);
        }

        public void InstallShip()
        {
            TryInstallShip(SlotInfoPanel.Ship);
        }

        public void OnSlotSelected(bool selected)
        {
            UpdateSlotInfo();
            UpdateShips();
        }

        public void RemoveShip()
        {
            TryInstallShip(null);
        }

        private bool TryInstallShip(IShip ship)
        {
            var id = SelectedSlotId;
            return id >= 0 && TryInstallShip(id, ship);
        }

        private bool TryInstallShip(int index, IShip ship)
        {
            var slot = Slots[index];
            if (!slot.TryInstall(ship))
                return false;

            _playerFleet.ActiveShipGroup[index] = ship;
            slot.GetComponent<Toggle>().isOn = true;
            UpdateSlotInfo();
            UpdateShips();
            return true;
        }

        private void UpdateSlotInfo()
        {
            var id = SelectedSlotId;
            if (id < 0)
                SlotInfoPanel.Clear();
            else
                SlotInfoPanel.Initialize(Slots[id]);
        }

        private bool IsPointInsideCorners(Vector2 point, Vector3[] corners)
        {
            if (corners.Length != 4)
                throw new System.ArgumentException();

            var maxX = corners[0].x;
            var maxY = corners[0].y;
            var minX = maxX;
            var minY = maxY;

            for (var i = 1; i < 4; ++i)
            {
                var item = corners[i];
                if (item.x > maxX) maxX = item.x;
                if (item.y > maxY) maxY = item.y;
                if (item.x < minX) minX = item.x;
                if (item.y < minY) minY = item.y;
            }

            return point.x > minX && point.x < maxX && point.y > minY && point.y < maxY;
        }

        private List<HangarItem> Slots
        {
            get
            {
                if (_slots.Count == 0)
                    _slots.AddRange(HangarLayout.transform.Cast<Transform>().Select(child => child.GetComponent<HangarItem>()).Where(item => item != null));

                return _slots;
            }
        }

        private void UpdateShips()
        {
            var ships = new HashSet<IShip>();

            ships.Clear();
            foreach (var ship in _playerFleet.Ships)
                ships.Add(ship);

            foreach (var ship in _playerFleet.ActiveShipGroup.Ships)
                ships.Remove(ship);

            var id = SelectedSlotId;
            ContentFiller.InitializeShips(ships.OrderBy(ship => ship.Id.Value).Select(ship => 
                new KeyValuePair<IShip, bool>(ship, id < 0 || (!Slots[id].Locked && Slots[id].Size >= ship.Model.SizeClass))));

            ShipList.RefreshContent();
        }

        private void UpdateSlots()
        {
            var size = SizeClass.Titan;
            var count = _playerSkills.GetAvailableHangarSlots(size);

            foreach (var slot in Slots)
            {
                while (size > SizeClass.Undefined && count <= 0)
                {
                    size--;
                    count = _playerSkills.GetAvailableHangarSlots(size);
                }

                slot.Locked = count <= 0;
                slot.Size = count > 0 ? size : SizeClass.Frigate;
                count--;
            }
        }

        private void UpdateHangar()
        {
            var order = Slots.Select(slot => slot.Locked ? SizeClass.Undefined : slot.Size);
            if (!_playerFleet.ActiveShipGroup.IsOrderValid(order))
                _playerFleet.ActiveShipGroup.Rearrange(order);

            for (var i = 0; i < Slots.Count; ++i)
            {
                var slot = Slots[i];
                if (!slot.TryInstall(_playerFleet.ActiveShipGroup[i]))
                    throw new InvalidOperationException();
            }
        }

        private int SelectedSlotId
        {
            get
            {
                var slot = HangarToggleGroup.ActiveToggles().FirstOrDefault();
                if (slot == null)
                    return -1;

                return Slots.IndexOf(slot.GetComponent<HangarItem>());
            }
        }

        private ShipListItem _draggableShip;
        private readonly List<HangarItem> _slots = new List<HangarItem>();
    }
}
