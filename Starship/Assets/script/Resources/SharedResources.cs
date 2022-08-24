using UnityEngine;
using System.Linq;
using System.Collections.Generic;
using System.Threading;

public class SharedResources : MonoBehaviour 
{
	public SharedResources()
	{
		_self = new WeakReference<SharedResources>(this);
	}
	
	public static SharedResources Instance
	{
		get { return _self != null ? _self.Target : null; }
	}
	
	public Mesh SquareMesh { get { return _squareMesh; } }

	private void Awake()
	{
		Primitives.CreateRectangle(_squareMesh = new Mesh(), 1, 1, 1);
	}

	private Mesh _squareMesh;
	private static WeakReference<SharedResources> _self;
}
