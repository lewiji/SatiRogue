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
         Height = 25,
         Width = 25,
         NumRooms = 5,
         MaxRoomWidth = 10,
         MinRoomWidth = 3,
         NumEnemies = 0
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

      var placeEntitiesCommandQueue = new CommandQueue();
      placeEntitiesCommandQueue.Add(new MapGenPlacePlayer(MapData));
      placeEntitiesCommandQueue.Add(new MapGenPlaceEnemies(MapData));
      placeEntitiesCommandQueue.ExecuteAll();
   }
}