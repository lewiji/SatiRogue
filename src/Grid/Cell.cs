using System.Collections.Generic;
using System.Linq;
using Godot;
using SatiRogue.Debug;
using SatiRogue.Entities;
using SatiRogue.Grid.MapGen;
using SatiRogue.MathUtils;

namespace SatiRogue.Grid;

public enum CellCondition {
   Frozen,
   Aflame,
   Wet,
   Destroyed
}

public enum CellType {
   Floor,
   Wall,
   DoorClosed,
   DoorOpen,
   Stairs,
   Void
}

public enum CellVisibility {
   Unseen,
   Seen,
   CurrentlyVisible
}

public class Cell : Reference {
   [Signal] public delegate void CellTypeChanged(int cellTypeId, long cellId);
   [Signal] public delegate void VisibilityChanged();
   
   private float? _luminosity;
   private CellType? _type;

   public HashSet<CellCondition> Conditions = new();
   public long Id;
   public HashSet<ulong> Occupants = new();
   private CellVisibility _visibility = CellVisibility.Unseen;

   public CellVisibility Visibility
   {
      get => _visibility;
      set
      {
         if (_visibility == value) return;
         _visibility = value;
         SetOccupantVisibility();
         EmitSignal(nameof(VisibilityChanged));
      }
   }

   public Cell(long id, CellType? type = null, IEnumerable<ulong>? occupants = null, IEnumerable<CellCondition>? conditions = null,
      CellVisibility? visibility = null) {
      Id = id;
      if (occupants != null) Occupants = occupants.ToHashSet();
      if (conditions != null) Conditions = conditions.ToHashSet();
      if (visibility != null) Visibility = visibility.Value;
      Type = type;
   }

   private void SetOccupantVisibility()
   {
      foreach (var occupant in Occupants)
      {
         if (GD.InstanceFromId(occupant) is GridEntity gridEntity)
         {
            gridEntity.Visible = Visibility == CellVisibility.CurrentlyVisible;
            Logger.Info($"Setting {gridEntity.Name} Visibility: {gridEntity.Visible}");
         }
      }
   }

   public CellType? Type {
      get => _type;
      set {
         _type = value;
         EmitSignal(nameof(CellTypeChanged), (int)_type.GetValueOrDefault(), this.Id);
      }
   }

   public float? Luminosity {
      get => _luminosity;
      set {
         _luminosity = value;
         if (_luminosity != null) SetCellVisibility(CellVisibility.CurrentlyVisible);
      }
   }

   public Vector3i Position => IdCalculator.Vec3FromId(Id);

   public bool Blocked =>
      Conditions.Contains(CellCondition.Destroyed) ||
      Type is CellType.Wall or CellType.DoorClosed ||
      Occupants.Count(x => GD.InstanceFromId(x).Get("BlocksCell").Equals(true)) > 0;

   public static Cell FromPosition(Vector3i position) {
      return new Cell(IdCalculator.IdFromVec3(position));
   }

   public Cell SetCellType(CellType? type) {
      Type = type;
      return this;
   }

   public Cell SetCellVisibility(CellVisibility? visibility) {
      Visibility = visibility.GetValueOrDefault();
      return this;
   }

   public Cell AddOccupant(ulong uid) {
      Occupants.Add(uid);
      return this;
   }

   public Cell RemoveOccupant(ulong uid) {
      Occupants.Remove(uid);
      return this;
   }

   public Cell RemoveOccupants() {
      Occupants.Clear();
      return this;
   }

   public Cell AddCondition(CellCondition condition) {
      Conditions.Add(condition);
      return this;
   }

   public Cell RemoveCondition(CellCondition condition) {
      Conditions.Remove(condition);
      return this;
   }

   public Cell RemoveConditions() {
      Conditions.Clear();
      return this;
   }
}