using System;
using Godot;
using GoDotNet;
using SatiRogue.Commands;
using SatiRogue.Commands.MapGen;
using SatiRogue.Components;
using SatiRogue.Debug;
using SatiRogue.Grid.MapGen.Strategies;

namespace SatiRogue.Grid.MapGen;

public class MapGenerator : Node{
   public static MapGenMapData? MapData;
   public static IMapGenStrategy? MapGenStrategy;
   
   public AStar AStar = new();

   public static MapGenParams? GetParams() {
      return MapData?.MapParams;
   }
   
   public override void _Ready() {
      GD.Randomize();
      
      MapGenStrategy ??= new RoomsAndCorridors(new MapGenParams {
         Height = 75,
         Width = 75,
         NumRooms = 12,
         MaxRoomWidth = 20,
         MinRoomWidth = 4,
         NumEnemies = 15
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

      CallDeferred(nameof(EnableInput));
   }

   private void EnableInput() {
      InputHandlerComponent.InputEnabled = true;
   }

   public void NextFloor() {
      GetNode<GameController>(GameController.Path).Restart();
   }
}