//using UnityEngine;
//using System;
//using System.Linq;
//using System.Collections.Generic;

//public class BackgroundManager : MonoBehaviour
//{
//	public int GameTextureSize;
//	public int GameDetailTextureSize;
//	public int CombatTextureSize;
//	public int CombatDetailTextureSize;

//	public BackgroundManager()
//	{
//		_self = new WeakReference<BackgroundManager>(this);
//	}
	
//	public static BackgroundManager Instance
//	{
//		get { return _self != null ? _self.Target : null; }
//	}

//	public Texture2D GameBackground
//	{
//		get 
//		{
//			while (!_textures.ContainsKey(Builder.BackgroundType.Game))
//			{
//				_builder.WaitTaskDone();
//				Update();
//			}

//			return _textures[Builder.BackgroundType.Game]; 
//		}
//	}

//	public Texture2D GameDetailBackground
//	{
//		get 
//		{
//			while (!_textures.ContainsKey(Builder.BackgroundType.GameDetail))
//			{
//				_builder.WaitTaskDone();
//				Update();
//			}
			
//			return _textures[Builder.BackgroundType.GameDetail]; 
//		}
//	}
	
//	public Texture2D CombatBackground
//	{
//		get
//		{
//			while (!_textures.ContainsKey(Builder.BackgroundType.Combat))
//			{
//				_builder.WaitTaskDone();
//				Update();
//			}

//			_builder.Create(Builder.BackgroundType.Combat);
//			return _textures[Builder.BackgroundType.Combat];
//		}
//	}

//	public Texture2D CombatDetailBackground
//	{
//		get
//		{
//			while (!_textures.ContainsKey(Builder.BackgroundType.CombatDetail))
//			{
//				_builder.WaitTaskDone();
//				Update();
//			}

//			_builder.Create(Builder.BackgroundType.CombatDetail);
//			return _textures[Builder.BackgroundType.CombatDetail];
//		}
//	}

//	public void Destroy(Texture2D texture)
//	{
//		if (!_textures.ContainsValue(texture))
//		{
//			GameObject.Destroy(texture);
//		}
//	}

//	private void Awake()
//	{
//		_builder = new Builder(this, GameTextureSize, GameDetailTextureSize, CombatTextureSize, CombatDetailTextureSize);

//		foreach (var name in Enum.GetValues(typeof(Builder.BackgroundType)).OfType<Builder.BackgroundType>())
//		{
//			_builder.Create(name);
//		}

//		/*foreach (var name in Enum.GetValues(typeof(Builder.BackgroundType)).OfType<Builder.BackgroundType>())
//		{
//			var texture = new Texture2D(1,1);
//			try
//			{
//				var data = System.IO.File.ReadAllBytes(Application.persistentDataPath + "/" + name);
//				texture.LoadImage(data);
				
//			}
//			catch (System.Exception e)
//			{
//				UnityEngine.Debug.Log(e.Message);
//				_builder.Create(name);
//				continue;
//			}

//			_textures[name] = texture;
//		}*/
//	}

//	private void Update()
//	{
//		Builder.TaskResult result;
//		while (_builder.TryGetResult(out result))
//		{
//			UnityEngine.Debug.Log("BackgroundManager: texture generated for " + result.Type);
//			_textures[result.Type] = result.Builder.Get();
//		}
//	}

//	private void OnDestroy()
//	{
//		/*foreach (var texture in _textures)
//		{
//			if (texture.Value == null)
//				continue;

//			try
//			{
//				System.IO.File.WriteAllBytes(Application.persistentDataPath + "/" + texture.Key, texture.Value.EncodeToPNG());
//			}
//			catch (System.Exception e)
//			{
//				UnityEngine.Debug.Log(e.Message);
//				continue;
//			}
//		}*/
//	}

//	private Dictionary<Builder.BackgroundType, Texture2D> _textures = new Dictionary<Builder.BackgroundType, Texture2D>();
//	private Builder _builder;
//	private static WeakReference<BackgroundManager> _self;

//	private class Builder : BackgroundTask
//	{
//		public enum BackgroundType
//		{
//			Game,
//			GameDetail,
//			Combat,
//			CombatDetail,
//		}

//		public Builder(object owner, int gameTextureSize, int gameDetailTextureSize, int combatTextureSize, int combatDetailTextureSize)
//		{
//			_gameTextureSize = gameTextureSize;
//			_gameDetailTextureSize = gameDetailTextureSize;
//			_combatTextureSize = combatTextureSize;
//			_combatDetailTextureSize = combatDetailTextureSize;
//			_random = new System.Random();
//			StartTask(owner);
//		}

