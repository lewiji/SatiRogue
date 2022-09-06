using Godot;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.lib.RelEcsGodot.src;
namespace SatiRogue.Ecs.Play.Nodes;

public partial class Stairs : GameObject {
   public enum StairsDirection { Up = 0, Down = 1 }
   public StairsDirection Direction = StairsDirection.Down;

   public Stairs() {
      BlocksCell = false;
   }

   public override void Spawn(EntityBuilder entityBuilder) {
      entityBuilder.Add(this).Add(new GridPositionComponent());
   }
}