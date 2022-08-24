using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System;
using System.Linq;
using Constructor;
using GameDatabase.Enums;
using GameDatabase.Model;
using Gui.Constructor;
using Services.Reources;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Zenject;

namespace ViewModel
{
	public class ShipLayoutViewModel : MonoBehaviour, IPointerClickHandler, /*IPointerDownHandler,*/ IEndDragHandler, IBeginDragHandler, IDragHandler
	{
	    [Inject] private readonly IResourceLocator _resourceLocator;

		public BlockViewModel WeaponBlock;
		public BlockViewModel InnerBlock;
		public BlockViewModel OuterBlock;
		public BlockViewModel IoBlock;
		public BlockViewModel EngineBlock;
		public RectTransform AllowedBlock;
		public RectTransform DeniedBlock;
		public RectTransform LockIcon;
		public ComponentIconViewModel ComponentIcon;
	    public DraggableComponentObject _draggableComponent;
        public Image BackgroundImage;

        [SerializeField] public int MinBlockSize = 64;
        [SerializeField] public Vector2 BlockSize { get { return new Vector2(MinBlockSize, MinBlockSize); } }

        [SerializeField] public ComponentEvent OnComponentInstalled = new ComponentEvent();
        [SerializeField] public ComponentEvent OnComponentRemoved = new ComponentEvent();
        [SerializeField] public ComponentSelectedEvent OnComponentSelected = new ComponentSelectedEvent();
        [SerializeField] public ComponentUnlockedEvent OnComponentUnlocked = new ComponentUnlockedEvent();

        [Serializable]
		public class ComponentEvent : UnityEvent<ComponentInfo> {};

		[Serializable]
		public class ComponentUnlockedEvent : UnityEvent<IntegratedComponent> {};

		[Serializable]
		public class ComponentSelectedEvent : UnityEvent<ShipLayoutViewModel, int> {};

		public void Reset()
		{
			_layout = null;
			Cleanup();

			var layoutElement = GetComponent<LayoutElement>();
			layoutElement.minWidth = 0;
			layoutElement.minHeight = 0;
		}

		public void Initialize(ShipLayout layout, InventoryComponents inventory)
		{
			_layout = layout;
            _inventory = inventory;
			Cleanup();
			CreateLayout();
		}

		public ShipLayout Ship { get { return _layout; } }
		public IEnumerable<IntegratedComponent> Components { get { return _layout != null ? _layout.Components : Enumerable.Empty<IntegratedComponent>(); } }

		public void PreviewComponent(Vector2 position, ComponentInfo info)
		{
			if (_layout == null)
				return;

			int x, y;
			GetComponentPosition(position, info.Data.Layout.Size, out x, out y);
			CreateSelection(info.Data, x, y);
		}

		public int InstallComponent(Vector2 position, ComponentInfo info, int keyBinding, int componentMode)
		{
			if (_layout == null)
				return -1;

			int x, y;
			var size = info.Data.Layout.Size;
			GetComponentPosition(position, size, out x, out y);

		    return InstallComponent(x, y, info, keyBinding, componentMode);
		}

        public int InstallComponent(int x, int y, ComponentInfo info, int keyBinding, int componentMode, int desiredId = -1)
        {
            if (_layout == null)
                return -1;

            ClearSelection();
            var size = info.Data.Layout.Size;

            if (!_inventory.Contains(info)) 
                return -1;

            var id = _layout.InstallComponent(info, x, y, desiredId);

            if (id >= 0)
            {
                AddComponentIcon(id, _resourceLocator.GetSprite(info.Data.Icon), info.Data.Layout, size, info.Data.Color, x, y);
                var component = _layout.GetComponent(id);
                component.KeyBinding = keyBinding;
                component.Behaviour = componentMode;
                UpdateLayout();

                OnComponentInstalled.Invoke(info);
            }

            return id;
        }

        public void OnPointerClick(PointerEventData eventData)
		{
		    var id = GetComponentAtPoint(eventData.position);
			if (id < 0)
				return;

			OnComponentSelected.Invoke(this, id);
		}

