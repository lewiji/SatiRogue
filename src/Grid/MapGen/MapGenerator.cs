using System;
using Godot;
using SatiRogue.Commands;
using SatiRogue.Commands.MapGen;
using SatiRogue.Debug;
using SatiRogue.Grid.MapGen.Strategies;

namespace SatiRogue.Grid.MapGen;

public class MapGenerator : Node {

   public static NodePath? Path;
   public static MapGenMapData? MapData;
   public static IMapGenStrategy? MapGenStrategy;
   
   public AStar AStar = new();

   public static MapGenParams? GetParams() {
      return MapData?.MapParams;
   }

   public override void _EnterTree() {
      Path = GetPath();
   }

   public override void _Ready() {
      GD.Randomize();
      
      MapGenStrategy ??= new RoomsAndCorridors(new MapGenParams {
         Height = 75,
         Width = 75,
         NumRooms = 18,
         MaxRoomWidth = 16,
         MinRoomWidth = 3,
         NumEnemies = 24
      });
      
      CallDeferred(nameof(StartGeneration));
   }

   private void StartGeneration() {
      MapData = MapGenStrategy?.GenerateMap();

      if (MapData == null)
         throw new Exception($"MapGenerator: No MapData returned from MapGenStrategy {MapGenStrategy}");

      Logger.Info("Finished generating grid.");

      var runtimeMapNode = new RuntimeMapNode();
      GetParent().AddChild(runtimeMapNode);
      runtimeMapNode.Owner = GetParent();
      runtimeMapNode.MapData = new MapData(MapData);
   }
}