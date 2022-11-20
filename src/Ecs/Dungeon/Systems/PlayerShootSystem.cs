using Godot;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Dungeon.Components;
using SatiRogue.Ecs.Dungeon.Components.Actor;
using SatiRogue.Ecs.Dungeon.Nodes.Actors;
using SatiRogue.Ecs.Dungeon.Nodes.Items;
using SatiRogue.Ecs.Dungeon.Triggers;
using SatiRogue.Ecs.MapGenerator.Components;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems;

public class PlayerShootSystem : ISystem {
   
   readonly PackedScene _arrowScene = GD.Load<PackedScene>("res://src/Ecs/Dungeon/Nodes/Items/Arrow.tscn");

   public void Run(World world) {
      foreach (var _ in world.Receive<PlayerHasShotTrigger>(this)) {
         foreach (var (_, gridPos, input) in world.Query<Player, GridPositionComponent, InputDirectionComponent>().Build()) {
            var direction = new Vector2(1, 0);

            if (input.LastDirection != Vector2.Zero) {
               direction = input.LastDirection;
            }
            Logger.Debug($"Firing {direction}");
            var arrow = _arrowScene.Instance<Arrow>();
            var entitiesNode = world.GetElement<Entities>();
            entitiesNode.AddChild(arrow);
            var arrowEntity = world.Spawn(arrow).Id();

            var arrowGridPos = new GridPositionComponent {
               Position = gridPos.Position + new Vector3(input.Direction.x, 0, input.Direction.y)
            };
            world.On(arrowEntity).Add(new InputDirectionComponent {Direction = direction}).Add(arrowGridPos);

            arrow.Direction = direction;
            arrow.Translation = arrowGridPos.Position;

            var mapData = world.GetElement<MapGenData>();

            for (var shootDistance = 0; shootDistance < arrow.Range; shootDistance++) {
               var cell = mapData.GetCellAt(arrowGridPos.Position + new Vector3(direction.x, 0, direction.y) * shootDistance);

               if (cell.Type is CellType.Wall or CellType.DoorClosed) {
                  arrow.Destination = arrowGridPos.Position + new Vector3(direction.x, 0, direction.y) * shootDistance;
               } else if (cell.Occupants.Count > 0) {
                  foreach (var cellOccupant in cell.Occupants) {
                     if (GD.InstanceFromId(cellOccupant) is not Enemy enemy || !enemy.Alive)
                        continue;
                     var entity = enemy.GetMeta("Entity") as Marshallable<Entity>;
                     world.GetComponent<HealthComponent>(entity?.Value!).Value -= 1;

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