using Services.Localization;
using UnityEngine;
using UnityEngine.UI;
using Utils;
using Zenject;

namespace Gui.Utils
{
    public class CanvasLocalizer : MonoBehaviour
    {
        [Inject] private ILocalization _localization;

        private void Localize(GameObject target)
        {
            foreach (var text in target.GetComponentsInChildren<Text>(true))
            {
                var localized = _localization.GetString(text.text);

#if UNITY_EDITOR
                if (!string.IsNullOrEmpty(localized) && localized[0] == '$')
                {
                    var sb = new System.Text.StringBuilder("Localized string not found: ");
                    var obj = text.transform;
                    while (obj != null)
                    {
                        sb.Insert(0, '.');
                        sb.Insert(0, obj.name);
                        obj = obj.parent;
                    }

                    OptimizedDebug.Log(sb.ToString());
                }
#endif
                text.text = localized;
            }
        }

        private void Start()
        {
            Localize(gameObject);
        }
    }
}
