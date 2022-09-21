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
	public partial class Skill
	{
		partial void OnDataDeserialized(SkillSerializable serializable, Database.Loader loader);

		public static Skill Create(SkillSerializable serializable, Database.Loader loader)
		{
			return new Skill(serializable, loader);
		}

		private Skill(SkillSerializable serializable, Database.Loader loader)
		{
			Id = new ItemId<Skill>(serializable.Id);
			loader.AddSkill(serializable.Id, this);

			Name = serializable.Name;
			Icon = new SpriteId(serializable.Icon, SpriteId.Type.SkillIcon);
			Description = serializable.Description;
			BaseRequirement = UnityEngine.Mathf.Clamp(serializable.BaseRequirement, 0f, 100f);
			RequirementPerLevel = UnityEngine.Mathf.Clamp(serializable.RequirementPerLevel, 0f, 100f);
			BasePrice = UnityEngine.Mathf.Clamp(serializable.BasePrice, 0f, 100f);
			PricePerLevel = UnityEngine.Mathf.Clamp(serializable.PricePerLevel, 0f, 100f);
			MaxLevel = UnityEngine.Mathf.Clamp(serializable.MaxLevel, 1, 1000);

			OnDataDeserialized(serializable, loader);
		}

		public readonly ItemId<Skill> Id;

		public string Name { get; private set; }
		public SpriteId Icon { get; private set; }
		public string Description { get; private set; }
		public float BaseRequirement { get; private set; }
		public float RequirementPerLevel { get; private set; }
		public float BasePrice { get; private set; }
		public float PricePerLevel { get; private set; }
		public int MaxLevel { get; private set; }

		public static Skill DefaultValue { get; private set; }
	}
}
