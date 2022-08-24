using UnityEngine;

namespace Gui
{
    public class PlatformDependendObject : MonoBehaviour
    {
#pragma warning disable 414
        [SerializeField] bool _editorOnly = false;
        [SerializeField] bool _android = true;
        [SerializeField] bool _ios = true;
        [SerializeField] bool _standalone = true;
        [SerializeField] bool _developmentBuildOnly = false;
        [SerializeField] bool _iapVersionOnly = false;
#pragma warning restore 414
        private void Awake()
        {
            if (_editorOnly || (_developmentBuildOnly && !Debug.isDebugBuild))
            {
#if !UNITY_EDITOR
                gameObject.SetActive(false);
#endif
                return;
            }

#if !UNITY_PURCHASING
            if (_iapVersionOnly)
            {
                gameObject.SetActive(false);
                return;
            }
#endif

#if UNITY_STANDALONE
            gameObject.SetActive(_standalone);
#elif UNITY_ANDROID
            gameObject.SetActive(_android);
#elif UNITY_IOS
            gameObject.SetActive(_ios);
#endif
        }
    }
}
