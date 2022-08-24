//using CombatSystem.View;
//using UnityEngine;

//namespace Assets.script.GameObjects
//{
//    public sealed class DoubleParticleObject : SpriteView, IGameObjectInterface
//    {
//        [SerializeField] private SpriteView _secondSprite;

//        public float Lifetime
//        {
//            get
//            {
//                return _lifetime;
//            }
//            set
//            {
//                _lifetime = value;
//                var opacity = 1f - Mathf.Pow(1f - value, 4);
//                base.Opacity = opacity;
//                _secondSprite.Opacity = opacity;
//            }
//        }

//        public override Color Color
//        {
//            get { return base.Color; }
//            set
//            {
//                base.Color = value;
//                _secondSprite.Color = value;
//            }
//        }

//        protected override void Update()
//        {
//            _imageRotation += _rotationSpeed*Time.deltaTime;
//            var temp1 = Scale;
//            var temp2 = Rotation;
//            Rotation += _imageRotation;
//            Scale *= (1.5f - Lifetime);
//            _secondSprite.Rotation = 45f - 2 * Rotation;

//            base.Update();

//            Scale = temp1;
//            Rotation = temp2;
//        }

//        public float Power { get; set; }

//        public CombatSystem.Collision.ColliderType ObjectColliderType { get { return base.ColliderType; } }
//        public CombatSystem.Collision.Map GetCollisionMap() { return base.CreateCollisionMap(); }

//        protected override void OnEnable()
//        {
//            var random = new System.Random(GetHashCode());
//            _imageRotation = random.Next(360);
//            _rotationSpeed = random.Next(-180, 180);
//            Update();
//        }

//        private float _imageRotation;
//        private float _rotationSpeed;
//        private float _lifetime = 1.0f;
//    }
//}
