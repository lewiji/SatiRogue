using System;
using SatiRogue.Math;

namespace SatiRogue.Grid.Entities;

public enum MovementDirection {
    Left,
    Up,
    Right,
    Down
}

public class EntityUtils {
    public static Vector3i MovementDirectionToVector(MovementDirection dir) {
        switch (dir) {
            case MovementDirection.Left:
                return Vector3i.Left;
            case MovementDirection.Up:
                return Vector3i.Forward;
            case MovementDirection.Right:
                return Vector3i.Right;
            case MovementDirection.Down:
                return Vector3i.Back;
            default:
                throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
        }
    }
}