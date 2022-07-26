using System.Collections.Generic;
using System.Linq;
using Godot;
using SatiRogue.Grid;

namespace SatiRogue.Ecs.MapGenerator.Components; 

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
   public HashSet<ulong> Occupants = new();
   public HashSet<CellCondition> Conditions = new();
   public bool Blocked => 
      Conditions.Contains(CellCondition.Destroyed) ||
      Type is CellType.Wall or CellType.DoorClosed ||
      Occupants.Count(x =>
           {
              var instance = GD.InstanceFromId(x);
              return instance.Get("BlocksCell").Equals(true);
           }) > 0;
   
   private long _id;
}