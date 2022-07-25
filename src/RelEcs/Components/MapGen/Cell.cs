using Godot;
using SatiRogue.Grid;

namespace SatiRogue.RelEcs.Components.MapGen; 

public enum CellType {
   Floor,
   Wall,
   DoorClosed,
   DoorOpen,
   Stairs,
   Void
}

public class Cell {
   public long Id {
      get => _id;
      set {
         _id = value;
         Position ??= IdCalculator.Vec3FromId(_id);
      }
   }
   public Vector3? Position;
   public float Luminosity;
   public CellType? Type;
   
   private long _id;
}