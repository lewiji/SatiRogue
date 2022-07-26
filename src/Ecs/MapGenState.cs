using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.MapGenerator.Systems;
using SatiRogue.Ecs.MapGenerator.Systems.MapGenStrategies;
using RelEcs;
using CreateRooms = SatiRogue.Ecs.MapGenerator.Systems.MapGenStrategies.CreateRooms;

namespace SatiRogue.Ecs; 

public class MapGenState : GameState {
   private ISystem[] _mapGenStrategySystems = {};

   public MapGenState() { }

   public MapGenState(ISystem[] mapGenStrategySystems) {
      _mapGenStrategySystems = mapGenStrategySystems;
   }
   
   public override void Init(GameStateController gameStates)
   {
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
            .Add(new PassToPlayState());
      }
   }
}