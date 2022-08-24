using Combat.Component.Ship;
using Combat.Unit;

namespace Combat.Ai
{
    public class EmptyController : IController
    {
	    public EmptyController(IShip ship) 
	    {
		    _ship = ship;
	    }

	    public void Update(float deltaTime) {}
	    public bool IsAlive { get { return _ship.IsActive(); } }

	    private readonly IShip _ship;

        public class Factory : IControllerFactory
        {
            public IController Create(IShip ship)
            {
                return new EmptyController(ship);
            }
        }
    }
}
