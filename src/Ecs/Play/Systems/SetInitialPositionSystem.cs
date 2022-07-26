using System.Linq;
using Godot;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.MapGenerator.Components;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Nodes.Actors;

namespace SatiRogue.Ecs.Play.Systems; 

public class SetInitialPositionSystem : GDSystem {
   public override void Run() {
      var mapData = GetElement<MapGenData>();
      var query = Query<Character, GridPositionComponent>();

      foreach (var (character, gridPos) in query) {
         var availableCells = mapData.IndexedCells.Where(c => !c.Value.Blocked).ToArray();
         if (availableCells.Length == 0) continue;
         
         var chosenCell = availableCells[GD.Randi() % availableCells.Length];
         gridPos.Position = chosenCell.Value.Position;
         chosenCell.Value.Occupants.Add(character.GetInstanceId());
         
         Logger.Info($"{character} initial gridpos: {gridPos.Position}");
      }
   }
}