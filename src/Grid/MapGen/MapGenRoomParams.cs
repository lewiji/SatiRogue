using Godot;

namespace SatiRogue.Grid.MapGen;

public struct MapGenRoomParams {
   public MapGenRoomParams(MapGenParams mapParams) {
      Width = (int) GD.RandRange(mapParams.MinRoomWidth, mapParams.MaxRoomWidth);
      Height = (int) GD.RandRange(mapParams.MinRoomWidth, mapParams.MaxRoomWidth);
      MaxX = mapParams.Width - Width;
      MaxY = mapParams.Height - Height;
      X = (int) GD.RandRange(1, MaxX);
      Y = (int) GD.RandRange(1, MaxY);
      FloorSpace = new Rect2(X, Y, Width, Height);
   }

   public int Height { get; set; }
   public int Width { get; set; }
   public int MaxX { get; set; }
   public int MaxY { get; set; }
   public int X { get; set; }
   public int Y { get; set; }
   public Rect2 FloorSpace { get; set; }
}