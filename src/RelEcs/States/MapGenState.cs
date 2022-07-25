using RelEcs;
using SatiRogue.RelEcs.Components.MapGen;
using SatiRogue.RelEcs.Systems;
using SatiRogue.RelEcs.Systems.MapGenStrategies;
using SatiRogue.RelEcs.Systems.MapGenStratgies;
using CreateRooms = SatiRogue.RelEcs.Systems.MapGenStrategies.CreateRooms;

namespace SatiRogue.RelEcs.States; 

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