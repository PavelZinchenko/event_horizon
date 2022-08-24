using System.Collections.Generic;
using System.Linq;
using Combat.Component.Ship;
using Combat.Domain;
using Services.Localization;
using Services.Reources;
using UnityEngine;
using UnityEngine.UI;
using ViewModel;
using Zenject;

namespace Gui.Combat
{
    public class ShipList : MonoBehaviour
    {
        [Inject] private readonly ILocalization _localization;
        [Inject] private readonly IResourceLocator _resourceLocator;

        [SerializeField] private ScrollRect ShipsArea;
        [SerializeField] private CanvasGroup CanvasGroup;
        [SerializeField] private Text ShipNameText;

        public IShipInfo SelectedShip { get { return _selectedIndex >= 0 ? _fleet.Ships[_selectedIndex] : null; } }
        public int SelectedShipIndex { get { return _selectedIndex; } set { _targetIndex = value; UpdateSelection(); } }

        public void Initialize(IFleetModel fleet, int activeShipIndex)
        {
            _fleet = fleet;
            _targetIndex = activeShipIndex;

            IEnumerator<IShipInfo> enumerator;
            if (fleet.Ships.Count <= MaxShipCount)
            {
                enumerator = fleet.Ships.Take(MaxShipCount).GetEnumerator();
            }
            else
            {
                _firstShipIndex = Mathf.Max(_targetIndex - MaxShipCount / 2, 0);
                enumerator = fleet.Ships.Skip(_firstShipIndex).Take(MaxShipCount).GetEnumerator();
            }

            RectTransform item = null;
            foreach (Transform transform in ShipsArea.content)
            {
                item = transform.GetComponent<RectTransform>();
                if (enumerator.MoveNext())
                    UpdateShipItem(item, enumerator.Current);
                else
                    item.gameObject.SetActive(false);
            }

            while (enumerator.MoveNext())
            {
                var newItem = (RectTransform)Instantiate(item);
                newItem.SetParent(item.parent);
                newItem.localScale = Vector3.one;
                UpdateShipItem(newItem, enumerator.Current);
            }

            UpdateSelection();
        }

        public void OnContentMoved(Vector2 position)
        {
            UpdateSelection();
        }

        private void UpdateSelection()
        {
            var areaWidth = ShipsArea.GetComponent<RectTransform>().rect.width;
            var currentPosition = ShipsArea.horizontalNormalizedPosition * (ShipsArea.content.rect.width - areaWidth) + areaWidth / 2;

            if (_targetIndex >= 0)
            {
                var item = ShipsArea.content.GetChild(_targetIndex - _firstShipIndex).GetComponent<RectTransform>();
                _selectedIndex = _targetIndex;
                _selectedDistance = currentPosition - item.anchoredPosition.x;

                if (Mathf.Abs(_selectedDistance) < areaWidth * 0.01f)
                    _targetIndex = -1;

                if (ShipNameText != null)
                    ShipNameText.text = _localization.GetString(_fleet.Ships[_selectedIndex].ShipData.Name);
            }
            else
            {
                var deltaMin = float.MaxValue;
                int index = _firstShipIndex;
                _selectedIndex = -1;
                foreach (RectTransform item in ShipsArea.content)
                {
                    if (!item.gameObject.activeSelf)
                        continue;

                    var delta = currentPosition - item.anchoredPosition.x;
                    if (Mathf.Abs(delta) < Mathf.Abs(deltaMin) && _fleet.Ships[index].Status != ShipStatus.Destroyed)
                    {
                        deltaMin = delta;
                        _selectedIndex = index;
                    }
                    index++;
                }
                _selectedDistance = deltaMin;
            }

            if (ShipNameText != null && _selectedIndex >= 0)
                ShipNameText.text = _localization.GetString(_fleet.Ships[_selectedIndex].ShipData.Name);
        }

        private void UpdateShipItem(RectTransform item, IShipInfo ship)
        {
            item.gameObject.SetActive(true);
            item.localScale = Vector3.one;
            var viewModel = item.GetComponent<SelectShipPanelItemViewModel>();

            viewModel.Icon.sprite = _resourceLocator.GetSprite(ship.ShipData.Model.ModelImage);
            viewModel.Icon.color = ship.ShipData.ColorScheme.HsvColor;
            viewModel.DisabledIcon.gameObject.SetActive(ship.Status == ShipStatus.Destroyed);
            viewModel.ConditionText.gameObject.SetActive(ship.Condition > 0 && ship.Condition < 1);
            viewModel.ConditionText.text = Mathf.FloorToInt(ship.Condition * 100) + "%";
            viewModel.SetClass(ship.ShipData.ExtraThreatLevel);
            viewModel.SetLevel(ship.ShipData.Experience.Level);
        }

        private void Update()
        {
            var delta = _selectedDistance * 5;
            var x = Mathf.Lerp(ShipsArea.velocity.x, delta, 0.1f);
            ShipsArea.velocity = Mathf.Abs(x) > 1 ? new Vector2(x, 0) : Vector2.zero;
        }

        private int _targetIndex = -1;
        private float _selectedDistance;
        private int _selectedIndex;
        private int _firstShipIndex;
        private IFleetModel _fleet;
        private const int MaxShipCount = 24;
    }
}
