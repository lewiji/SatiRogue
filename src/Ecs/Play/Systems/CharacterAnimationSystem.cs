using Godot;
using SatiRogue.Debug;
using SatiRogue.Ecs.Play.Components.Actor;
using SatiRogue.Ecs.Play.Nodes.Actors;
using SatiRogue.Ecs.Play.Triggers;
using SatiRogue.lib.RelEcsGodot.src;
namespace SatiRogue.Ecs.Play.Systems;

public class CharacterAnimationSystem : GdSystem {
   public override void Run() {
      PlayRequestedAnimation();
      RevertToIdleAnimation();
   }

   void PlayRequestedAnimation() {
      var counter = 0;

      foreach (var (character, name) in Receive<CharacterAnimationTrigger>()) {
         if (!IsInstanceValid(character) || character.AnimatedSprite3D is not { } sprite) continue;

         if (sprite.Frames.HasAnimation(name)) {
            sprite.Play(name);
         }

         if (name == "die") {
            character.OnDeathAnimation();
         }
         counter++;
      }
      if (counter > 0) Logger.Info($"{counter} animations received");
   }

   void RevertToIdleAnimation() {
      foreach (var _ in Receive<NewTurnTrigger>()) {
         var query = QueryBuilder<InputDirectionComponent>().Has<Controllable>().Build();

         foreach (var input in query) {
            if (input.Direction != Vector2.Zero) continue;
            var player = GetElement<Player>();

            if (player.AnimatedSprite3D?.Animation == "walk") {
               player.AnimatedSprite3D.Animation = "idle";
            }
         }
      }
   }
}