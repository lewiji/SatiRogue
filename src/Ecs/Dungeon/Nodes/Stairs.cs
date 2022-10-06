using Godot;
using SatiRogue.Ecs.Play.Components;
using RelEcs;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Play.Nodes;

public partial class Stairs : GameObject {
   public enum StairsDirection {
      Up = 0,
      Down = 1
   }

   public StairsDirection Direction = StairsDirection.Down;

   public Stairs() {
      BlocksCell = false;
   }

   public override void OnSpawn(EntityBuilder entityBuilder) {
      entityBuilder.Add(new GridPositionComponent());
   }
}