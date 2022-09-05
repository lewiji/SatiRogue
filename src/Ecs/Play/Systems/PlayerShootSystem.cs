using Godot;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.MapGenerator.Components;
using SatiRogue.Ecs.MapGenerator.Triggers;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Components.Actor;
using SatiRogue.Ecs.Play.Nodes.Actors;
using SatiRogue.Ecs.Play.Nodes.Items;
namespace SatiRogue.Ecs.Play.Systems;

public class PlayerShootSystem : GdSystem {
   readonly PackedScene _arrowScene = GD.Load<PackedScene>("res://src/Ecs/Play/Nodes/Items/Arrow.tscn");

   public override void Run() {
      foreach (var shot in Receive<PlayerHasShotTrigger>()) {
         foreach (var (player, gridPos, input) in Query<Player, GridPositionComponent, InputDirectionComponent>()) {
            var direction = new Vector2(1, 0);

            if (input.LastDirection != Vector2.Zero) {
               direction = input.LastDirection;
            }
            Logger.Debug($"Firing {direction}");
            var arrow = _arrowScene.Instance<Arrow>();
            var entitiesNode = World.GetElement<Core.Entities>();
            entitiesNode.AddChild(arrow);
            var arrowEntity = Spawn(arrow).Id();

            var arrowGridPos = new GridPositionComponent
               {Position = gridPos.Position + new Vector3(input.Direction.x, 0, input.Direction.y)};
            On(arrowEntity).Add(new InputDirectionComponent {Direction = direction}).Add(arrowGridPos);

            arrow.Direction = direction;
            arrow.Translation = arrowGridPos.Position;

            var mapData = GetElement<MapGenData>();

            for (var shootDistance = 0; shootDistance < arrow.Range; shootDistance++) {
               var cell = mapData.GetCellAt(arrowGridPos.Position + new Vector3(direction.x, 0, direction.y) * shootDistance);

               if (cell.Type is CellType.Wall or CellType.DoorClosed) {
                  arrow.Destination = arrowGridPos.Position + new Vector3(direction.x, 0, direction.y) * shootDistance;
               } else if (cell.Occupants.Count > 0) {
                  foreach (var cellOccupant in cell.Occupants) {
                     if (GD.InstanceFromId(cellOccupant) is not Enemy enemy || !enemy.Alive) continue;
                     var entity = enemy.GetMeta("Entity") as Entity;
                     GetComponent<HealthComponent>(entity!).Value -= 1;

                     if (arrow.Destination == Vector3.Zero) {
                        arrow.Destination = cell.Position;
                     }
                  }
               }
            }

            if (arrow.Destination == Vector3.Zero) {
               arrow.Destination = arrowGridPos.Position + new Vector3(direction.x, 0, direction.y) * arrow.Range;
            }
         }
      }
   }
}