using UnityEngine;

public static class RectTransformHelpers
{
	public static Vector2 GetScreenSize(RectTransform rectTransform)
	{
		var corners = new Vector3[4];
		rectTransform.GetWorldCorners(corners);
		var width = Vector2.Distance(corners[1], corners[2]);
		var height = Vector2.Distance(corners[0], corners[1]);
		return new Vector2(width, height);
	}

	public static Vector2 GetScreenSizeScale(RectTransform rectTransform)
	{
		var corners = new Vector3[4];
		rectTransform.GetWorldCorners(corners);
		var width = Vector2.Distance(corners[1], corners[2]);
		var height = Vector2.Distance(corners[0], corners[1]);
		var size = rectTransform.rect.size;
		size.x /= width;
		size.y /= height;
		return size;
	}
}
