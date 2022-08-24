using Combat.Component.Ship;
using UnityEngine;

namespace Combat.Scene
{
    public interface IViewRect
    {
        float MaxWidth { get; }
        float MaxHeight { get; }

        Rect Rect { get; }
        Vector2 Center { get; }

        void Zoom();

        void Update(IShip playerShip, IShip enemyShip, bool playerInCenter);
    }
}
