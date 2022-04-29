using Godot;
using SatiRogue.Math;

namespace SatiRogue.Grid;

public static class IdCalculator {
    public static long IdFromVec3(Vector3i vec) {
        return SignedCantorPairVec3(vec);
    }

    public static Vector3i Vec3FromId(long id) {
        return SignedReverseCantorPairVec3(id);
    }

    public static long CantorPairVec3(Vector3i vec) {
        var xPairY = CantorPair(Mathf.FloorToInt(vec.x), Mathf.FloorToInt(vec.y));
        var xyPairZ = CantorPair(xPairY, Mathf.FloorToInt(vec.z));
        return xyPairZ;
    }

    public static Vector3i ReverseCantorPairVec3(long cantor) {
        ReverseCantorPair(cantor, out var xPairY, out var z);
        ReverseCantorPair(xPairY, out var x, out var y);
        return new Vector3i(x, y, z);
    }

    public static long SignedCantorPairVec3(Vector3i vec) {
        var xPairY = SignedCantorPair(Mathf.FloorToInt(vec.x), Mathf.FloorToInt(vec.y));
        var xyPairZ = SignedCantorPair(xPairY, Mathf.FloorToInt(vec.z));
        return xyPairZ;
    }

    public static Vector3i SignedReverseCantorPairVec3(long cantor) {
        SignedReverseCantorPair(cantor, out var xPairY, out var z);
        SignedReverseCantorPair(xPairY, out var x, out var y);
        return new Vector3i(x, y, z);
    }

    public static long CantorPair(long x, long y) {
        return (x + y) * (x + y + 1) / 2 + y;
    }

    public static void ReverseCantorPair(long cantor, out int x, out int y) {
        var t = (long) Mathf.Floor((-1 + Mathf.Sqrt(1 + 8 * cantor)) / 2);
        x = (int) (t * (t + 3) / 2 - cantor);
        y = (int) (cantor - t * (t + 1) / 2);
    }

    public static long SignedCantorPair(long x, long y) {
        x = x >= 0 ? 2 * x : x * -2 + 1;
        y = y >= 0 ? 2 * y : y * -2 + 1;

        return (x + y) * (x + y + 1) / 2 + y;
    }

    public static void SignedReverseCantorPair(long cantor, out int x, out int y) {
        var t = (long) Mathf.Floor((-1 + Mathf.Sqrt(1 + 8 * cantor)) / 2);
        x = (int) (t * (t + 3) / 2 - cantor);
        y = (int) (cantor - t * (t + 1) / 2);

        x = x % 2 == 0 ? x / 2 : (1 - x) / 2;
        y = y % 2 == 0 ? y / 2 : (1 - y) / 2;
    }
}