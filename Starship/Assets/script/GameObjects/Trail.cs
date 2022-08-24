//using UnityEngine;
//using System.Collections.Generic;
//using System.Linq;
//using CombatSystem.View;

//[RequireComponent(typeof(MeshRenderer))]
//[RequireComponent(typeof(MeshFilter))]
//public class Trail : BaseView
//{
//	public MeshRenderer Renderer;
//	public MeshFilter MeshFilter;
//	public int Segments = 10;
//	public float UpdateInterval = 0.1f;

//	protected override void UpdateColor()
//	{
//		Renderer.material.color = new Color(Color.r, Color.g, Color.b, Color.a);
//	}

//	protected override void UpdatePosition() {}
//	protected override void UpdateSize() {}

//	protected override void Awake()
//	{
//		_mesh = MeshFilter.mesh;
//		_mesh.MarkDynamic();
//    }

//	protected override void OnEnable()
//	{
//		InitMeshData();
//		_elapsedTime = 0;
//        _direction = RotationHelpers.Direction(Rotation);
//    }

//	protected override void Update()
//	{
//		base.Update();

//		//var viewPoint = SceneManager.Scene.ViewPoint;
//		var position = Position + 0.5f*RotationHelpers.Transform(Offset, Rotation);
//		//gameObject.Move(viewPoint.Direction(new Position(position.x, position.y)));
//		gameObject.Move(position);

//		_positionOffset = position - _lastPosition;
//		var length = _positionOffset.magnitude;
//		if (length > 10)
//		{
//			_positionOffset = Vector2.zero;
//			_lastPosition = Position;
//			_direction = RotationHelpers.Direction(Rotation);
//		}
//		if (length > 0.001f)
//		{
//			_direction = _positionOffset.normalized;
//		}
//		if (_direction == Vector2.zero)
//		{
//			_direction = RotationHelpers.Direction(Rotation);
//		}

//		{
//			var right = RotationHelpers.Transform(_direction, -90);
//			_vertices[0] = -Scale*right;
//			_vertices[1] = Scale*right;
//		}

//		_lastPosition = position;

//		var isVisible = Vector2.Dot(RotationHelpers.Direction(Rotation), _direction) > 0 && Opacity > 0.1f;

//		for (var i = 2; i < _vertices.Length; ++i)
//			_vertices[i] = _vertices[i] - (Vector3)_positionOffset;

//		_elapsedTime += Time.deltaTime;

//		if (_elapsedTime > UpdateInterval)
//		{
//			_elapsedTime -= UpdateInterval;

//			for (var i = _vertices.Length-1; i >= 2; --i)
//				_vertices[i] = _vertices[i-2];

//			_visibility.Dequeue();
//			_visibility.Enqueue(isVisible);

//			var right = RotationHelpers.Transform(_direction, -90);

//			_vertices[0] = -Scale*right;
//			_vertices[1] = Scale*right;
//		}

//		var t = 1.0f - _elapsedTime / UpdateInterval;
//		var index = _uv.Length-1;
//		foreach (var visible in _visibility)
//		{
//			var y = visible ? 1.0f - ((float)index + t) / _uv.Length : 0.0f;
//			_uv[index--] = new Vector2(1, y);
//			_uv[index--] = new Vector2(0, y);
//		}

//		_mesh.vertices = _vertices;
//		_mesh.uv = _uv;
//	}

//	private void InitMeshData()
//	{
//		_vertices = Enumerable.Repeat(Vector3.zero, Segments*2).ToArray();
//		_uv = new Vector2[_vertices.Length];
//		for (var i = 0; i < _uv.Length; ++i)
//		{
//			_uv[i] = new Vector2(i%2, 0);
//		}

//		_triangles = new List<int>(6*(Segments - 1));
//		for (var i = 0; i < Segments - 1; ++i)
//		{
//			_triangles.Add(i*2);
//			_triangles.Add(i*2+1);
//			_triangles.Add(i*2+3);
//			_triangles.Add(i*2+3);
//			_triangles.Add(i*2+2);
//			_triangles.Add(i*2);
//		}

//		_visibility = new Queue<bool>(Enumerable.Repeat(false, Segments));

//		_mesh.vertices = _vertices;
//		_mesh.triangles = _triangles.ToArray();
//		_mesh.uv = _uv.ToArray();
//	}

//	private Vector2 _lastPosition;
//	private Vector2 _positionOffset;
//	private Vector2 _direction;

//	private float _elapsedTime;

//	private Vector3[] _vertices;
//	private Vector2[] _uv;
//	private List<int> _triangles;
//	private Queue<bool> _visibility;

//	private Mesh _mesh;
//}