        public void OnBeginDrag(PointerEventData eventData)
        {
            var id = GetComponentAtPoint(eventData.position);
            if (id < 0 || _layout.GetComponent(id).Locked || Input.touches.Length > 1 || Time.realtimeSinceStartup - eventData.clickTime < 0.1f)
            {
                ExecuteEvents.ExecuteHierarchy<IBeginDragHandler>(transform.parent.gameObject, eventData, ExecuteEvents.beginDragHandler);
                return;
            }

            ExecuteEvents.Execute<IEndDragHandler>(gameObject, eventData, ExecuteEvents.endDragHandler);
            var command = new RemoveComponentCommand(this, id);
            _draggableComponent.Initialize(_layout.GetComponent(id), eventData, BlockSize, command);
        }

        public void OnDrag(PointerEventData eventData)
        {
            ExecuteEvents.ExecuteHierarchy<IDragHandler>(transform.parent.gameObject, eventData, ExecuteEvents.dragHandler);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            ExecuteEvents.ExecuteHierarchy<IEndDragHandler>(transform.parent.gameObject, eventData, ExecuteEvents.endDragHandler);
        }

        public void RemoveComponent(int id)
		{
			if (_layout == null)
				return;

			var component = _layout.GetComponent(id);
			if (component.Locked)
				return;

			_layout.RemoveComponent(id);
			GameObject.Destroy(_components[id].gameObject);
			_components.Remove(id);
			UpdateLayout();

			OnComponentRemoved.Invoke(component.Info);
		}

		public void UnlockComponent(int id)
		{
			if (_layout == null)
				return;

			var component = _layout.GetComponent(id);
			if (!component.Locked)
				return;

			component.Locked = false;
			var children = new List<Transform>(_components[id].transform.Cast<Transform>());
			foreach (var item in children)
				GameObject.Destroy(item.gameObject);

			OnComponentUnlocked.Invoke(component);
		}

        public ShipLayout Layout { get { return _layout; } }

		private void OnEnable()
		{
			WeaponBlock.gameObject.SetActive(false);
			InnerBlock.gameObject.SetActive(false);
			OuterBlock.gameObject.SetActive(false);
			IoBlock.gameObject.SetActive(false);
			EngineBlock.gameObject.SetActive(false);
			AllowedBlock.gameObject.SetActive(false);
			DeniedBlock.gameObject.SetActive(false);
			ComponentIcon.gameObject.SetActive(false);
			LockIcon.gameObject.SetActive(false);
			Cleanup();
		}

	    private int GetComponentAtPoint(Vector2 position)
	    {
            if (_layout == null)
                return -1;

            int x, y;
            GetComponentPosition(position, 1, out x, out y);

            return _layout[x, y].ComponentId;
        }

        private void GetComponentPosition(Vector2 point, int componentSize, out int x, out int y)
		{
			var center = transform.position;
			var scale = RectTransformHelpers.GetScreenSizeScale(GetComponent<RectTransform>());
			
			x = Mathf.RoundToInt((0.5f + (point.x - center.x)*scale.x / _width)*_layout.Size - 0.5f*componentSize);
			y = Mathf.RoundToInt((0.5f - (point.y - center.y)*scale.y / _height)*_layout.Size - 0.5f*componentSize);
		}
		
		private void ClearSelection()
		{
			foreach (var item in _selection)
				GameObject.Destroy(item.gameObject);
			_selection.Clear();
        }

		private void CreateSelection(GameDatabase.DataModel.Component component, int x, int y)
		{
			ClearSelection();

			var componentSize = component.Layout.Size;
			for (int i = 0; i < componentSize; ++i)
			{
				if (i + y < 0 || i + y >= _layout.Size)
					continue;

				for (int j = 0; j < componentSize; ++j)
				{
					if (j + x < 0 || j + x >= _layout.Size)
						continue;
					var cellType = (CellType)component.Layout[j,i];
					if (cellType == CellType.Empty)
						continue;

					var item = GameObject.Instantiate<RectTransform>(_layout.CanInstall(component, j+x, i+y) ? AllowedBlock : DeniedBlock);

					_selection.Add(item);
					SetBlockLayout(item, j+x, i+y, 1);
				}
			}
		}

