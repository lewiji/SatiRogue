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
         _position ??= IdCalculator.Vec3FromId(_id);
      }
   }
   private Vector3? _position;
   public Vector3 Position {
      get => _position.GetValueOrDefault();
      set => _position = value;
   }
   
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