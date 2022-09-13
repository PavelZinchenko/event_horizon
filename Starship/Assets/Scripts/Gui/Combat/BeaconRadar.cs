using System;
using UnityEngine;
using Combat.Component.Unit;
using Combat.Scene;
using Combat.Unit;
using UnityEngine.UI;

namespace Gui.Combat
{
    public class BeaconRadar : MonoBehaviour
    {
        [SerializeField] private Image Image;
        [SerializeField] private Image Background;
        [SerializeField] private float Size = 24;

        public void Open(IUnit unit, IScene scene)
        {
            _scene = scene;
            _unit = unit;

            Initialize();
            gameObject.SetActive(true);
        }

        public IUnit Unit => _unit;

        private void Update()
        {
            if (!_unit.IsActive())
            {
                Close();
                return;
            }

            var itemPosition = _unit.Body.Position;
            var position = _scene.ViewPoint.Direction(itemPosition);
            var cameraHeight = _mainCamera.orthographicSize;
            var cameraWidth = cameraHeight * _mainCamera.aspect;

            var x = position.x / cameraWidth;
            var y = position.y / cameraHeight;

            if (x > -1 && x < 1 && y > -1 && y < 1)
            {
                Image.enabled = false;
                Background.enabled = false;
                return;
            }

            Image.enabled = true;
            Background.enabled = true;

            var width = _scene.Settings.AreaWidth/2;
            var height = _scene.Settings.AreaHeight/2;
            var dx = ((position.x > 0 ? position.x : -position.x) - cameraWidth) / (width - cameraWidth);
            var dy = ((position.y > 0 ? position.y : -position.y) - cameraHeight) / (height - cameraHeight);
            var scale = Mathf.Max(1 - Mathf.Max(dx, dy), 0.25f);

            var max = Mathf.Max(x > 0 ? x : -x, y > 0 ? y : -y);
            var offset = scale * Size;

            x = offset + 0.5f * (x / max + 1) * (_screenSize.x - 2 * offset);
            y = offset + 0.5f * (y / max + 1) * (_screenSize.y - 2 * offset);

            gameObject.SetActive(true);

            RectTransform.anchoredPosition = new Vector2(x, y);
            RectTransform.localScale = Vector3.one * scale;
        }

        public void Close()
        {
            _unit = null;

            if (this)
                gameObject.SetActive(false);
        }

        public RectTransform RectTransform
        {
            get
            {
                if (_rectTransform == null)
                    _rectTransform = GetComponent<RectTransform>();
                return _rectTransform;
            }
        }

        private void Initialize()
        {
            _screenSize = RectTransform.parent.GetComponent<RectTransform>().rect.size;
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, Size * 2);
            RectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Size * 2);
        }

        private void Start()
        {
            _mainCamera = Camera.main;
        }

        private Vector2 _screenSize;
        private RectTransform _rectTransform;
        private IUnit _unit;
        private IScene _scene;
        private Camera _mainCamera;
    }
}
