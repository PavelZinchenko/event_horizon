using UnityEngine;
using System.Collections.Generic;
using System.Linq;
using Session;
using GameModel;
using Zenject;

public class StarBorder : MonoBehaviour
{
    [Inject] private readonly ISessionData _session;
    [Inject] private readonly RegionMap _regionMap;

	public float Scale = 8;
	public float Thickness = 2.0f;
	public int Segments = 16;
	public Color Color = Color.white;
	public Texture2D Texture;	

	private void OnStateChanged(Star.State state)
	{
		var renderer = gameObject.GetComponent<Renderer>();
		if (renderer != null)
			renderer.enabled = Star.State.Normal == state || Star.State.Map == state;
	}

	public void Create(Galaxy.Star star, Color color)
	{
		transform.localScale = Vector3.one*Scale;
		CreateMesh(star, color*Color);
	}

	private void CreateMesh(Galaxy.Star star, Color color)
	{
		var mesh = gameObject.GetMesh();
		mesh.Clear(false);

		var regionId = star.Region.Id;
		if (regionId == Region.UnoccupiedRegionId || star.Region.IsCaptured)
			return;

		List<Vector3> vertices = new List<Vector3>();
		List<Vector2> uv = new List<Vector2>();
		List<int> triangles = new List<int>();

		var center = star.Position;
		var stars = StarLayout.GetAdjacentStars(star.Id).ToArray();
		var regions = stars.Select(item => _regionMap.GetStarRegion(item).Id).ToArray();

		var count = stars.Length;
	    var seed = _session.Game.Seed;

		for (int i = 0; i < count; ++i)
		{
			var id0 = (i - 1 + count) % count;
			var id1 = i;
			var id2 = (i + 1) % count;

			var star0 = stars[id0];
			var star1 = stars[id1];
			var star2 = stars[id2];

			if (regions[id1] == regionId)
				continue;

			var pos0 = StarLayout.GetStarPosition(star0, seed);
			var pos1 = StarLayout.GetStarPosition(star1, seed);
			var pos2 = StarLayout.GetStarPosition(star2, seed);

			var p0 = regions[id0] != regionId ? (pos0 - center)/2 : (pos0 + pos1)/2 - center;
			var p1 = (StarLayout.GetStarPosition(star1, seed) - center)/2;
			var p2 = regions[id2] != regionId ? (pos2 - center)/2 : (pos1 + pos2)/2 - center;

			CreateSegment(vertices, uv, triangles, Vector2.Lerp(p0,p1,0.5f), p1, Vector2.Lerp(p1,p2,0.5f));
		}

		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uv.ToArray();
		
		gameObject.CreateDefaultMaterial(Texture, color);
	}

	private void CreateSegment(List<Vector3> vertices, List<Vector2> uv, List<int> triangles, Vector2 p1, Vector2 p2, Vector2 p3)
	{
		int startIndex = vertices.Count;		
		int uvStartIndex = uv.Count;

		var dir = (p2-p1).normalized * Thickness;
		var outer1 = p1;
		var inner1 = p1 + new Vector2(-dir.y, dir.x);

		vertices.Add(outer1);
		vertices.Add(inner1);
		uv.Add(new Vector2(0, 1));
		uv.Add(new Vector2(0, 0));

		dir = (p3-p2).normalized * Thickness;
		var outer3 = p3;
		var inner3 = p3 + new Vector2(-dir.y, dir.x);

		var length = 0f;

		for (int i = 1; i < Segments; ++i)
		{
			var outer2 = Geometry.Bezier(p1, p2, p3, (float)i / Segments);
			dir = Geometry.BezierTangent(p1, p2, p3, (float)i / Segments).normalized * Thickness;
			var inner2 = outer2 + new Vector2(-dir.y, dir.x);

			length += Vector2.Distance(outer2, outer1);

			outer1 = outer2;
			if (Geometry.IsCounterClockwise(inner2,inner1,outer1) && Geometry.IsCounterClockwise(outer3, inner3, inner2))
				inner1 = inner2;

			vertices.Add(outer1);
			vertices.Add(inner1);

			uv.Add(new Vector2(length, 1));
			uv.Add(new Vector2(length, 0));
		}

		vertices.Add(outer3);
		vertices.Add(inner3);
		length += Vector2.Distance(outer1, outer3);
		uv.Add(new Vector2(length, 1));
		uv.Add(new Vector2(length, 0));

		var scale = Mathf.Round(length/Thickness) / length;
		for (int i = 0; i < Segments*2; ++i)
		{
			var index = uvStartIndex + i + 2;
			var p = uv[index];
			p.x *= scale;
			uv[index] = p;
		}

		for (int i = 0; i < Segments; ++i)
		{
			var v1 = startIndex + i*2;
			var v2 = startIndex + i*2 + 1;
			var v3 = startIndex + i*2 + 2;
			var v4 = startIndex + i*2 + 3;
			triangles.Add(v1);
			triangles.Add(v3);
			triangles.Add(v4);
			triangles.Add(v4);
			triangles.Add(v2);
			triangles.Add(v1);
		}
	}
}
