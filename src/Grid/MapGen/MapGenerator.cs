using System;
using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Commands;
using SatiRogue.Commands.MapGen;
using SatiRogue.Components;
using SatiRogue.Debug;
using SatiRogue.Grid.MapGen.Strategies;
using GoDotNet;

namespace SatiRogue.Grid.MapGen;

public partial class MapGenerator : Node, IProvider<MapGenerator>, IProvider<RuntimeMapNode> {
   public static MapGenMapData? MapData;
   public static IMapGenStrategy? MapGenStrategy;
   private RuntimeMapNode? _runtimeMapNode;
   MapGenerator IProvider<MapGenerator>.Get() => this;
   RuntimeMapNode IProvider<RuntimeMapNode>.Get() => _runtimeMapNode;
   public AStar AStar = new();

   public static MapGenParams? GetParams() {
      return MapData?.MapParams;
   }

   [OnReady]
   public void Ready() {
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

      _runtimeMapNode = new RuntimeMapNode();
      GetParent().AddChild(_runtimeMapNode);
      _runtimeMapNode.Owner = GetParent();
      _runtimeMapNode.MapData = new MapData(MapData);

      CallDeferred(nameof(PlaceEntities));
   }

   private void PlaceEntities() {
      
      var placeEntitiesCommandQueue = new CommandQueue();
      placeEntitiesCommandQueue.Add(new MapGenPlacePlayer(MapData!));
      placeEntitiesCommandQueue.Add(new MapGenPlaceEnemies(MapData!));
      
      this.Provided();
      
      this.Autoload<Scheduler>().NextFrame(() =>
      {
         placeEntitiesCommandQueue.ExecuteAll();
         InputHandlerComponent.InputEnabled = true;
      });
   }

   public void NextFloor() {
      this.Autoload<GameController>().Restart();
   }
}