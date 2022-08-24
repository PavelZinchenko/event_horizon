using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

namespace Gui.Controls
{
    public class DragHandler : UIBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        [SerializeField] private RectTransform _draggableObject;

        [SerializeField] private PointerEvent _beginDragEvent = new PointerEvent();
        [SerializeField] private PointerEvent _dragEvent = new PointerEvent();
        [SerializeField] private PointerEvent _endDragEvent = new PointerEvent();

        public void OnBeginDrag(PointerEventData eventData)
        {
            var delta = eventData.position - eventData.pressPosition;
            if (Math.Abs(delta.x) < 2*Math.Abs(delta.y))
            {
                ExecuteEvents.ExecuteHierarchy<IBeginDragHandler>(transform.parent.gameObject, eventData, ExecuteEvents.beginDragHandler);
                return;
            }

            _handleDragEvents = true;
            _draggableObject.gameObject.SetActive(true);
            _draggableObject.position = eventData.position;
            _beginDragEvent.Invoke(eventData.position);
        }

        public void OnDrag(PointerEventData eventData)
        {
            if (!_handleDragEvents)
            {
                ExecuteEvents.ExecuteHierarchy<IDragHandler>(transform.parent.gameObject, eventData, ExecuteEvents.dragHandler);
                return;
            }

            _draggableObject.position = eventData.position;
            _dragEvent.Invoke(eventData.position);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!_handleDragEvents)
            {
                ExecuteEvents.ExecuteHierarchy<IEndDragHandler>(transform.parent.gameObject, eventData, ExecuteEvents.endDragHandler);
                return;
            }

            _draggableObject.gameObject.SetActive(false);
            _handleDragEvents = false;
            _endDragEvent.Invoke(eventData.position);
        }

        private bool _handleDragEvents;

        [Serializable]
        public class PointerEvent : UnityEvent<Vector2> { }
    }
}
