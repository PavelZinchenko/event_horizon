using System;
using Constructor;
using Services.Reources;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using ViewModel;
using Zenject;

namespace Gui.Constructor
{
    public class DraggableComponentObject : MonoBehaviour, IDragHandler, IPointerDownHandler, IBeginDragHandler, IEndDragHandler
    {
        [Inject] private readonly IResourceLocator _resourceLocator;

        [SerializeField] private ComponentIconViewModel _icon;
        [SerializeField] private ShipLayoutViewModel _shipLayout;
        [SerializeField] private ShipLayoutViewModel _leftPlatformLayout;
        [SerializeField] private ShipLayoutViewModel _rightPlatformLayout;
        [SerializeField] private ConstructorViewModel.CommandEvent _onCommandExecutedEvent = new ConstructorViewModel.CommandEvent();

        [Serializable]
        public class PositionChangedEvent : UnityEvent<Vector2> { }

        public void Initialize(IntegratedComponent component, PointerEventData eventData, Vector2 blockSize, ICommand removeComponentCommand)
        {
            if (removeComponentCommand != null && !removeComponentCommand.TryExecute())
                return;

            _component = component;
            _removeComponentCommand = removeComponentCommand;

            gameObject.SetActive(true);
            var size = _component.Info.Data.Layout.Size * blockSize;
            RectTransform.position = eventData.position;
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, size.x);
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, size.y);

            _icon.SetIcon(_resourceLocator.GetSprite(component.Info.Data.Icon), component.Info.Data.Layout.Data, component.Info.Data.Layout.Size, component.Info.Data.Color);

            eventData.pointerDrag = gameObject;
            ExecuteEvents.Execute<IBeginDragHandler>(gameObject, eventData, ExecuteEvents.beginDragHandler);
        }

        public void OnPointerDown(PointerEventData eventData)
        {
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
        }

        public void OnDrag(PointerEventData eventData)
        {
            RectTransform.position = eventData.position;
            var position = eventData.position;

            _shipLayout.PreviewComponent(position, _component.Info);
            _leftPlatformLayout.PreviewComponent(position, _component.Info);
            _rightPlatformLayout.PreviewComponent(position, _component.Info);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            gameObject.SetActive(false);
            var position = eventData.position;

            ICommand installCommand;
            if (!(installCommand = new InstallComponentCommand(_shipLayout, position, _component.Info, _component.KeyBinding, _component.Behaviour)).TryExecute())
                if (!(installCommand = new InstallComponentCommand(_leftPlatformLayout, position, _component.Info, _component.KeyBinding, _component.Behaviour)).TryExecute())
                    if (!(installCommand = new InstallComponentCommand(_rightPlatformLayout, position, _component.Info, _component.KeyBinding, _component.Behaviour)).TryExecute())
                        installCommand = null;

            if (_removeComponentCommand != null || installCommand != null)
                _onCommandExecutedEvent.Invoke(new ComplexCommand(_removeComponentCommand, installCommand));

            _component = null;
            _removeComponentCommand = null;
        }


        private RectTransform RectTransform { get { return _rectTransform ?? (_rectTransform = GetComponent<RectTransform>()); } }

        private RectTransform _rectTransform;
        private IntegratedComponent _component;
        private ICommand _removeComponentCommand;
    }
}
