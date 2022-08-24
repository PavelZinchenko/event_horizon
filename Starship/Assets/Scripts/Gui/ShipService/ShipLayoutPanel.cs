using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using Constructor;
using GameDatabase.Enums;
using GameDatabase.Model;
using Services.Reources;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using ViewModel;
using Zenject;

namespace Gui.ShipService
{
    public class ShipLayoutPanel : MonoBehaviour, IPointerClickHandler
    {
        [Inject] private readonly IResourceLocator _resourceLocator;

        public BlockViewModel WeaponBlock;
        public BlockViewModel InnerBlock;
        public BlockViewModel OuterBlock;
        public BlockViewModel IoBlock;
        public BlockViewModel EngineBlock;
        public BlockViewModel CustomBlock;
        public BlockViewModel Selection;
        public Image BackgroundImage;

        [SerializeField] public int MinBlockSize = 64;
        [SerializeField] public Vector2 BlockSize { get { return new Vector2(MinBlockSize, MinBlockSize); } }

        [SerializeField] public BlockSelectedEvent _onBlockSelected = new BlockSelectedEvent();

        [Serializable]
        public class BlockSelectedEvent : UnityEvent<int,int> { };

        public void Reset()
        {
            _layout = new Layout(string.Empty);
            Cleanup();

            var layoutElement = GetComponent<LayoutElement>();
            layoutElement.minWidth = 0;
            layoutElement.minHeight = 0;
        }

        public void Initialize(Layout layout)
        {
            _layout = layout;
            Cleanup();
            CreateLayout();
        }

        public void ClearSelection()
        {
            Selection.gameObject.SetActive(false);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            int x, y;
            GetComponentPosition(eventData.position, 1, out x, out y);
            if ((CellType)_layout[x, y] != CellType.Custom) return;
            _onBlockSelected.Invoke(x, y);

            Selection.gameObject.SetActive(true);
            SetBlockLayout(Selection.RectTransform, x, y, 1);
        }

        private void GetComponentPosition(Vector2 point, int componentSize, out int x, out int y)
        {
            var center = transform.position;
            var scale = RectTransformHelpers.GetScreenSizeScale(GetComponent<RectTransform>());

            x = Mathf.RoundToInt((0.5f + (point.x - center.x) * scale.x / _width) * _layout.Size - 0.5f * componentSize);
            y = Mathf.RoundToInt((0.5f - (point.y - center.y) * scale.y / _height) * _layout.Size - 0.5f * componentSize);
        }

        private void OnEnable()
        {
            WeaponBlock.gameObject.SetActive(false);
            InnerBlock.gameObject.SetActive(false);
            OuterBlock.gameObject.SetActive(false);
            IoBlock.gameObject.SetActive(false);
            EngineBlock.gameObject.SetActive(false);
            CustomBlock.gameObject.SetActive(false);
            Cleanup();
        }

        private void CreateLayout()
        {
            var areaSize = _layout.Size * MinBlockSize;
            var layoutElement = GetComponent<LayoutElement>();

            layoutElement.minWidth = areaSize;
            layoutElement.minHeight = areaSize;

            _width = areaSize;//RectTransform.rect.width;
            _height = areaSize;//RectTransform.rect.height;

            _blocks.Clear();

            for (int i = 0; i < _layout.Size; ++i)
            {
                for (int j = 0; j < _layout.Size; ++j)
                {
                    var item = CreateBlock((CellType)_layout[j, i]);
                    _blocks.Add(item);
                    if (item == null)
                        continue;

                    SetBlockLayout(item.RectTransform, j, i, 1);
                }
            }
        }

        private BlockViewModel CreateBlock(/*ShipLayout.LayoutElement cell*/CellType cell)
        {
            switch (cell)
            {
                case CellType.Outer:
                    return GameObject.Instantiate<BlockViewModel>(OuterBlock);
                case CellType.Inner:
                    return GameObject.Instantiate<BlockViewModel>(InnerBlock);
                case CellType.InnerOuter:
                    return GameObject.Instantiate<BlockViewModel>(IoBlock);
                case CellType.Weapon:
                    var item = GameObject.Instantiate<BlockViewModel>(WeaponBlock);
                    //item.Label.text = string.IsNullOrEmpty(cell.WeaponClass) ? "•" : cell.WeaponClass;
                    return item;
                case CellType.Engine:
                    return GameObject.Instantiate<BlockViewModel>(EngineBlock);
                case CellType.Custom:
                    return GameObject.Instantiate<BlockViewModel>(CustomBlock);
            }

            return null;
        }

        private void SetBlockLayout(RectTransform item, int x, int y, int size)
        {
            item.SetParent(RectTransform);
            item.localScale = Vector3.one;
            item.gameObject.SetActive(true);
            item.anchoredPosition = new Vector2(x * _width / _layout.Size, -y * _height / _layout.Size);
            item.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, BlockSize.x * size);
            item.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, BlockSize.y * size);
        }

        private void Cleanup()
        {
            foreach (Transform child in transform)
            {
                if (child == WeaponBlock.transform ||
                    child == InnerBlock.transform ||
                    child == OuterBlock.transform ||
                    child == EngineBlock.transform ||
                    child == IoBlock.transform ||
                    child == CustomBlock.transform ||
                    child == Selection.transform ||
                    child == BackgroundImage.transform)
                    continue;

                GameObject.Destroy(child.gameObject);
            }

            _blocks.Clear();
            ClearSelection();
        }

        private RectTransform RectTransform
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

        private Layout _layout;
        private float _width;
        private float _height;
        private RectTransform _rectTransform;
        private readonly List<BlockViewModel> _blocks = new List<BlockViewModel>();
    }
}
