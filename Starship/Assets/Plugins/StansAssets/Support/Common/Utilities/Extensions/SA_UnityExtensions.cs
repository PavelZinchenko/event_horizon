////////////////////////////////////////////////////////////////////////////////
//  
// @module Assets Common Lib
// @author Osipov Stanislav (Stan's Assets) 
// @support support@stansassets.com
// @website https://stansassets.com
//
////////////////////////////////////////////////////////////////////////////////

using UnityEngine;

using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;


public static class SA_UnityExtensions  {

	//--------------------------------------
	// GameObject
	//--------------------------------------


	public static void MoveTo(this GameObject go, Vector3 position, float time, SA.Common.Animation.EaseType easeType = SA.Common.Animation.EaseType.linear, System.Action OnCompleteAction = null ) {
		SA.Common.Animation.ValuesTween tw = go.AddComponent<SA.Common.Animation.ValuesTween>();

		tw.DestoryGameObjectOnComplete = false;
		tw.VectorTo(go.transform.position, position, time, easeType);

		tw.OnComplete += OnCompleteAction;
	}


	public static void ScaleTo(this GameObject go, Vector3 scale, float time, SA.Common.Animation.EaseType easeType = SA.Common.Animation.EaseType.linear, System.Action OnCompleteAction = null ) {
		SA.Common.Animation.ValuesTween tw = go.AddComponent<SA.Common.Animation.ValuesTween>();

		tw.DestoryGameObjectOnComplete = false;
		tw.ScaleTo(go.transform.localScale, scale, time, easeType);

		tw.OnComplete += OnCompleteAction;
	}

	public static Bounds GetRealBounds (this GameObject go) {
		var renderers = go.GetComponentsInChildren<Renderer>(false);
		var combinedBounds = new Bounds(go.transform.position, Vector3.zero);

		foreach (var render in renderers) {
			if (render.bounds.size != Vector3.zero) {
				combinedBounds.Encapsulate(render.bounds);
			}
		}

		return combinedBounds;
	}

	public static Bounds GetRealBounds (this Component go) {
		var renderers = go.GetComponentsInChildren<Renderer>(false);
		var combinedBounds = new Bounds(go.transform.position, Vector3.zero);

		foreach (var render in renderers) {
			if (render.bounds.size != Vector3.zero) {
				combinedBounds.Encapsulate(render.bounds);
			}
		}

		return combinedBounds;
	}


	public static Bounds GetRendererBounds(this GameObject go) {
		return CalculateBounds(go);
	}


	public static Vector3 GetVertex(this GameObject go, VertexX x, VertexY y, VertexZ z) {

		Bounds bounds = go.GetRendererBounds ();
		return bounds.GetVertex (x, y, z);
	}



	public static void Reset(this GameObject go) {
		go.transform.Reset ();
	}

	//--------------------------------------
	// Renderer
	//--------------------------------------

	public static bool IsVisibleFrom(this Renderer renderer, Camera camera) {
		Plane[] planes = GeometryUtility.CalculateFrustumPlanes(camera);
		return GeometryUtility.TestPlanesAABB(planes, renderer.bounds);
	}

	//--------------------------------------
	// Transform
	//--------------------------------------

	public static void Reset(this Transform t) {
		t.localScale = Vector3.one;
		t.localPosition = Vector3.zero;
		t.localRotation = Quaternion.identity;
	}


	public static Bounds GetRendererBounds(this Transform t) {
		return t.gameObject.GetRendererBounds ();
	}


	public static Vector3 GetVertex(this Transform t, VertexX x, VertexY y, VertexZ z) {
		return t.gameObject.GetVertex (x, y, z);
	}

	public static Transform Clear(this Transform transform) {

		if(transform.childCount == 0) {
			return transform;
		}

		Transform[] children = transform.GetComponentsInChildren<Transform> ();

		foreach (Transform child in children) {
			if(child != transform && child != null) {
				GameObject.DestroyImmediate(child.gameObject);
			}
		}
		return transform;
	}


	//--------------------------------------
	// Bounds
	//--------------------------------------

