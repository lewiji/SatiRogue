 
using Godot;
using RelEcs;
using SatiRogue.Ecs.Play.Components.Actor;
using SatiRogue.Ecs.Play.Nodes.Actors;

namespace SatiRogue.Ecs.Play.Systems;


public class ResetInputDirectionSystem : GDSystem {
   public override void Run() {
      foreach (var input in Query<InputDirectionComponent>()) {
         input.Direction = Vector2.Zero;
      }
   }
}