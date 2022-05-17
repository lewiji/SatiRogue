using System;

namespace SatiRogue.MathUtils; 

public static class BresenhamsLine {
   private static void Swap<T>(ref T lhs, ref T rhs) => (lhs, rhs) = (rhs, lhs);

   public delegate bool PlotFunction(Vector3i gridPos);

   public static bool Line(Vector3i from, Vector3i to, PlotFunction plot) {
      bool steep = Math.Abs(to.z - from.z) > Math.Abs(to.x - from.x);
      if (steep) { Swap<int>(ref from.x, ref from.z); Swap<int>(ref to.x, ref to.z); }
      if (from.x > to.x) { Swap<int>(ref from.x, ref to.x); Swap<int>(ref from.z, ref to.z); }
      int dX = (to.x - from.x), dY = Math.Abs(to.z - from.z), err = (dX / 2), ystep = (from.z < to.z ? 1 : -1), y = from.z;

      for (int x = from.x; x <= to.x; ++x)
      {
         if (!(steep ? plot(new Vector3i(y, 0, x)) : plot(new Vector3i(x, 0, y)))) return false;
         err = err - dY;
         if (err < 0) { y += ystep;  err += dX; }
      }

      return true;
   }
}