	public static Vector3 GetVertex(this Bounds bounds, VertexX x, VertexY y, VertexZ z) {


		Vector3 center = bounds.center;

		switch(x){
		case VertexX.Right:
			center.x -= bounds.extents.x;
			break;
		case VertexX.Left:
			center.x += bounds.extents.x;
			break;
		}


		switch(y) {
		case VertexY.Bottom:
			center.y -= bounds.extents.y;
			break;

		case VertexY.Top:
			center.y += bounds.extents.y;
			break;
		}

		switch(z) {
		case VertexZ.Back:
			center.z -= bounds.extents.z;
			break;

		case VertexZ.Front:
			center.z += bounds.extents.z;
			break;
		}

		return center;
	}


	//--------------------------------------
	// Material
	//--------------------------------------

	public static void SetAlpha (this Material material, float value) {
		if(material.HasProperty("_Color")) {
			Color color = material.color;
			color.a = value;
			material.color = color;
		}
	}

	//--------------------------------------
	// Texture2D
	//--------------------------------------

	public static Sprite ToSprite(this Texture2D texture) {
		return Sprite.Create(texture, new Rect(0.0f, 0.0f, texture.width, texture.height), new Vector2(0.5f, 0.5f)); 
	}


		
	public static Texture2D Rotate (this Texture2D texture, float angle){
		return SA.Common.Util.IconManager.Rotate (texture, angle);
	}



	//--------------------------------------
	// Vector3
	//--------------------------------------

	public static Vector3 Reset(this Vector3 vec) {
		return Vector3.zero;
	}


	public static Vector3 ResetXCoord(this Vector3 vec) {
		Vector3 newVec = vec;
		newVec.x = 0f;

		return newVec;
	}

	public static Vector3 ResetYCoord(this Vector3 vec) {
		Vector3 newVec = vec;
		newVec.y = 0f;

		return newVec;
	}

	public static Vector3 ResetZCoord(this Vector3 vec) {
		Vector3 newVec = vec;
		newVec.z = 0f;

		return newVec;
	}

	public static Vector3 Average(this Vector3[] f) {
		var temp = Vector3.zero;
		var validValues = 0;

		for (int i = 0; i < f.Length - 1; i++)
		{
			// If a value is 0.0f it means we didn't collect that sample yet, therefore, we'll ignore it in our average
			if (f[i] != Vector3.zero)
			{
				temp += f[i];
				validValues++;
			}
		}

		// Prevent divisions by 0, and as such, the destruction of the world
		validValues = (validValues > 0) ? validValues : 1;

		temp.x = temp.x / validValues;
		temp.y = temp.y / validValues;
		temp.z = temp.z / validValues;

		return temp;

	}


	//--------------------------------------
	// String
	//--------------------------------------


	public static string GetLast(this string source, int count) {
		if(count >= source.Length)
			return source;
		return source.Substring(source.Length - count);
	}

	public static string GetFirst(this string source, int count) {


		if(count >= source.Length)
			return source;
		return source.Substring (0, count);
	}

	public static void CopyToClipboard(this string source) {
#if UNITY_2017
		TextEditor te = new TextEditor();
		te.text = source;
		te.SelectAll();
		te.Copy();
#endif
	}


	public static System.Uri CovertToURI(this string source) {
		return new System.Uri (source);
	}



	//--------------------------------------
	// Uri
	//--------------------------------------

	private static readonly Regex _regex = new Regex(@"[?|&]([\w\.]+)=([^?|^&]+)");
	public static Dictionary<string, string> ParseQueryString(this Uri uri) {
		var match = _regex.Match(uri.PathAndQuery);
		var paramaters = new Dictionary<string, string>();
		while (match.Success) {
			paramaters.Add(match.Groups[1].Value, match.Groups[2].Value);
			match = match.NextMatch();
		}
		return paramaters;
	}



	//--------------------------------------
	// Public Methods
	//--------------------------------------


	public static Bounds CalculateBounds(GameObject obj) {

		bool hasBounds = false;
		Bounds Bounds = new Bounds(Vector3.zero, Vector3.zero);
		Renderer[] ChildrenRenderer = obj.GetComponentsInChildren<Renderer>();

		//Quaternion oldRotation = obj.transform.rotation;
		//obj.transform.rotation = Quaternion.identity;

		Renderer rnd = obj.GetComponent<Renderer> ();
		if(rnd != null) {
			Bounds = rnd.bounds;
			hasBounds = true;
		}

		foreach(Renderer child in ChildrenRenderer) {

			if(!hasBounds) {
				Bounds = child.bounds;
				hasBounds = true;
			} else {
				Bounds.Encapsulate(child.bounds);
			}
		}

		//obj.transform.rotation = oldRotation;

		return Bounds;
	}



}
