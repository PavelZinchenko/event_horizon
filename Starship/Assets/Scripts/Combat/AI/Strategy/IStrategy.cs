using Combat.Component.Ship;
using Combat.Component.Unit;

namespace Combat.Ai
{
	public interface IStrategy 
	{
		bool IsThreat(IShip ship, IUnit unit);
		void Apply(Context context);
	}
}
