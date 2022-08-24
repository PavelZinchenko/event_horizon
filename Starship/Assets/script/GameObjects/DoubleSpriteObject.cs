//using UnityEngine;
//using CombatSystem.View;

//public sealed class DoubleSpriteObject : SpriteView, IGameObjectInterface
//{
//    [SerializeField] private SpriteView _secondSprite;

//    [SerializeField] private bool _showSmoothly = false;

//    public override float Rotation
//    {
//        get { return base.Rotation; }
//        set
//        {
//            base.Rotation = value;
//            Background.Rotation = 45f - value*2;
//        }
//    }

//    public override Color Color
//    {
//        get { return base.Color; }
//        set
//        {
//            base.Color = value;
//            Background.Color = value;
//        }
//    }

//    public float Lifetime
//    {
//        get
//        {
//            return _lifetime;
//        }
//        set
//        {
//            _lifetime = value;
//            var opacity = _showSmoothly ? 1f - Mathf.Pow(Mathf.Abs(_lifetime - 0.5f)*2f, 4) :  1f - Mathf.Pow(1f - value, 4);
//            base.Opacity = opacity;
//            Background.Opacity = opacity;
//        }
//    }


//    public float Power { get; set; }

//    public CombatSystem.Collision.ColliderType ObjectColliderType { get { return base.ColliderType; } }
//    public CombatSystem.Collision.Map GetCollisionMap() { return base.CreateCollisionMap(); }

//    private SpriteView Background { get { return _secondSprite; } }

//    protected override void OnEnable()
//    {
//        Update();
//    }

//    private float _lifetime = 1.0f;
//}
