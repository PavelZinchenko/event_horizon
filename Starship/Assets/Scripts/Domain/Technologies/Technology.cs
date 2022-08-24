using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Constructor;
using Constructor.Ships;
using Constructor.Ships.Modification;
using Economy.ItemType;
using Economy.Products;
using GameDatabase;
using GameDatabase.DataModel;
using GameDatabase.Enums;
using GameDatabase.Model;
using GameDatabase.Extensions;
using GameServices.Database;
using Services.Localization;
using Services.Reources;
using UnityEngine.Assertions;

namespace DataModel.Technology
{
	public abstract class TechnologyBase : ITechnology
	{
		protected TechnologyBase(ITechnologies technologies, GameDatabase.DataModel.Technology data)
		{
			Id = data.Id;
		    _technologies = technologies;
			_requirements = data.Dependencies.Select(item => item.Id);
			Price = data.Price;
			Hidden = data.Hidden;
		    Special = data.Special;
		}

		public ItemId<GameDatabase.DataModel.Technology> Id { get; private set; }

	    public IEnumerable<ITechnology> Requirements
	    {
	        get
	        {
	            return _requirementList ?? (_requirementList = _requirements.Select<ItemId<GameDatabase.DataModel.Technology>, ITechnology>(_technologies.Get).ToList());
	        }
	    }

	    public abstract CraftingPrice GetCraftPrice(CraftItemQuality quality);
        public abstract string GetName(ILocalization localization);
	    public abstract UnityEngine.Sprite GetImage(IResourceLocator resourceLocator);
	    public abstract string GetDescription(ILocalization localization);
		public abstract UnityEngine.Color Color { get; }
		public abstract Faction Faction { get; }
		public abstract IProduct CreateItem(CraftItemQuality quality, System.Random random);
		public bool Hidden { get; private set; }
	    public bool Special { get; private set; }
        public int Price { get; private set; }

		private List<ITechnology> _requirementList;
		private readonly IEnumerable<ItemId<GameDatabase.DataModel.Technology>> _requirements;
	    private readonly ITechnologies _technologies;
	}

	public class ShipTechnology : TechnologyBase
	{
		public ShipTechnology(ITechnologies technologies, IDatabase database, ItemTypeFactory factory, Technology_Ship data)
			: base(technologies, data)
		{
			_factory = factory;
		    _database = database;
		    Ship = data.Ship;
		}
				
		public override string GetName(ILocalization localization) { return localization.GetString(Ship.Name); }
		public override UnityEngine.Sprite GetImage(IResourceLocator resourceLocator) { return resourceLocator.GetSprite(Ship.ModelImage); }
		public override string GetDescription(ILocalization localization) { return localization.GetString("$Ship"); }
		public override UnityEngine.Color Color { get { return UnityEngine.Color.white; } }
		public override Faction Faction { get { return Ship.Faction; } }

	    public override CraftingPrice GetCraftPrice(CraftItemQuality quality)
	    {
	        var credits = Ship.Layout.CellCount*Ship.Layout.CellCount*5;
	        var stars = 0;
	        var techs = 0;

            switch (Ship.ShipCategory)
            {
                case ShipCategory.Common:
                    stars = Ship.Layout.CellCount / 70;
                    break;
                case ShipCategory.Rare:
                    stars = 1 + (Ship.Layout.CellCount - 30) / 10;
                    credits = 3*credits/2;
                    break;
                case ShipCategory.Flagship:
                    stars = Ship.Layout.CellCount / 10;
                    credits *= 3;
                    break;
            }

            switch (quality)
            {
                case CraftItemQuality.Improved:
                    credits += credits/3;
                    stars += 3 + stars / 3;
                    techs = Ship.Layout.CellCount/20;
                    break;
                case CraftItemQuality.Excellent:
                    credits += 2*credits/3;
                    stars += 6 + 2*stars/3;
                    techs = Ship.Layout.CellCount / 10;
                    break;
                case CraftItemQuality.Superior:
                    credits += credits;
                    stars += 10 + stars;
                    techs = Ship.Layout.CellCount/5;
                    break;
            }

            return new CraftingPrice(credits, stars, techs);
	    }

        public override IProduct CreateItem(CraftItemQuality quality, System.Random random)
		{
            IShipModel model;
            switch (quality)
            {
                case CraftItemQuality.Common:
                    model = new ShipModel(Ship);
                    break;
                case CraftItemQuality.Improved:
                    model = new ShipModel(Ship);
                    model.Modifications.Add(new EmptyModification());
                    break;
                case CraftItemQuality.Excellent:
                    model = new ShipModel(Ship);
                    model.Modifications.Add(new EmptyModification());
                    model.Modifications.Add(new EmptyModification());
                    break;
                case CraftItemQuality.Superior:
                    model = new ShipModel(Ship);
                    model.Modifications.Add(new EmptyModification());
                    model.Modifications.Add(new EmptyModification());
                    model.Modifications.Add(new EmptyModification());
                    break;
                default:
                    throw new InvalidEnumArgumentException();
            }
            
            var defaultBuild = _database.ShipBuildList.Available().FirstOrDefault(build => build.Ship.Id == Ship.Id && build.DifficultyClass == DifficultyClass.Default);
            var ship = new CommonShip(model, defaultBuild?.Components.Select<InstalledComponent,IntegratedComponent>(Constructor.ComponentExtensions.FromDatabase) ?? Enumerable.Empty<IntegratedComponent>());
			return new Product(_factory.CreateShipItem(ship));
		}

