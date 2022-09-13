using UnityEngine;
using Combat.Scene;
using GameServices.Settings;
using Zenject;

namespace Combat
{
	namespace Camera
	{
		[RequireComponent(typeof(UnityEngine.Camera))]
		public class CombatCamera : MonoBehaviour 
		{
			[SerializeField] GameObject Background;
			[SerializeField] float MinSize = 5;
            [SerializeField] float MaxSize = 25;
            //[SerializeField] float Border = 5;
			[SerializeField] float ZoomSpeed = 5;
			[SerializeField] float LinearSpeed = 5;
            [SerializeField] private float ZoomOverride = -1;

		    [Inject]
		    private void Initialize(IScene scene, GameSettings gameSettings)
		    {
		        _scene = scene;
		        _gameSettings = gameSettings;
		    }

            private void Start()
            {
	            if (Background == null) Background = null;
	            _camera = gameObject.GetComponent<UnityEngine.Camera>();
	            _zoom = ZoomOverride >= 0 ? ZoomOverride : _gameSettings.CameraZoom;
            }
			
			private void LateUpdate()
			{
				var viewRect = _scene.ViewRect;
				var orthographicSize = Mathf.Clamp(0.5f*Mathf.Max(viewRect.width/_camera.aspect, viewRect.height), MinSize + _zoom*(MaxSize - MinSize), MaxSize);
				var delta = Mathf.Min(ZoomSpeed*Time.unscaledDeltaTime, 1);
				var size = _camera.orthographicSize;
				size += (orthographicSize - size) * delta;
				_camera.orthographicSize = size;
				var position = Vector2.Lerp(transform.position, viewRect.center, LinearSpeed*Time.unscaledDeltaTime);
				gameObject.Move(position);

				// ReSharper disable once Unity.NoNullPropagation
				Background?.Move(position);
			}

			private float _zoom;
            private IScene _scene;
		    private GameSettings _gameSettings;
		    private UnityEngine.Camera _camera;
		}
    }
}
