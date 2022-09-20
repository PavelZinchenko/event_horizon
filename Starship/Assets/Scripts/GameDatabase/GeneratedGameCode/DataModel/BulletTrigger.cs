//-------------------------------------------------------------------------------
//                                                                               
//    This code was automatically generated.                                     
//    Changes to this file may cause incorrect behavior and will be lost if      
//    the code is regenerated.                                                   
//                                                                               
//-------------------------------------------------------------------------------

using System.Linq;
using GameDatabase.Enums;
using GameDatabase.Serializable;
using GameDatabase.Model;

namespace GameDatabase.DataModel
{
	public abstract partial class BulletTrigger
	{
		partial void OnDataDeserialized(BulletTriggerSerializable serializable, Database.Loader loader);

		public static BulletTrigger Create(BulletTriggerSerializable serializable, Database.Loader loader)
		{
			switch (serializable.EffectType)
		    {
				case BulletEffectType.None:
					return new BulletTrigger_None(serializable, loader);
				case BulletEffectType.PlaySfx:
					return new BulletTrigger_PlaySfx(serializable, loader);
				case BulletEffectType.SpawnBullet:
					return new BulletTrigger_SpawnBullet(serializable, loader);
				case BulletEffectType.Detonate:
					return new BulletTrigger_Detonate(serializable, loader);
				default:
                    throw new DatabaseException("BulletTrigger: Invalid content type - " + serializable.EffectType);
			}
		}

		public abstract T Create<T>(IBulletTriggerFactory<T> factory);

		protected BulletTrigger(BulletTriggerSerializable serializable, Database.Loader loader)
		{
			Condition = serializable.Condition;
			EffectType = serializable.EffectType;

			OnDataDeserialized(serializable, loader);
		}

		public BulletTriggerCondition Condition { get; private set; }
		public BulletEffectType EffectType { get; private set; }

		public static BulletTrigger DefaultValue { get; private set; }
	}

	public interface IBulletTriggerFactory<T>
    {
	    T Create(BulletTrigger_None content);
	    T Create(BulletTrigger_PlaySfx content);
	    T Create(BulletTrigger_SpawnBullet content);
	    T Create(BulletTrigger_Detonate content);
    }

    public partial class BulletTrigger_None : BulletTrigger
    {
		partial void OnDataDeserialized(BulletTriggerSerializable serializable, Database.Loader loader);

  		public BulletTrigger_None(BulletTriggerSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(IBulletTriggerFactory<T> factory)
        {
            return factory.Create(this);
        }

    }
    public partial class BulletTrigger_PlaySfx : BulletTrigger
    {
		partial void OnDataDeserialized(BulletTriggerSerializable serializable, Database.Loader loader);

  		public BulletTrigger_PlaySfx(BulletTriggerSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			VisualEffect = loader.GetVisualEffect(new ItemId<VisualEffect>(serializable.VisualEffect));
			AudioClip = new AudioClipId(serializable.AudioClip);
			Color = new ColorData(serializable.Color);
			ColorMode = serializable.ColorMode;
			Size = UnityEngine.Mathf.Clamp(serializable.Size, 0f, 100f);
			Lifetime = UnityEngine.Mathf.Clamp(serializable.Lifetime, 0f, 1000f);

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(IBulletTriggerFactory<T> factory)
        {
            return factory.Create(this);
        }

		public VisualEffect VisualEffect { get; private set; }
		public AudioClipId AudioClip { get; private set; }
		public ColorData Color { get; private set; }
		public ColorMode ColorMode { get; private set; }
		public float Size { get; private set; }
		public float Lifetime { get; private set; }
    }
    public partial class BulletTrigger_SpawnBullet : BulletTrigger
    {
		partial void OnDataDeserialized(BulletTriggerSerializable serializable, Database.Loader loader);

  		public BulletTrigger_SpawnBullet(BulletTriggerSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			AudioClip = new AudioClipId(serializable.AudioClip);
			Ammunition = loader.GetAmmunition(new ItemId<Ammunition>(serializable.Ammunition));
			Color = new ColorData(serializable.Color);
			ColorMode = serializable.ColorMode;
			Quantity = UnityEngine.Mathf.Clamp(serializable.Quantity, 0, 1000);
			Size = UnityEngine.Mathf.Clamp(serializable.Size, 0f, 100f);
			Cooldown = UnityEngine.Mathf.Clamp(serializable.Cooldown, 0f, 1000f);
			RandomFactor = UnityEngine.Mathf.Clamp(serializable.RandomFactor, 0f, 3.402823E+38f);
			PowerMultiplier = UnityEngine.Mathf.Clamp(serializable.PowerMultiplier, 0f, 3.402823E+38f);
			MaxNestingLevel = UnityEngine.Mathf.Clamp(serializable.MaxNestingLevel, 0, 100);

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(IBulletTriggerFactory<T> factory)
        {
            return factory.Create(this);
        }

		public AudioClipId AudioClip { get; private set; }
		public Ammunition Ammunition { get; private set; }
		public ColorData Color { get; private set; }
		public ColorMode ColorMode { get; private set; }
		public int Quantity { get; private set; }
		public float Size { get; private set; }
		public float Cooldown { get; private set; }
		public float RandomFactor { get; private set; }
		public float PowerMultiplier { get; private set; }
		public int MaxNestingLevel { get; private set; }
    }
    public partial class BulletTrigger_Detonate : BulletTrigger
    {
		partial void OnDataDeserialized(BulletTriggerSerializable serializable, Database.Loader loader);

  		public BulletTrigger_Detonate(BulletTriggerSerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(IBulletTriggerFactory<T> factory)
        {
            return factory.Create(this);
        }

    }

}

