//using UnityEngine;
//using CombatSystem.Collision;

//public class BaseObject : MonoBehaviour, IGameObjectInterface
//{
//	public enum ColliderType
//	{
//		Map,
//		Empty,
//		Circle,
//        Line,
//	}
	
//	public bool Active = false;
//	public bool DEBUG = false;
//	public ColliderType Collider = ColliderType.Map;

//	public virtual Vector2 Position { get; set; }
//	public virtual float Rotation { get; set; }
//	public virtual float Scale { get; set; }
//	public virtual Vector2 Offset { get; set; }
//	public virtual Color Color { get; set; }

//	public virtual float Power { get; set; }
//	public virtual float Lifetime { get; set; }

//	public CombatSystem.Collision.ColliderType ObjectColliderType
//	{
//		get
//		{
//			switch (Collider)
//			{
//			case ColliderType.Circle:
//				return CombatSystem.Collision.ColliderType.Circle;
//			case ColliderType.Empty:
//				return CombatSystem.Collision.ColliderType.Empty;
//            case ColliderType.Line:
//                return CombatSystem.Collision.ColliderType.Vector;
//            default:
//				return CombatSystem.Collision.ColliderType.Map;
//			}
//		}
//	}

//	public virtual Map GetCollisionMap()
//	{
//		if (Collider == ColliderType.Map)
//		{
//			var renderer = gameObject.GetComponent<Renderer>();
//			if (renderer == null) return null;
//			var map = ResourceManager.Instance.GetCollisionMap(renderer.material.mainTexture as Texture2D);
//			//renderer.material.mainTexture = map.GetTexture();
//			return map;
//		}
//		return null;
//	}

//	public virtual void Awake()
//	{
//		//print("awake " + gameObject.name);
        
//		if (Active || DEBUG)
//		{
//			Lifetime = 1.0f;
//		}
//		else if (transform.parent == null)
//		{
//			gameObject.SetActive(false);
//		}

//		Position = transform.localPosition;
//		Rotation = transform.localEulerAngles.z;
//		Scale = transform.localScale.z;
//    }
    
//    public virtual void Start()
//	{
//		//print("start " + gameObject.name);
//	}

//	public virtual void OnEnable()
//	{
//		//print("enabled " + gameObject.name);
//		Update();
//	}

//	public virtual void OnDisable()
//	{
//		//print("disabled " + gameObject.name);
//    }

//	public virtual void OnDestroy()
//	{
//		gameObject.Cleanup();
//	}
    
//    public virtual void Update()
//	{
//		gameObject.Move(Position);
//		transform.localEulerAngles = new Vector3(0, 0, Rotation);
//		transform.localScale = Vector3.one*Scale;

//		if (DEBUG)
//		{
//			Lifetime -= Time.deltaTime;
//			if (Lifetime < 0) Lifetime = 1;
//		}
//	}
//}
