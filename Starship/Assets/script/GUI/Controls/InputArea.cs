using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections.Generic;

[AddComponentMenu("UI/Input Area", 1000)]
public class InputArea : Graphic
{
	public override void Rebuild(CanvasUpdate update) {}
	public override bool Raycast(Vector2 sp, Camera eventCamera) { return isActiveAndEnabled; }

	protected InputArea() {}
}
