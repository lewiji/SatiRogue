using System.Collections.Generic;
using Godot;
using SatiRogue.Grid;

namespace SatiRogue.Ecs.MapGenerator.Components;

public struct GeneratorParameters {
   public GeneratorParameters() { }

   public int Height { get; set; } = 75;
   public int Width { get; set; } = 75;
   public int NumRooms { get; set; } = 12;
   public int MinRoomWidth { get; set; } = 20;
   public int MaxRoomWidth { get; set; } = 4;
   public int NumEnemies { get; set; } = 15;
}


public struct MapGenRoomParams {
   public MapGenRoomParams(GeneratorParameters mapParams) {
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

public class MapGenData {
   public Dictionary<long, Cell> IndexedCells { get; protected set; } = new();
   public HashSet<Rect2> GeneratorSpaces;
   public GeneratorParameters GeneratorParameters;
   public MapGenData(GeneratorParameters? mapParams = null) {
      GeneratorSpaces = new HashSet<Rect2>();
      GeneratorParameters = mapParams ?? new GeneratorParameters();
   }
   
   public Cell SetCellType(Vector3 position, CellType type) {
      var cell = InitialiseOrGetCell(position);
      cell.Type = type;
      return cell;
   }
   
   public Cell SetCellType(long id, CellType type) {
      var cell = InitialiseOrGetCell(id);
      cell.Type = type;
      return cell;
   }
   
   public Cell GetCellAt(Vector3 position) {
      return InitialiseOrGetCell(position);
   }

   private Cell InitialiseOrGetCell(Vector3 position) {
      return InitialiseOrGetCell(IdCalculator.IdFromVec3(position));
   }

   private Cell InitialiseOrGetCell(long id) {
      // Try to add id to collection, if already exists, return matching cell struct
      if (IndexedCells.ContainsKey(id))
         return IndexedCells[id];
      // create and add new cell otherwise
      var cell = new Cell{Id = id};
      IndexedCells[id] = cell;
      return cell;
   }
}