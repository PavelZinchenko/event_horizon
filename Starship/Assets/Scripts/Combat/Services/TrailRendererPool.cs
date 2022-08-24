using System.Collections.Generic;
using GameServices.Settings;
using UnityEngine;
using Zenject;

namespace Combat.Services
{
    public class TrailRendererPool : MonoBehaviour
    {
        [SerializeField] private TrailRenderer _prefab;

        [Inject]
        private void Initialize(GameSettings gameSettings)
        {
            _powerSavingMode = gameSettings.LowQualityMode;
        }

        public TrailRenderer CreateTrailRenderer(Transform parent, float startWidth, float endWidth, Color color, float duration)
        {
            if (_powerSavingMode)
                return null;

            TrailRenderer renderer = null;

            foreach (var item in _trailRenderers)
            {
                if (Time.time - item.Value < item.Key.time)
                    continue;

                renderer = item.Key;
                _trailRenderers.Remove(renderer);
                break;
            }

            if (renderer == null)
                renderer = GameObject.Instantiate<TrailRenderer>(_prefab);

            renderer.transform.parent = parent;
            renderer.transform.localPosition = Vector3.zero;
            renderer.Clear();
            renderer.startWidth = startWidth;
            renderer.endWidth = endWidth;
            renderer.material.color = color;
            renderer.time = duration;

            return renderer;
        }

        public void ReleaseTrailRenderer(TrailRenderer renderer)
        {
            if (!this)
            {
                GameObject.DestroyImmediate(renderer.gameObject);
                return;
            }

            renderer.transform.parent = transform;
            _trailRenderers.Add(renderer, Time.time);
        }

        private void OnDestroy()
        {
            foreach (var renderer in _trailRenderers.Keys)
            {
                foreach (var item in renderer.materials)
                    GameObject.DestroyImmediate(item);

                GameObject.Destroy(renderer.gameObject);
            }

            _trailRenderers.Clear();
        }


        private readonly Dictionary<TrailRenderer, float> _trailRenderers = new Dictionary<TrailRenderer, float>();
        private bool _powerSavingMode;
    }
}
