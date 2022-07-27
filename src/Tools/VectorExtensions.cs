using Godot;

namespace SatiRogue.Tools; 

public static class VectorExtensions {
   public static Vector3 ToVector3(this Vector2 vector2, float height = 0.0f) {
      return new Vector3(vector2.x, height, vector2.y);
   }
   
   public static Vector2 ToVector2(this Vector3 vector3) {
      return new Vector2(vector3.x, vector3.z);
   }
}