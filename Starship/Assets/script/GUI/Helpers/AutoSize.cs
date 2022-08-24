using UnityEngine;

[RequireComponent(typeof(RectTransform))]
[ExecuteInEditMode]
public class AutoSize : MonoBehaviour
{
    [SerializeField] bool _symmetrycal;
	[SerializeField] float HorizontalMargin;
	[SerializeField] float VerticalMargin;

    private void Awake()
    {
        var content = GetComponent<RectTransform>();

        float xmin = 0f, xmax = 0f, ymin = 0f, ymax = 0f;
        var isfirst = true;

        foreach (Transform child in content)
        {
            if (!child.gameObject.activeSelf)
                continue;

            var rectTransform = child.GetComponent<RectTransform>();
            var offset = rectTransform.anchoredPosition;
            var width = rectTransform.rect.width/2;
            var height = rectTransform.rect.height/2;

            if (isfirst)
            {
                xmax = offset.x + width;
                xmin = offset.x - width;
                ymax = offset.y + height;
                ymin = offset.y - height;
                isfirst = false;
            }
            else
            {
                if (xmin > offset.x - width) xmin = offset.x - width;
                if (xmax < offset.x + width) xmax = offset.x + width;
                if (ymin > offset.y - height) ymin = offset.y - height;
                if (ymax < offset.y + height) ymax = offset.y + height;
            }
        }

        if (_symmetrycal)
        {
            var width = Mathf.Max(-xmin, xmax);
            xmin = -width;
            xmax = width;
            var height = Mathf.Max(-ymin, ymax);
            ymin = -height;
            ymax = height;
        }

		content.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, ymax - ymin + 2*HorizontalMargin);
        //content.anchoredPosition = new Vector2((xmax+xmin)/2, (ymax+ymin)/2);
		content.SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, xmax - xmin + 2*VerticalMargin);
        //content.transform.localScale = Vector3.one * _zoom;
    }
}
