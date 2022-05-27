namespace SatiRogue.Grid.MapGen;

public struct MapGenParams {
   public int Height { get; set; }
   public int Width { get; set; }
   public int NumRooms { get; set; }
   public int MinRoomWidth { get; set; }
   public int MaxRoomWidth { get; set; }
   public int NumEnemies { get; set; }
}