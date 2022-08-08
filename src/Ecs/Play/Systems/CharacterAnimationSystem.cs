using Godot;
using RelEcs;
using SatiRogue.Ecs.MapGenerator.Triggers;
using SatiRogue.Ecs.Play.Components.Actor;
namespace SatiRogue.Ecs.Play.Systems;

public class CharacterAnimationSystem : GDSystem {
   public override void Run() {
      foreach (var (character, name) in Receive<CharacterAnimationTrigger>()) {
         if (!IsInstanceValid(character) || character.AnimatedSprite3D is not { } sprite) continue;

         if (sprite.Frames.HasAnimation(name)) {
            sprite.Play(name);

            if (character.WallPeekSprite != null) {
               character.WallPeekSprite.Play(name);
            }
         }

         if (name == "die") {
            character.OnDeathAnimation();
         }
      }

      foreach (var newTurn in Receive<NewTurnTrigger>()) {
         var query = QueryBuilder<InputDirectionComponent>().Has<Controllable>().Build();

         foreach (var input in query) {
            if (input.Direction != Vector2.Zero) continue;
            var player = GetElement<Nodes.Actors.Player>();

            if (player.AnimatedSprite3D?.Animation == "walk") {
               player.AnimatedSprite3D.Animation = "idle";

               if (player.WallPeekSprite != null) {
                  player.WallPeekSprite.Animation = "idle";
               }
            }
         }
      }
   }
}