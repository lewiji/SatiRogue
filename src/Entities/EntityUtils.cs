using System;
using Godot;
using SatiRogue.Math;

namespace SatiRogue.Entities;

public enum MovementDirection {
    Left,
    Up,
    Right,
    Down,
    UpLeft,
    UpRight,
    DownLeft,
    DownRight,
    None
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
            case MovementDirection.UpLeft:
                return Vector3i.Forward + Vector3i.Left;
            case MovementDirection.UpRight:
                return Vector3i.Forward + Vector3i.Right;
            case MovementDirection.DownLeft:
                return Vector3i.Back + Vector3i.Left;
            case MovementDirection.DownRight:
                return Vector3i.Back + Vector3i.Right;
            case MovementDirection.None:
                return Vector3i.Zero;
            default:
                throw new ArgumentOutOfRangeException(nameof(dir), dir, null);
        }
    }

    public static MovementDirection VectorToMovementDirection(Vector3i vec3i) {
        return VectorToMovementDirection(vec3i.ToVector3());
    }

    public static MovementDirection VectorToMovementDirection(Vector3 vec3) {
        if (vec3.IsEqualApprox(Vector3.Left)) {
            return MovementDirection.Left;
        } else if (vec3.IsEqualApprox(Vector3.Right)) {
            return MovementDirection.Right;
        } else if (vec3.IsEqualApprox(Vector3.Forward)) {
            return MovementDirection.Up;
        } else if (vec3.IsEqualApprox(Vector3.Back)) {
            return MovementDirection.Down;
        } else if (vec3.IsEqualApprox(Vector3.Forward + Vector3.Right)) {
            return MovementDirection.UpRight;
        } else if (vec3.IsEqualApprox(Vector3.Forward + Vector3.Left)) {
            return MovementDirection.UpLeft;
        } else if (vec3.IsEqualApprox(Vector3.Back + Vector3.Right)) {
            return MovementDirection.DownRight;
        } else if (vec3.IsEqualApprox(Vector3.Back + Vector3.Left)) {
            return MovementDirection.DownLeft;
        }

        return MovementDirection.None;
    }
}