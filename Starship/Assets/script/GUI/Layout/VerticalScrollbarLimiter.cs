using System;
using UnityEngine;
using UnityEngine.UI;

namespace script.GUI.Layout
{
    [RequireComponent(typeof(ScrollRect))]
    public class VerticalScrollbarLimiter : MonoBehaviour
    {
        private ScrollRect _scrollRect;
        [SerializeField] public float minScrollbarSize = 0.1f;

        private void Start()
        {
            _scrollRect = GetComponent<ScrollRect>();
            _scrollRect.onValueChanged.AddListener(OnScroll);
            OnScroll(Vector2.zero);
        }

        private void OnScroll(Vector2 value)
        {
            var bar = _scrollRect.verticalScrollbar;
            if (bar.size < minScrollbarSize) bar.size = minScrollbarSize;
        }
    }
}
