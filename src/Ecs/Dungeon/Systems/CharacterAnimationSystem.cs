using Godot;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.Dungeon.Components.Actor;
using SatiRogue.Ecs.Dungeon.Nodes.Actors;
using SatiRogue.Ecs.Dungeon.Triggers;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems;

public class CharacterAnimationSystem : Reference, ISystem {
   public World World { get; set; } = null!;

   public void Run() {
      PlayRequestedAnimation();
      RevertToIdleAnimation();
   }

   void PlayRequestedAnimation() {
      var counter = 0;

      foreach (var (character, name) in this.Receive<CharacterAnimationTrigger>()) {
         if (!IsInstanceValid(character) || character.AnimatedSprite3D is not { } sprite)
            continue;

         if (sprite.Frames.HasAnimation(name)) {
            sprite.Play(name);
         }

         if (name == "die") {
            character.OnDeathAnimation();
         }
         counter++;
      }
   }

   void RevertToIdleAnimation() {
      foreach (var _ in this.Receive<NewTurnTrigger>()) {
         var query = this.QueryBuilder<InputDirectionComponent>().Has<Controllable>().Build();

         foreach (var input in query) {
            if (input.Direction != Vector2.Zero)
               continue;
            var player = World.GetElement<Player>();

            if (player.AnimatedSprite3D?.Animation == "walk") {
               player.AnimatedSprite3D.Animation = "idle";
            }
         }
      }
   }
}