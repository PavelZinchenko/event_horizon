//using UnityEngine;

//public class ChaoticObjectBehaviour : MonoBehaviour
//{
//	public float Scale = 1.0f;

//	private void Awake()
//	{
//		OptimizedDebug.Log("awake");
//		_object = gameObject.GameObjectInterface();
//		_parent = transform.parent.gameObject.GameObjectInterface();
//	}

//	private void Start()
//	{
//		OptimizedDebug.Log("start");
//		gameObject.SetActive(true);
//		_object.Lifetime = 1;
//	}

//	private void Update()
//	{
//		_object.Color = _parent.Color;
//		_object.Rotation = Random.Range(0,360);
//		_object.Scale = Scale;//*Random.value;
//	}

//	private IGameObjectInterface _parent;
//	private IGameObjectInterface _object;
//}
