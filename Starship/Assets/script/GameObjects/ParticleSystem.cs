//using UnityEngine;

//namespace Effects
//{
//	public class ParticleSystem : BaseObject
//	{
//		public override void Awake()
//		{
//			_particleSystem = GetComponent<UnityEngine.ParticleSystem>();
//			_baseParticleSize = _particleSystem.startSize;
//			_baseParticleSpeed = _particleSystem.startSpeed;
//			base.Awake();
//		}

//		public override void OnEnable()
//		{
//			Update();
//			_particleSystem.startSize = _baseParticleSize*Scale;
//			_particleSystem.startSpeed = _baseParticleSpeed*Scale;
//		    _particleSystem.startColor = Color;
//		    _particleSystem.startRotation = Rotation;
//			_particleSystem.Play();
//		}

//		public override void OnDisable()
//		{
//			_particleSystem.Stop();
//		}

//		public override void Update()
//		{
//            base.Update();

//			if (!DEBUG)
//			{
//				var particleSystemLifeTime = 1.0f - _particleSystem.time/_particleSystem.duration;
//				_particleSystem.playbackSpeed = Mathf.Max(0, 1.0f + (particleSystemLifeTime - Lifetime)*10);
//			}
//		}

//		private float _baseParticleSize;
//		private float _baseParticleSpeed;
//		private UnityEngine.ParticleSystem _particleSystem;
//	}
//}

