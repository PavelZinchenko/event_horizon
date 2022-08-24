using UnityEngine;

public interface IGameObjectInterface
{
    Vector2 Position { get; set; }
    float Rotation { get; set; }
    float Scale { get; set; }

    Color Color { get; set; }
    Vector2 Offset { get; set; }

    float Lifetime { get; set; }
    float Power { get; set; }
}
