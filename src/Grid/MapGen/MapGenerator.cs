using Godot;
using SatiRogue.Commands;
using SatiRogue.Commands.MapGen;
using SatiRogue.Debug;

namespace SatiRogue.Grid.MapGen;

public class MapGenerator : Node {
   [Signal]
   public delegate void MapChanged();

   [Signal]
   public delegate void VisibilityChanged(Vector3[] changedCells);

   public static NodePath? Path;

   public static MapData MapData = new(new MapGenParams {
      Height = 150,
      Width = 150,
      NumRooms = 35,
      MaxRoomWidth = 32,
      MinRoomWidth = 32,
      NumEnemies = 50
   });

   public AStar AStar = new();

   public static MapGenParams GetParams() {
      return MapData.MapParams;
   }

   public override void _EnterTree() {
      Path = GetPath();
   }

   public override void _Ready() {
      GD.Randomize();
      CallDeferred(nameof(StartGeneration));
   }

   private void StartGeneration() {
      var commandQueue = new CommandQueue();
      commandQueue.Add(new MapGenCreateRooms(MapData));
      commandQueue.Add(new MapGenCreateCorridors(MapData));
      commandQueue.Add(new MapGenFloodFill(MapData));
      commandQueue.Add(new MapGenConnectPathfindingPoints(MapData));
      commandQueue.Add(new MapGenPlacePlayer(MapData));
      commandQueue.Add(new MapGenPlaceEnemies(MapData));
      commandQueue.ExecuteAll();

      Logger.Info("Finished generating grid.");
      CallDeferred(nameof(EmitMapChangedSignal));
   }

   public override void _Process(float delta) {
      if (MapData.CellsVisibilityChanged.Count > 0) {
         EmitSignal(nameof(VisibilityChanged), MapData.CellsVisibilityChanged.ToArray());
         MapData.CellsVisibilityChanged.Clear();
      }
   }

   private void EmitMapChangedSignal() {
      EmitSignal(nameof(MapChanged));
   }
}