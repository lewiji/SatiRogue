using Godot;
using RelEcs;
using SatiRogue.Ecs.MapGenerator.Triggers;
using SatiRogue.Ecs.Play.Components.Actor;
using SatiRogue.Ecs.Play.Nodes.Actors;
using Object = Godot.Object;
namespace SatiRogue.Ecs.Play.Systems;

public class CharacterAnimationSystem : GdSystem {
   public override void Run() {
      PlayRequestedAnimation();
      RevertToIdleAnimation();
   }

   private void PlayRequestedAnimation() {
      foreach (var (character, name) in Receive<CharacterAnimationTrigger>()) {
         if (!IsInstanceValid(character) || character.AnimatedSprite3D is not { } sprite) continue;

         if (sprite.Frames.HasAnimation(name)) {
            sprite.Play(name);
         }

         if (name == "die") {
            character.OnDeathAnimation();
         }
      }
   }

   private void RevertToIdleAnimation() {
      foreach (var newTurn in Receive<NewTurnTrigger>()) {
         var query = QueryBuilder<InputDirectionComponent>().Has<Controllable>().Build();

         foreach (var input in query) {
            if (input.Direction != Vector2.Zero) continue;
            var player = GetElement<Nodes.Actors.Player>();

            if (player.AnimatedSprite3D?.Animation == "walk") {
               player.AnimatedSprite3D.Animation = "idle";
            }
         }
      }
   }
}