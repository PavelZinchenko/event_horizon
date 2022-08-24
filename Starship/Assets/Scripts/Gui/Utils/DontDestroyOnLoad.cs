using System.Collections.Generic;
using UnityEngine;

namespace Gui.Utils
{
    public class DontDestroyOnLoad : MonoBehaviour
    {
        private void Awake()
        {
            DontDestroyOnLoad(gameObject);
            _objects.Add(gameObject);
        }

        public static IList<GameObject> All { get { return _objects.AsReadOnly(); } }
        private static List<GameObject> _objects = new List<GameObject>();
    }
}
