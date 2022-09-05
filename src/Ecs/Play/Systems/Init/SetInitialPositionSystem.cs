using System.Linq;
using Godot;
using SatiRogue.Ecs.MapGenerator.Components;
using SatiRogue.Ecs.MapGenerator.Systems;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Nodes.Actors;
using SatiRogue.Ecs.Play.Nodes.Items;
using SatiRogue.lib.RelEcsGodot.src;
namespace SatiRogue.Ecs.Play.Systems.Init;

public class SetInitialPositionSystem : GdSystem {
   public override void Run() {
      var mapData = GetElement<MapGenData>();
      var pathfindingHelper = GetElement<PathfindingHelper>();
      var query = Query<Character, GridPositionComponent>();

      var availableCells = mapData.IndexedCells.Where(c => !c.Value.Blocked).ToArray();

      foreach (var (character, gridPos) in query) {
         if (!availableCells.Any()) break;
         var chosenCell = availableCells[(int) (GD.Randi() % availableCells.Length)];

         while (chosenCell.Value.Blocked) {
            chosenCell = availableCells[(int) (GD.Randi() % availableCells.Length)];
         }

         gridPos.Position = chosenCell.Value.Position;
         chosenCell.Value.Occupants.Add(character.GetInstanceId());
         pathfindingHelper.SetCellWeight(chosenCell.Value.Id, chosenCell.Value.Occupants.Count);
      }

      var itemQuery = Query<Item, GridPositionComponent>();

      foreach (var (item, gridPos) in itemQuery) {
         if (!availableCells.Any()) break;
         var chosenCell = availableCells[(int) (GD.Randi() % availableCells.Length)];

         while (chosenCell.Value.Blocked) {
            chosenCell = availableCells[(int) (GD.Randi() % availableCells.Length)];
         }
         gridPos.Position = chosenCell.Value.Position;
         chosenCell.Value.Occupants.Add(item.GetInstanceId());
         pathfindingHelper.SetCellWeight(chosenCell.Value.Id, chosenCell.Value.Occupants.Count);
         item.Translation = gridPos.Position;
      }
   }
}