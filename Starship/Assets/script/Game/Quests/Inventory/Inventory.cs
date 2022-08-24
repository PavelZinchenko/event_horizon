using System.Collections.Generic;
using System.Linq;
using Economy.Products;

namespace GameModel
{
    namespace Quests
	{
		public class Inventory : IInventory
		{
			public void Refresh() {}

			public Inventory(IEnumerable<IProduct> items = null)
			{
				if (items != null)
					_items.AddRange(items);
			}
			
			public IEnumerable<IProduct> Items { get { return _items; } }

			private List<IProduct> _items = new List<IProduct>();
		}
	}
}
