using System.Linq;
using Godot;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.MapGenerator.Components;
using SatiRogue.Ecs.MapGenerator.Systems;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Nodes.Actors;

namespace SatiRogue.Ecs.Play.Systems; 

public class SetInitialPositionSystem : GDSystem {
   public override void Run() {
      var mapData = GetElement<MapGenData>();
      var pathfindingHelper = GetElement<PathfindingHelper>();
      var query = Query<Character, GridPositionComponent>();

      var availableCells = mapData.IndexedCells.Where(c => !c.Value.Blocked).ToArray();
      foreach (var (character, gridPos) in query) {
         if (!availableCells.Any()) continue;
         
         var chosenCell = availableCells[(int)(GD.Randi() % availableCells.Length)];
         if (chosenCell.Value.Blocked) continue;
         gridPos.Position = chosenCell.Value.Position;
         chosenCell.Value.Occupants.Add(character.GetInstanceId());
         pathfindingHelper.SetCellWeight(chosenCell.Value.Id, chosenCell.Value.Occupants.Count);
      }
   }
}