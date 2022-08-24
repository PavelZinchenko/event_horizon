using System.Linq;

namespace GameDatabase.Model
{
    public struct AudioClipId
    {
        public AudioClipId(string name)
        {
            var pos = string.IsNullOrEmpty(name) ? -1 : name.IndexOf('*');
            _name = pos >= 0 ? name.Remove(pos, 1) : name;
            _loop = pos >= 0;
        }

        public string Id { get { return _name; } }
        public bool Loop { get { return _loop; } }

        public static implicit operator bool(AudioClipId prefabId)
        {
            return !string.IsNullOrEmpty(prefabId._name);
        }

        private readonly bool _loop;
        private readonly string _name;

        public static readonly AudioClipId None = new AudioClipId();
    }
}
