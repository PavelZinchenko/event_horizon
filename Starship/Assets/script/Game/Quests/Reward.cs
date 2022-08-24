using System.Collections.Generic;
using System.Linq;
using Economy.Products;

namespace GameModel
{
    namespace Quests
	{
		public class Reward : IReward
		{
			public Reward(params IProduct[] items)
			{
				_items = items;
			}

			public IEnumerable<IProduct> Items { get { return _items; } }
			public IEnumerable<ExperienceData> Experience { get { return Enumerable.Empty<ExperienceData>(); } }
		    public ExperienceData PlayerExperience { get { return ExperienceData.Empty; } }

		    private IProduct[] _items;
		}
	}
}
