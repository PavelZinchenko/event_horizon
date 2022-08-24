using System.Collections.Generic;
using Economy.Products;

namespace GameModel
{
    namespace Quests
	{
		public interface IInventory
		{
			void Refresh();
			IEnumerable<IProduct> Items { get; }
			//int Money { get; }
		}
	}
}
