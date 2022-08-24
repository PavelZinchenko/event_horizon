using UnityEngine;
using UnityEngine.UI;

namespace Gui.Actors
{
    [RequireComponent(typeof(Button))]
    [RequireComponent(typeof(Image))]
    [DisallowMultipleComponent]
    [SelectionBase]
    [AddComponentMenu("UI/Actors/SimpleButton", 0)]
    public class SimpleButtonActor : MonoBehaviour
    {
        private void Reset()
        {
            var button = GetComponent<Button>();
            var colors = ColorBlock.defaultColorBlock;
            colors.normalColor = new Color(1, 1, 1, 0);
            colors.highlightedColor = new Color(1, 1, 1, 0);
            colors.pressedColor = new Color(1, 1, 1, 1);
            colors.disabledColor = new Color(0, 0, 0, 1);
            button.colors = colors;

            if (button.targetGraphic == null || button.targetGraphic.gameObject == gameObject)
            {
                var focusObject = new GameObject("Focus", typeof(Image), typeof(RectTransform), typeof(LayoutElement));
                focusObject.transform.parent = transform;
                var layoutElement = focusObject.GetComponent<LayoutElement>();
                layoutElement.ignoreLayout = true;
                var rectTransform = focusObject.GetComponent<RectTransform>();
                rectTransform.anchorMin = Vector2.zero;
                rectTransform.anchorMax = Vector2.one;
                rectTransform.offsetMax = Vector2.zero;
                rectTransform.offsetMin = Vector2.zero;
                rectTransform.localScale = Vector3.one;

                var focusImage = focusObject.GetComponent<Image>();
                focusImage.color = new Color32(80, 192, 255, 64);
                focusImage.type = Image.Type.Sliced;
#if UNITY_EDITOR
                focusImage.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/ui_content1_bg.png");
#endif
                button.targetGraphic = focusImage;
            }

            var image = GetComponent<Image>();
            image.color = new Color32(80, 192, 255, 255);
            image.type = Image.Type.Sliced;
#if UNITY_EDITOR
            image.sprite = UnityEditor.AssetDatabase.LoadAssetAtPath<Sprite>("Assets/Sprites/UI/ui_content1.png");
#endif

            gameObject.SetActive(false);
            gameObject.SetActive(true);
        }
    }
}
