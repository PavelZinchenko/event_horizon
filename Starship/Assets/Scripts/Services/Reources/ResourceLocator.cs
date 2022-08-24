using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GameDatabase;
using GameDatabase.Model;
using UnityEditor;
using UnityEngine;
using Zenject;

namespace Services.Reources
{
    public class ResourceLocator : MonoBehaviour, IResourceLocator
    {
        [Inject] private readonly IDatabase _database;

        [SerializeField] private Sprite[] _shipSprites;
        [SerializeField] private Sprite[] _shipIconSprites;
        [SerializeField] private Sprite[] _componentSprites;
        [SerializeField] private Sprite[] _satelliteSprites;
        [SerializeField] private Sprite[] _controlButtonSprites;
        [SerializeField] private Sprite[] _guiIconSprites;
        [SerializeField] private AudioClip[] _audioClips;
        [SerializeField] private Texture2D[] _nebulaTextures;

        public Sprite GetSprite(SpriteId spriteId)
        {
            if (!spriteId) return null;

            Sprite sprite;

            switch (spriteId.Category)
            {
                case SpriteId.Type.Component:
                    sprite = GetComponentSprite(spriteId.Id);
                    break;
                case SpriteId.Type.Ship:
                    sprite = GetShipSprite(spriteId.Id);
                    break;
                case SpriteId.Type.ShipIcon:
                    sprite = GetShipIconSprite(spriteId.Id);
                    break;
                case SpriteId.Type.Satellite:
                    sprite = GetSatelliteSprite(spriteId.Id);
                    break;
                case SpriteId.Type.ActionButton:
                    sprite = GetControlButtonSprite(spriteId.Id);
                    break;
                case SpriteId.Type.GuiIcon:
                    sprite = GetGuiIcon(spriteId.Id);
                    break;
                case SpriteId.Type.AvatarIcon:
                    sprite = GetSprite("Textures/Avatars/" + spriteId.Id);
                    break;
                case SpriteId.Type.ArtifactIcon:
                    sprite = GetSprite("Textures/Artifacts/" + spriteId.Id);
                    break;
                case SpriteId.Type.Ammunition:
                    sprite = GetSprite("Textures/Bullets/" + spriteId.Id);
                    break;
                case SpriteId.Type.Effect:
                    sprite = GetSprite("Textures/Effects/" + spriteId.Id);
                    break;
                default:
                    return GetSprite(spriteId.Id);
            }

            if (sprite == null && _database != null)
                sprite = _database.GetImage(spriteId.Id).Sprite;

            return sprite;
        }

        public AudioClip GetAudioClip(AudioClipId id)
        {
            AudioClip audioClip;
            if (!id) return null;

            return _audio.TryGetValue(id.Id, out audioClip) ? audioClip : _database.GetAudioClip(id.Id).AudioClip;
        }

        public Texture2D GetNebulaTexture(int seed)
        {
            return _nebulaTextures[seed % _nebulaTextures.Length];
        }

        public Sprite GetSprite(string name)
        {
            var sprite = Resources.Load<Sprite>(name);
            return sprite;
        }

        [Inject]
        private void Initialize()
        {
#if UNITY_EDITOR
            var prefab = Resources.Load<ResourceLocator>("ResourceLocator");

            _shipSprites = prefab._shipSprites = LoadAllAssets<Sprite>("/Sprites/Ships").ToArray();
            _shipIconSprites = prefab._shipIconSprites = LoadAllAssets<Sprite>("/Sprites/ShipIcons").ToArray();
            _componentSprites = prefab._componentSprites = LoadAllAssets<Sprite>("/Sprites/Components").ToArray();
            _satelliteSprites = prefab._satelliteSprites = LoadAllAssets<Sprite>("/Sprites/Satellites").ToArray();
            _controlButtonSprites = prefab._controlButtonSprites = LoadAllAssets<Sprite>("/Resources/Textures/GUI/Controls").ToArray();
            _audioClips = prefab._audioClips = LoadAllAssets<AudioClip>("/Audio").ToArray();

            PrefabUtility.SavePrefabAsset(prefab.gameObject);
#endif

            foreach (var item in _shipSprites)
                _ships.Add(item.name, item);
            foreach (var item in _shipIconSprites)
                _shipIcons.Add(item.name, item);
            foreach (var item in _componentSprites)
                _components.Add(item.name, item);
            foreach (var item in _satelliteSprites)
                _satellites.Add(item.name, item);
            foreach (var item in _controlButtonSprites)
                _controlButtons.Add(item.name, item);
            foreach (var item in _guiIconSprites)
                _guiIcons.Add(item.name, item);
            foreach (var item in _audioClips)
                _audio.Add(item.name, item);
        }

#if UNITY_EDITOR
        private IEnumerable<T> LoadAllAssets<T>(string path) where T : UnityEngine.Object
        {
            var files =
                Directory.GetFiles(Application.dataPath + path, "*", SearchOption.AllDirectories)
                    .Where(file => !file.EndsWith(".meta"));
            foreach (var file in files)
            {
                var assetPath = "Assets" + file.Replace(Application.dataPath, "").Replace('\\', '/');
                var asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(assetPath);
                if (asset != null)
                    yield return asset;
            }
        }
#endif

        private Sprite GetShipSprite(string id)
        {
            Sprite sprite;
            return _ships.TryGetValue(id, out sprite) ? sprite : null;
        }

        private Sprite GetShipIconSprite(string id)
        {
            Sprite sprite;
            return _shipIcons.TryGetValue(id, out sprite) || _ships.TryGetValue(id, out sprite) ? sprite : null;
        }

        private Sprite GetComponentSprite(string id)
        {
            Sprite sprite;
            return _components.TryGetValue(id, out sprite) ? sprite : null;
        }

        private Sprite GetSatelliteSprite(string id)
        {
            Sprite sprite;
            return _satellites.TryGetValue(id, out sprite) ? sprite : null;
        }

        private Sprite GetControlButtonSprite(string id)
        {
            Sprite sprite;
            return _controlButtons.TryGetValue(id, out sprite) ? sprite : null;
        }

        private Sprite GetGuiIcon(string id)
        {
            Sprite sprite;
            return _guiIcons.TryGetValue(id, out sprite) ? sprite : null;
        }

        private readonly Dictionary<string, Sprite> _ships = new Dictionary<string, Sprite>();
        private readonly Dictionary<string, Sprite> _shipIcons = new Dictionary<string, Sprite>();
        private readonly Dictionary<string, Sprite> _components = new Dictionary<string, Sprite>();
        private readonly Dictionary<string, Sprite> _satellites = new Dictionary<string, Sprite>();
        private readonly Dictionary<string, Sprite> _controlButtons = new Dictionary<string, Sprite>();
        private readonly Dictionary<string, Sprite> _guiIcons = new Dictionary<string, Sprite>();
        private readonly Dictionary<string, AudioClip> _audio = new Dictionary<string, AudioClip>();
    }
}
