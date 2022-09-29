using Godot;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.MapGenerator.Systems;
using SatiRogue.Ecs.MapGenerator.Systems.MapGenStrategies;
using RelEcs;
using World = RelEcs.World;

namespace SatiRogue.Ecs;

public class MapGenState : GameState {
   [Signal]
   public delegate void FinishedGenerating();

   ISystem[] _mapGenStrategySystems = { };

   public MapGenState() { }

   public MapGenState(ISystem[] mapGenStrategySystems) {
      _mapGenStrategySystems = mapGenStrategySystems;
   }

   public override void Init(GameStateController gameStates) {
      gameStates.World.AddElement(this);

      InitSystems.Add(new InitMapGen());
      ContinueSystems.Add(new ResetMapGen());

      if (_mapGenStrategySystems.Length > 0) {
         foreach (var mapGenStrategySystem in _mapGenStrategySystems) {
            InitSystems.Add(mapGenStrategySystem);
            ContinueSystems.Add(mapGenStrategySystem);
         }
      } else {
         // default
         AddDefaultGeneratorStrategies(InitSystems);
         AddDefaultGeneratorStrategies(ContinueSystems);
      }

      InitSystems.Add(new PassToPlayState());
      ContinueSystems.Add(new PassToPlayState());
   }

   void AddDefaultGeneratorStrategies(SystemGroup systemGroup) {
      systemGroup.Add(new CreateRooms()).Add(new CreateCorridors()).Add(new FloodFill()).Add(new GenPathfindingNodes());
   }
}