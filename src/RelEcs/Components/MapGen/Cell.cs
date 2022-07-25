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
   public long Id;
   public float Luminosity;
   public CellType Type;
}