using System.Linq;
using DataModel.Technology;

namespace GameModel
{
	public static class Craft
	{
		public static int GetWorkshopLevel(ITechnology technology)
		{
		    return technology.Price + technology.Requirements.Sum(GetWorkshopLevel);
		}
    }
}
