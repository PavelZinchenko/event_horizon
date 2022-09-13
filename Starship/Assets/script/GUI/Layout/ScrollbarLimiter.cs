using System;
using UnityEngine;
using UnityEngine.UI;

namespace script.GUI.Layout
{
    [RequireComponent(typeof(ScrollRect))]
    public class ScrollbarLimiter : MonoBehaviour
    {
        private ScrollRect _scrollRect;
        [SerializeField] public float minScrollbarSize = 0.1f;
        [SerializeField] public bool isHorizontal = false;

        private void Start()
        {
            _scrollRect = GetComponent<ScrollRect>();
            _scrollRect.onValueChanged.AddListener(OnScroll);
            OnScroll(Vector2.zero);
        }

        private void UpdateValue()
        {
            var bar = isHorizontal ? _scrollRect.horizontalScrollbar : _scrollRect.verticalScrollbar;
            if (bar.size < minScrollbarSize) bar.size = minScrollbarSize; 
        }

        // Scrollbars like to randomly update their scrollbar size, so checking this each tick is required for good visuals
        // This should be pretty cheap too
        private void Update()
        {
            UpdateValue();
        }

        private void OnScroll(Vector2 value)
        {
            UpdateValue();
        }
    }
}
