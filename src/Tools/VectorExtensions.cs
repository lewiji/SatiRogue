using System;
using Godot;
namespace SatiRogue.Tools;

public static class VectorExtensions {
   public static Vector3 ToVector3(this Vector2 vector2, float height = 0.0f) {
      return new Vector3(vector2.x, height, vector2.y);
   }

   public static Vector2 ToVector2(this Vector3 vector3) {
      return new Vector2(vector3.x, vector3.z);
   }

   public static bool WithinManhattanDistance(this Vector3 from, Vector3 to, float distance)
   {
      var dx = MathF.Abs(to.x - from.x);
      if (dx > distance) return false;

      var dz = MathF.Abs(to.z - from.z);
      if (dz > distance) return false;

      return true;
   }
   
   /*float CBox::Within3DManhattanDistance( Vec3 c1, Vec3 c2, float distance )
   {
       float dx = abs(c2.x - c1.x);
       if (dx > distance) return 0; // too far in x direction

       float dy = abs(c2.y - c1.y);
       if (dy > distance) return 0; // too far in y direction

       // since x and y distance are likely to be larger than
       // z distance most of the time we don't need to execute
       // the code below:

       float dz = abs(c2.z - c1.z);
       if (dz > distance) return 0; // too far in z direction

       return 1; // we're within the cube
   }*/
}