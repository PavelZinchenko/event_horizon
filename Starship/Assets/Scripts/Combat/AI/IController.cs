using Combat.Component.Ship;

namespace Combat.Ai
{
    public interface IController
    {
	    void Update(float deltaTime);
	    bool IsAlive { get; }
    }

    public interface IControllerFactory
    {
        IController Create(IShip ship);
    }
}
