using System;
using Constructor;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using ViewModel;

namespace Gui.Constructor
{
    public class ComponentDragHandler : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler, IPointerClickHandler
    {
        [SerializeField] private ComponentViewModel _component;
        [SerializeField] private ShipLayoutViewModel _shipLayout;
        [SerializeField] private DraggableComponentObject _draggableComponent;
        [SerializeField] private ConstructorViewModel _constructor;
        [SerializeField] private UnityEvent _clickedEvent = new UnityEvent();

        public void OnBeginDrag(PointerEventData eventData)
        {
            var delta = eventData.position - eventData.pressPosition;

            if (Math.Abs(delta.x) < 2 * Math.Abs(delta.y) || 
                _constructor.IsUniqueItemInstalled(_component.Component.Data) || 
                !_component.Component.CreateComponent(_constructor.ShipSize).IsSuitable(_constructor.Ship.Model))
            {
                ExecuteEvents.ExecuteHierarchy<IBeginDragHandler>(transform.parent.gameObject, eventData, ExecuteEvents.beginDragHandler);
                return;
            }

            ExecuteEvents.Execute<IEndDragHandler>(gameObject, eventData, ExecuteEvents.endDragHandler);
            var keyBinding = _constructor.GetDefaultKey(_component.Component.Data.Id);
            _draggableComponent.Initialize(new IntegratedComponent(_component.Component, 0, 0, 0, keyBinding, 0, false), eventData, _shipLayout.BlockSize, null);
        }

        public void OnDrag(PointerEventData eventData)
        {
            ExecuteEvents.ExecuteHierarchy<IDragHandler>(transform.parent.gameObject, eventData, ExecuteEvents.dragHandler);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            ExecuteEvents.ExecuteHierarchy<IEndDragHandler>(transform.parent.gameObject, eventData, ExecuteEvents.endDragHandler);
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (eventData.dragging)
                return;

            _clickedEvent.Invoke();
        }
    }
}