	    public Ship Ship { get; private set; }

	    private readonly IDatabase _database;
		private readonly ItemTypeFactory _factory;
	}
			
	public class ComponentTechnology : TechnologyBase
	{
		public ComponentTechnology(ITechnologies technologies, ItemTypeFactory factory, Technology_Component data)
			: base(technologies, data)
		{
			_faction = data.Faction;
			_factory = factory;
		    Component = data.Component;
		}
				
		public override string GetName(ILocalization localization) { return localization.GetString(Component.Name); }
		public override UnityEngine.Sprite GetImage(IResourceLocator resourceLocator) { return resourceLocator.GetSprite(Component.Icon); }
		public override string GetDescription(ILocalization localization)
        {
			switch (Component.DisplayCategory)
			{
			case ComponentCategory.Weapon:
				return localization.GetString("$Weapon");
			case ComponentCategory.Defense:
				return localization.GetString("$Armor");
			case ComponentCategory.Drones:
                return localization.GetString("$DroneBay");
			case ComponentCategory.Energy:
				return localization.GetString("$Reactor");
			case ComponentCategory.Engine:
                return localization.GetString("$Engine");
			default:
				return localization.GetString("$Device");
			}
		}

	    public override CraftingPrice GetCraftPrice(CraftItemQuality quality)
	    {
	        var credits = Economy.Price.ComponentPrice(Component).Amount;
	        var stars = Component.Level/75;
            var techs = 0;

            switch (quality)
            {
                case CraftItemQuality.Improved:
                    credits += credits / 2;
                    techs = 1;
                    break;
                case CraftItemQuality.Excellent:
                    credits += 2*credits / 3;
                    stars += 1 + stars / 2;
                    techs = UnityEngine.Mathf.Max(3, Component.Level / 10);
                    break;
                case CraftItemQuality.Superior:
                    credits += credits;
                    stars += 1 + stars;
                    techs = UnityEngine.Mathf.Max(5, Component.Level/20);
                    break;
            }

            return new CraftingPrice(credits, stars, techs);
	    }

        public override UnityEngine.Color Color { get { return Component.Color; } }
		public override Faction Faction { get { return _faction; } }

	    public override IProduct CreateItem(CraftItemQuality quality, System.Random random)
	    {
	        switch (quality)
	        {
	            case CraftItemQuality.Common:
	                return new Product(_factory.CreateComponentItem(new ComponentInfo(Component)));
                case CraftItemQuality.Improved:
                    return new Product(_factory.CreateComponentItem(ComponentInfo.CreateRandomModification(Component, random, ModificationQuality.P1, ModificationQuality.P1)));
                case CraftItemQuality.Excellent:
                    return new Product(_factory.CreateComponentItem(ComponentInfo.CreateRandomModification(Component, random, ModificationQuality.P2, ModificationQuality.P2)));
                case CraftItemQuality.Superior:
                    return new Product(_factory.CreateComponentItem(ComponentInfo.CreateRandomModification(Component, random, ModificationQuality.P2, ModificationQuality.P3)));
                default:
                    throw new InvalidEnumArgumentException();
            }
        }

	    public GameDatabase.DataModel.Component Component { get; private set; }

	    private readonly Faction _faction;
        private readonly ItemTypeFactory _factory;
	}

	public class SatelliteTechnology : TechnologyBase
	{
		public SatelliteTechnology(ITechnologies technologies, ItemTypeFactory factory, Technology_Satellite data)
			: base(technologies, data)
		{
			_factory = factory;
			_faction = data.Faction;
		    _satellite = data.Satellite;
		}
				
		public override string GetName(ILocalization localization) { return localization.GetString(_satellite.Name); }
		public override UnityEngine.Sprite GetImage(IResourceLocator resourceLocator) { return resourceLocator.GetSprite(_satellite.ModelImage); }
		public override string GetDescription(ILocalization localization) { return localization.GetString("$Satellite"); }
		public override UnityEngine.Color Color { get { return UnityEngine.Color.white; } }
		public override Faction Faction { get { return _faction; } }

	    public override IProduct CreateItem(CraftItemQuality quality, System.Random random)
	    {
            if (quality != CraftItemQuality.Common)
                throw new ArgumentException();

	        return new Product(_factory.CreateSatelliteItem(_satellite));
	    }

	    public override CraftingPrice GetCraftPrice(CraftItemQuality quality)
	    {
	        var credits = 10*Economy.Price.SatellitePrice(_satellite).Amount;
            var stars = UnityEngine.Mathf.Max(0, _satellite.Layout.CellCount / 7 - 1);
	        var techs = 0;

	        switch (quality)
	        {
	            case CraftItemQuality.Improved:
	                credits += credits/2;
	                techs = 1;
	                break;
                case CraftItemQuality.Excellent:
                    credits += 2*credits/3;
                    stars += 3 + stars/2;
                    techs = 5;
                    break;
                case CraftItemQuality.Superior:
                    credits += credits;
                    stars += 5 + stars;
                    techs = 10;
                    break;
            }

            return new CraftingPrice(credits, stars, techs);
        }

	    private readonly Satellite _satellite;
		private readonly Faction _faction;
		private readonly ItemTypeFactory _factory;
	}
}
