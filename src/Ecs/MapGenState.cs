using Godot;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.MapGenerator.Systems;
using SatiRogue.Ecs.MapGenerator.Systems.MapGenStrategies;
using RelEcs;
using CreateRooms = SatiRogue.Ecs.MapGenerator.Systems.MapGenStrategies.CreateRooms;

namespace SatiRogue.Ecs; 

public class MapGenState : GameState {
   [Signal] public delegate void FinishedGenerating();
   private ISystem[] _mapGenStrategySystems = {};

   public MapGenState() { }

   public MapGenState(ISystem[] mapGenStrategySystems) {
      _mapGenStrategySystems = mapGenStrategySystems;
   }
   
   public override void Init(GameStateController gameStates)
   {
      gameStates.World.AddElement(this);
      
      if (_mapGenStrategySystems.Length > 0) {
         foreach (var mapGenStrategySystem in _mapGenStrategySystems) {
            InitSystems.Add(mapGenStrategySystem);
         }
      }
      else {
         // default
         InitSystems
            .Add(new InitMapGen())
            .Add(new CreateRooms())
            .Add(new CreateCorridors())
            .Add(new FloodFill())
            .Add(new GenPathfindingNodes())
            .Add(new PassToPlayState());
      }
   }
}