		private void CreateLayout()
		{
			var areaSize = _layout.Size*MinBlockSize;
			var layoutElement = GetComponent<LayoutElement>();

			layoutElement.minWidth =  areaSize;
			layoutElement.minHeight = areaSize;

			_width = areaSize;//RectTransform.rect.width;
			_height = areaSize;//RectTransform.rect.height;

			_blocks.Clear();

			for (int i = 0; i < _layout.Size; ++i)
			{
				for (int j = 0; j < _layout.Size; ++j)
				{
					var item = CreateBlock(_layout[j,i]);
					_blocks.Add(item);
					if (item == null)
						continue;

					SetBlockLayout(item.RectTransform, j, i, 1);
				}
			}

			foreach (var item in _layout.ComponentsIndex)
			{
				var id = item.Key;
				var component = item.Value;
				int size = component.Info.Data.Layout.Size;
				AddComponentIcon(id, _resourceLocator.GetSprite(component.Info.Data.Icon), component.Info.Data.Layout, size, component.Info.Data.Color, component.X, component.Y);

				if (component.Locked)
					CreateLockIcons(_components[id].RectTransform, component.Info.Data.Layout, size);
			}

			UpdateLayout();
		}

		void UpdateLayout()
		{
			for (int i = 0; i < _layout.Size; ++i)
			{
				for (int j = 0; j < _layout.Size; ++j)
				{
					var block = _blocks[i*_layout.Size + j];
					if (block == null || block.Label == null)
						continue;

					block.Label.gameObject.SetActive(_layout[j,i].ComponentId < 0);
				}
			}
		}

		private void AddComponentIcon(int id, Sprite icon, Layout layout, int size, Color color, int x, int y)
		{
			var item = GameObject.Instantiate<ComponentIconViewModel>(ComponentIcon);
			item.SetIcon(icon, layout.Data, size, color);
			SetBlockLayout(item.RectTransform, x, y, size);
			_components.Add(id, item);
		}

		private void CreateLockIcons(RectTransform component, Layout layout, int size)
		{
			for (int i = 0; i < size; ++i)
			{
				for (int j = 0; j < size; ++j)
				{
					if ((CellType)layout[j,i] == CellType.Empty)
						continue;

					var item = GameObject.Instantiate<RectTransform>(LockIcon);

					item.SetParent(component);
					item.localScale = Vector3.one;
					item.gameObject.SetActive(true);
					item.anchoredPosition = new Vector2(j*BlockSize.x,-i*BlockSize.y);
					item.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, BlockSize.x);
					item.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, BlockSize.y);
				}
			}
		}

		private BlockViewModel CreateBlock(ShipLayout.LayoutElement cell)
		{
			switch (cell.Type)
			{
			case CellType.Outer:
				return GameObject.Instantiate<BlockViewModel>(OuterBlock);
			case CellType.Inner:
				return GameObject.Instantiate<BlockViewModel>(InnerBlock);
			case CellType.InnerOuter:
				return GameObject.Instantiate<BlockViewModel>(IoBlock);
			case CellType.Weapon:
				var item = GameObject.Instantiate<BlockViewModel>(WeaponBlock);
				item.Label.text = string.IsNullOrEmpty(cell.WeaponClass) ? "•" : cell.WeaponClass;
				return item;
			case CellType.Engine:
				return GameObject.Instantiate<BlockViewModel>(EngineBlock);
			}

			return null;
		}

		private void SetBlockLayout(RectTransform item, int x, int y, int size)
		{
			item.SetParent(RectTransform);
			item.localScale = Vector3.one;
			item.gameObject.SetActive(true);
			item.anchoredPosition = new Vector2(x*_width/_layout.Size,-y*_height/_layout.Size);
			item.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, BlockSize.x*size);
			item.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, BlockSize.y*size);
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
				    child == AllowedBlock.transform ||
				    child == DeniedBlock.transform ||
				    child == ComponentIcon.transform ||
				    child == LockIcon.transform ||
				    child == BackgroundImage.transform)
					continue;

				GameObject.Destroy(child.gameObject);
			}
			_selection.Clear();
			_components.Clear();
			_blocks.Clear();
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

		private ShipLayout _layout;
		private float _width;
		private float _height;
		private RectTransform _rectTransform;
		private List<RectTransform> _selection = new List<RectTransform>();
		private List<BlockViewModel> _blocks = new List<BlockViewModel>();
		private Dictionary<int, ComponentIconViewModel> _components = new Dictionary<int, ComponentIconViewModel>();
        private InventoryComponents _inventory;
    }
}