//		public void Create(BackgroundType type)
//		{
//			lock (_lockObject)
//			{
//				_tasks.Enqueue(type);
//			}
//		}

//		public bool TryGetResult(out TaskResult result)
//		{
//			lock (_lockObject)
//			{
//				if (_results.Count > 0)
//				{
//					result = _results.Dequeue();
//					return true;
//				}
//			}

//			result = new TaskResult();
//			return false;
//		}

//		public void WaitTaskDone()
//		{
//			_autoEvent.WaitOne();
//		}

//		protected override bool DoWork()
//		{
//			while (true)
//			{
//				BackgroundType task;
//				lock (_lockObject)
//				{
//					if (_tasks.Count == 0)
//						break;
//					task = _tasks.Dequeue();
//				}

//				switch (task)
//				{
//				case BackgroundType.Game:
//					CreateGameBackground();
//					break;
//				case BackgroundType.GameDetail:
//					CreateGameDetailBackground();
//					break;
//				case BackgroundType.Combat:
//					CreateCombatBackground();
//					break;
//				case BackgroundType.CombatDetail:
//					CreateCombatDetailBackground();
//					break;
//				}

//				_autoEvent.Set();
//			}

//			_autoEvent.Set();
//			return false;
//		}

//		protected override void OnIdle()
//		{
//			System.Threading.Thread.Sleep(500);
//		}

//		private void CreateGameBackground()
//		{
//			var builder = new BackgroundBuilder(_gameTextureSize, false, _random.Next());
			
//			builder.NebulaDetail = 32;
//			builder.NebulaSmooth = 20;
//			builder.NebulaContrast = 0.8f;
//			builder.NebulaColor.Clear();
//			builder.NebulaColor[0] = new Color(0.0f, 0.0f, 0.0f);
//			builder.NebulaColor[1] = new Color(0.1f, 0.2f, 0.4f);
//            builder.AddNebula();
			
//			lock (_lockObject)
//			{
//				_results.Enqueue(new TaskResult(BackgroundType.Game, builder));
//			}
//		}

//		private void CreateGameDetailBackground()
//		{
//			var builder = new BackgroundBuilder(_gameDetailTextureSize, true, _random.Next());

//			builder.NumberOfStars = 250;
//			builder.StarsGlowSize = 14;
//			builder.StarsDimness = 2;
//			builder.AddStars();
			
//			builder.NumberOfStars = 1500;
//			builder.StarsGlowSize = 2;
//			builder.AddStars();

//			lock (_lockObject)
//			{
//				_results.Enqueue(new TaskResult(BackgroundType.GameDetail, builder));
//			}
//		}

//		private void CreateCombatBackground()
//		{
//			var builder = new BackgroundBuilder(_combatTextureSize, false, _random.Next()) 
//			{
//				NumberOfStars = 2000,
//				StarsGlowSize = 8,
//				StarsDimness = 6,
//			};
//			builder.AddNebula();
//			builder.AddStars();

//			lock (_lockObject)
//			{
//				_results.Enqueue(new TaskResult(BackgroundType.Combat, builder));
//			}
//		}
		
//		private void CreateCombatDetailBackground()
//		{
//			var builder = new BackgroundBuilder(_combatDetailTextureSize, true, _random.Next()) 
//			{
//				NumberOfStars = 500,
//				StarsGlowSize = 3,
//            };
//			builder.AddStars();

//			lock (_lockObject)
//			{
//				_results.Enqueue(new TaskResult(BackgroundType.CombatDetail, builder));
//			}
//		}
		
//		private Queue<BackgroundType> _tasks = new Queue<BackgroundType>();
//		private Queue<TaskResult> _results = new Queue<TaskResult>();
//		private object _lockObject = new object();
//		private System.Threading.AutoResetEvent _autoEvent = new System.Threading.AutoResetEvent(false);
//		private readonly int _gameTextureSize;
//		private readonly int _gameDetailTextureSize;
//		private readonly int _combatTextureSize;
//		private readonly int _combatDetailTextureSize;
//		private readonly System.Random _random;

//		public struct TaskResult
//		{
//			public TaskResult(BackgroundType type, BackgroundBuilder builder)
//			{
//				Type = type;
//				Builder = builder;
//			}

//			public readonly BackgroundType Type;
//			public readonly BackgroundBuilder Builder;
//		}
//	}
//}
