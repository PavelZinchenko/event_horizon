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
	public abstract partial class Technology
	{
		partial void OnDataDeserialized(TechnologySerializable serializable, Database.Loader loader);

		public static Technology Create(TechnologySerializable serializable, Database.Loader loader)
		{
			switch (serializable.Type)
		    {
				case TechType.Component:
					return new Technology_Component(serializable, loader);
				case TechType.Ship:
					return new Technology_Ship(serializable, loader);
				case TechType.Satellite:
					return new Technology_Satellite(serializable, loader);
				default:
                    throw new DatabaseException("Technology: Invalid content type - " + serializable.Type);
			}
		}

		public abstract T Create<T>(ITechnologyFactory<T> factory);

		protected Technology(TechnologySerializable serializable, Database.Loader loader)
		{
			Id = new ItemId<Technology>(serializable.Id);
			loader.AddTechnology(serializable.Id, this);

			Type = serializable.Type;
			Price = UnityEngine.Mathf.Clamp(serializable.Price, 0, 10000);
			Hidden = serializable.Hidden;
			Special = serializable.Special;
			Dependencies = new ImmutableCollection<Technology>(serializable.Dependencies?.Select(item => loader.GetTechnology(new ItemId<Technology>(item), true)));

			OnDataDeserialized(serializable, loader);
		}

		public readonly ItemId<Technology> Id;

		public TechType Type { get; private set; }
		public int Price { get; private set; }
		public bool Hidden { get; private set; }
		public bool Special { get; private set; }
		public ImmutableCollection<Technology> Dependencies { get; private set; }

		public static Technology DefaultValue { get; private set; }
	}

	public interface ITechnologyFactory<T>
    {
	    T Create(Technology_Component content);
	    T Create(Technology_Ship content);
	    T Create(Technology_Satellite content);
    }

    public partial class Technology_Component : Technology
    {
		partial void OnDataDeserialized(TechnologySerializable serializable, Database.Loader loader);

  		public Technology_Component(TechnologySerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			Component = loader.GetComponent(new ItemId<Component>(serializable.ItemId));
			if (Component == null)
			    throw new DatabaseException(this.GetType().Name + ".Component cannot be null - " + serializable.ItemId);
			Faction = loader.GetFaction(new ItemId<Faction>(serializable.Faction));

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(ITechnologyFactory<T> factory)
        {
            return factory.Create(this);
        }

		public Component Component { get; private set; }
		public Faction Faction { get; private set; }
    }
    public partial class Technology_Ship : Technology
    {
		partial void OnDataDeserialized(TechnologySerializable serializable, Database.Loader loader);

  		public Technology_Ship(TechnologySerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			Ship = loader.GetShip(new ItemId<Ship>(serializable.ItemId));
			if (Ship == null)
			    throw new DatabaseException(this.GetType().Name + ".Ship cannot be null - " + serializable.ItemId);

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(ITechnologyFactory<T> factory)
        {
            return factory.Create(this);
        }

		public Ship Ship { get; private set; }
    }
    public partial class Technology_Satellite : Technology
    {
		partial void OnDataDeserialized(TechnologySerializable serializable, Database.Loader loader);

  		public Technology_Satellite(TechnologySerializable serializable, Database.Loader loader)
            : base(serializable, loader)
        {
			Satellite = loader.GetSatellite(new ItemId<Satellite>(serializable.ItemId));
			if (Satellite == null)
			    throw new DatabaseException(this.GetType().Name + ".Satellite cannot be null - " + serializable.ItemId);
			Faction = loader.GetFaction(new ItemId<Faction>(serializable.Faction));

            OnDataDeserialized(serializable, loader);
        }

        public override T Create<T>(ITechnologyFactory<T> factory)
        {
            return factory.Create(this);
        }

		public Satellite Satellite { get; private set; }
		public Faction Faction { get; private set; }
    }

}

