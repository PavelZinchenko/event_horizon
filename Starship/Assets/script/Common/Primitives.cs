using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Primitives
{
	public static void CreateRectangle(Mesh mesh, float width, float height, float textureScale)
	{
		mesh.Clear(false);
		
		mesh.vertices = new Vector3[]
		{
			new Vector3 (-width/2, height/2, 0),
			new Vector3 (width/2, height/2, 0),
			new Vector3 (width/2, -height/2, 0),
			new Vector3 (-width/2, -height/2, 0)
		};
		mesh.triangles = new int[] { 0,1,2, 2,3,0 };

		var min = -(1/textureScale - 1)/2;
		var max = (1/textureScale + 1)/2;

		mesh.uv = new Vector2[]
		{
			new Vector2 (min, max),
			new Vector2 (max, max),
			new Vector2 (max, min),
			new Vector2 (min, min) 
		};
	}

    public static void CreatePlane(Mesh mesh, float width, float height, int subdivisions)
    {
        mesh.Clear(false);

        var vertices = new List<Vector3>();
        var uv = new List<Vector2>();
        var uv1 = new List<Vector2>();
        var triangles = new List<int>();

        for (var i = 0; i < subdivisions; ++i)
        {
            for (var j = 0; j < subdivisions; ++j)
            {
                var x0 = (float)j / subdivisions;
                var y0 = (float)i / subdivisions;
                var x1 = (1.0f + j) / subdivisions;
                var y1 = (1.0f + i) / subdivisions;

                var index = vertices.Count;

                vertices.Add(new Vector3(width * (x0 - 0.5f), height * (y1 - 0.5f), 0));
                vertices.Add(new Vector3(width * (x1 - 0.5f), height * (y1 - 0.5f), 0));
                vertices.Add(new Vector3(width * (x1 - 0.5f), height * (y0 - 0.5f), 0));
                vertices.Add(new Vector3(width * (x0 - 0.5f), height * (y0 - 0.5f), 0));

                uv.Add(new Vector2(x0, y1));
                uv.Add(new Vector2(x1, y1));
                uv.Add(new Vector2(x1, y0));
                uv.Add(new Vector2(x0, y0));

                triangles.Add(index);
                triangles.Add(index + 1);
                triangles.Add(index + 2);
                triangles.Add(index + 2);
                triangles.Add(index + 3);
                triangles.Add(index);
            }
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();
        mesh.RecalculateNormals();
    }

    public static void CreateSphere(Mesh mesh, float radius, int size, float noiseFrequency, float noisePower)
	{
		mesh.Clear(false);

		var vertices = new List<Vector3>();
		var triangles = new List<int>();
		var uv = new List<Vector2>();

		float step = 2.0f/size;
		var offset = new Vector3(Random.Range(-100f, 100f), Random.Range(-100f, 100f), Random.Range(-100f, 100f));
		
		for (int k = 0; k < 6; ++k)
		{
			float y = -1;
			for (int i = 0; i <= size; ++i)
			{
				float x = -1;
				for (int j = 0; j <= size; ++j)
				{
					Vector3 point = Vector3.zero;
					switch(k)
					{
					case 0:
						point = new Vector3(x,y,-1);
						break;
					case 1:
						point = new Vector3(1,y,x);
						break;
					case 2:
						point = new Vector3(-x,y,1);
						break;
					case 3:
						point = new Vector3(-1,y,-x);
						break;
					case 4:
						point = new Vector3(-x,-1,y);
						break;
					case 5:
						point = new Vector3(x,1,y);
						break;
					}
					
					point = point.normalized*radius;
					var temp = offset + point*radius*noiseFrequency;
					var scale = 1 + noisePower*(Mathf.PerlinNoise(temp.x, temp.y) + Mathf.PerlinNoise(temp.y, temp.z) - 2*Mathf.PerlinNoise(temp.z, temp.x));					
					vertices.Add(point*scale);
					
					uv.Add(new Vector2(j/(float)size,i/(float)size));
					x += step;
				}
				y += step;
			}
			
			int start = k*(size+1)*(size+1);
			for (int i = 0; i < size; ++i)
			{
				for (int j = 0; j < size; ++j)
				{
					triangles.Add(start + j);
					triangles.Add(start + size+1 + j);
					triangles.Add(start + size+1 + j+1);
					triangles.Add(start + size+1 + j+1);
					triangles.Add(start + j+1);
					triangles.Add(start + j);
				}
				start += size+1;
			}
		}

		mesh.vertices = vertices.ToArray();
		mesh.triangles = triangles.ToArray();
		mesh.uv = uv.ToArray();
		mesh.normals = vertices.Select(item => item.normalized).ToArray();
		//mesh.RecalculateNormals();
	}

    public static void CreateLine(Mesh mesh, float length, float thickness, float borderSize)
    {
        mesh.Clear(false);

        if (length <= 2 * borderSize)
        {
            mesh.vertices = new Vector3[]
            {
                new Vector3 (0, thickness/2, 0),
                new Vector3 (0, -thickness/2, 0),
                new Vector3 (length/2, thickness/2, 0),
                new Vector3 (length/2, -thickness/2, 0),
                new Vector3 (length/2, thickness/2, 0),
                new Vector3 (length/2, -thickness/2, 0),
                new Vector3 (length, thickness/2, 0),
                new Vector3 (length, -thickness/2, 0),
            };
            mesh.triangles = new int[] { 0, 1, 3, 3, 2, 0, 4, 5, 7, 7, 6, 4 };
            mesh.uv = new Vector2[]
            {
                new Vector2 (0, 1),
                new Vector2 (0, 0),
                new Vector2 (length/2, 1),
                new Vector2 (length/2, 0),
                new Vector2 (1-length/2, 1),
                new Vector2 (1-length/2, 0),
                new Vector2 (1, 1),
                new Vector2 (1, 0),
            };
        }
        else
        {
            mesh.vertices = new Vector3[]
            {
                new Vector3 (0, thickness/2, 0),
                new Vector3 (0, -thickness/2, 0),
                new Vector3 (borderSize, thickness/2, 0),
                new Vector3 (borderSize, -thickness/2, 0),
                new Vector3 (length - borderSize, thickness/2, 0),
                new Vector3 (length - borderSize, -thickness/2, 0),
                new Vector3 (length, thickness/2, 0),
                new Vector3 (length, -thickness/2, 0),
            };
            mesh.triangles = new int[] { 0, 1, 3, 3, 2, 0, 2, 3, 5, 5, 4, 2, 4, 5, 7, 7, 6, 4 };
            mesh.uv = new Vector2[]
            {
                new Vector2 (0, 1),
                new Vector2 (0, 0),
                new Vector2 (borderSize, 1),
                new Vector2 (borderSize, 0),
                new Vector2 (1-borderSize, 1),
                new Vector2 (1-borderSize, 0),
                new Vector2 (1, 1),
                new Vector2 (1, 0),
            };
        }
    }

    public static void CreateCircle(Mesh mesh, float radius, float thickness, int segments, float textureAspect = 1.0f)
    {
        mesh.Clear(false);

        var vertices = new List<Vector3>();
        var uv = new List<Vector2>();
        var triangles = new List<int>();

        var length = 2 * Mathf.PI * radius;
        var step = Mathf.Round(textureAspect * length / thickness) / segments;

        for (var i = 0; i <= segments; ++i)
        {
            var alpha = i * 2 * Mathf.PI / segments;
            var x = Mathf.Cos(alpha);
            var y = Mathf.Sin(alpha);
            vertices.Add(new Vector3(x, y, 0));
            x = (1.0f - thickness / radius) * Mathf.Cos(alpha);
            y = (1.0f - thickness / radius) * Mathf.Sin(alpha);
            vertices.Add(new Vector3(x, y, 0));
            uv.Add(new Vector2(i * step, 1));
            uv.Add(new Vector2(i * step, 0));
        }

        for (var i = 0; i < segments; ++i)
        {
            var p1 = i * 2;
            var p2 = i * 2 + 1;
            var p3 = i * 2 + 2;
            var p4 = i * 2 + 3;
            triangles.Add(p2);
            triangles.Add(p1);
            triangles.Add(p3);
            triangles.Add(p3);
            triangles.Add(p4);
            triangles.Add(p2);
        }

        mesh.vertices = vertices.ToArray();
        mesh.triangles = triangles.ToArray();
        mesh.uv = uv.ToArray();
    }
}
