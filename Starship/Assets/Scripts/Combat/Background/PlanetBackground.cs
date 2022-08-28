using System;
using Game.Exploration;
using Services.Reources;
using UnityEngine;
using Zenject;

namespace Combat.Background
{
    public class PlanetBackground : MonoBehaviour
    {
        [SerializeField] private float _size = 50f;
        [SerializeField] private Material _gasPlanetMaterial;
        [SerializeField] private Material _barrenPlanetMaterial;
        [SerializeField] private Material _infectedPlanetMaterial;

        [Inject]
        public void Initialize(IResourceLocator resourceLocator, Planet planet)
        {
            _width = _height = _size * Screen.width / Screen.height;
            _planet = planet;

            Primitives.CreatePlane(gameObject.GetMesh(), _width, _height, 8);

            switch (planet.Type)
            {
                case PlanetType.Gas:
                    InitializeGasMaterial();
                    break;
                case PlanetType.Infected:
                    InitializeInfectedMaterial(resourceLocator);
                    break;
                case PlanetType.Barren:
                case PlanetType.Terran:
                    InitializeBarrenMaterial(resourceLocator);
                    break;
                default:
                    throw new ArgumentException("PlanetBackground: Wrong planet type - " + planet.Type);
            }
        }

        private void InitializeGasMaterial()
        {
            // Copy material to avoid modifying global material at runtime
            _gasPlanetMaterial = new Material(_gasPlanetMaterial);
            gameObject.AddComponent<MeshRenderer>().sharedMaterial = _gasPlanetMaterial;

            //var random = new System.Random(planet.Seed);
            //_material.SetTexture("_DecalTex", resourceLocator.GetNebulaTexture(random.Next()));
            //_material.SetTexture("_CloudsTex", resourceLocator.GetNebulaTexture(random.Next()));
            _gasPlanetMaterial.color = Color.Lerp(_planet.Color, Color.black, 0.75f);
        }

        private void InitializeBarrenMaterial(IResourceLocator resourceLocator)
        {
            // Copy material to avoid modifying global material at runtime
            _barrenPlanetMaterial = new Material(_barrenPlanetMaterial);
            gameObject.AddComponent<MeshRenderer>().sharedMaterial = _barrenPlanetMaterial;

            var random = new System.Random(_planet.Seed);
            _barrenPlanetMaterial.SetTexture("_CloudsTex", resourceLocator.GetNebulaTexture(random.Next()));
            _barrenPlanetMaterial.color = Color.Lerp(_planet.Color, Color.black, 0.3f);
        }

        private void InitializeInfectedMaterial(IResourceLocator resourceLocator)
        {
            // Copy material to avoid modifying global material at runtime
            _infectedPlanetMaterial = new Material(_infectedPlanetMaterial);
            gameObject.AddComponent<MeshRenderer>().sharedMaterial = _infectedPlanetMaterial;

            var random = new System.Random(_planet.Seed);
            _infectedPlanetMaterial.SetTexture("_CloudsTex", resourceLocator.GetNebulaTexture(random.Next()));
            _infectedPlanetMaterial.color = Color.Lerp(_planet.Color, Color.black, 0.3f);
        }

        private void LateUpdate()
        {
            switch (_planet.Type)
            {
                case PlanetType.Gas:
                    UpdateGasMaterial();
                    break;
                case PlanetType.Infected:
                    UpdateInfectedMaterial();
                    break;
                case PlanetType.Barren:
                case PlanetType.Terran:
                    UpdateBarrenMaterial();
                    break;
            }
        }

        private void UpdateBarrenMaterial()
        {
            var offset = transform.position;

            offset.x /= _width;
            offset.y /= _height;
            offset.x -= Mathf.FloorToInt(offset.x);
            offset.y -= Mathf.FloorToInt(offset.y);
            _barrenPlanetMaterial.mainTextureOffset = offset;
        }

        private void UpdateInfectedMaterial()
        {
            var offset = transform.position;

            offset.x /= _width;
            offset.y /= _height;
            offset.x -= Mathf.FloorToInt(offset.x);
            offset.y -= Mathf.FloorToInt(offset.y);
            _infectedPlanetMaterial.mainTextureOffset = offset;
        }

        private void UpdateGasMaterial()
        {
            var offset = transform.position;

            offset.x /= _width;
            offset.y /= _height;
            offset.x -= Mathf.FloorToInt(offset.x);
            offset.y -= Mathf.FloorToInt(offset.y);
            _gasPlanetMaterial.mainTextureOffset = offset;

            var decalOffset = offset * 2;
            decalOffset.x -= Mathf.FloorToInt(offset.x);
            decalOffset.y -= Mathf.FloorToInt(offset.y);
            _gasPlanetMaterial.SetTextureOffset("_DecalTex", decalOffset);

            var cloudOffset = offset * 3;
            cloudOffset.x -= Mathf.FloorToInt(offset.x);
            cloudOffset.y -= Mathf.FloorToInt(offset.y);
            _gasPlanetMaterial.SetTextureOffset("_CloudsTex", cloudOffset);
        }

        private Planet _planet;
        private float _width;
        private float _height;
    }
}
