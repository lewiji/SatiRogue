using System.Collections.Generic;
using System.Linq;
using Godot;
using SatiRogue.Debug;
using SatiRogue.Ecs.Dungeon.Nodes;
using SatiRogue.Ecs.Dungeon.Nodes.Actors;
namespace SatiRogue.Ecs.MapGenerator.Components;

public enum CellType {
   Floor,
   Wall,
   DoorClosed,
   DoorOpen,
   Stairs,
   Void
}

public enum CellCondition {
   Frozen,
   Aflame,
   Wet,
   Destroyed
}

public enum CellVisibility {
   Unseen,
   Seen,
   CurrentlyVisible
}

public class Cell : Reference {
   [Signal] public delegate void VisibilityChanged();

   long _id;

   float? _luminosity;
   Vector3? _position;

   CellVisibility _visibility = CellVisibility.Unseen;
   public HashSet<CellCondition> Conditions = new();
   public HashSet<ulong> Occupants = new();
   public CellType? Type;
   public long Id {
      get => _id;
      set {
         _id = value;
         _position ??= IdCalculator.Vec3FromId(_id);
      }
   }
   public Vector3 Position {
      get => _position.GetValueOrDefault();
      set => _position = value;
   }
   public float? Luminosity {
      get => _luminosity;
      set {
         _luminosity = value;
         if (_luminosity != null) SetCellVisibility(CellVisibility.CurrentlyVisible);
      }
   }
   public CellVisibility Visibility {
      get => _visibility;
      set {
         _visibility = value;
         SetOccupantVisibility();
      }
   }
   public bool Blocked {
      get => Conditions.Contains(CellCondition.Destroyed) || Type is CellType.Wall or CellType.DoorClosed || Occupants.Any(x =>
         GD.InstanceFromId(x) is GameObject {
            BlocksCell: true,
            Enabled: true
         });
   }

   public bool Occupied {
      get => Conditions.Contains(CellCondition.Destroyed) || Type is CellType.Wall or CellType.DoorClosed || Occupants.Any(x =>
         GD.InstanceFromId(x) is GameObject {
            Enabled: true
         });
   }

   void SetOccupantVisibility() {
      foreach (var occupant in Occupants) {
         if (GD.InstanceFromId(occupant) is not Character character || !IsInstanceValid(character)) continue;

         character.CheckVisibility(_visibility == CellVisibility.CurrentlyVisible);
         Logger.Debug($"Setting {character.Name} Visibility: {character.Visible}");
      }

      EmitSignal(nameof(VisibilityChanged));
   }

   public Cell SetCellVisibility(CellVisibility? visibility) {
      Visibility = visibility.GetValueOrDefault();

      return this;
   }
}