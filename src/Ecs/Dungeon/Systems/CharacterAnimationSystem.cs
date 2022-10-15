using Godot;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.Dungeon.Components;
using SatiRogue.Ecs.Dungeon.Components.Actor;
using SatiRogue.Ecs.Dungeon.Nodes.Actors;
using SatiRogue.Ecs.Dungeon.Triggers;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems;

public class CharacterAnimationSystem : Reference, ISystem {
   public World World { get; set; } = null!;

   Turn? _turn;

   public void Run() {
      _turn ??= World.GetElement<Turn>();
      PlayRequestedAnimation();
   }

   void PlayRequestedAnimation() {

      foreach (var (character, animationComponent) in this.Query<Character, CharacterAnimationComponent>()) {
         if (!IsInstanceValid(character) || character.AnimatedSprite3D is not { } sprite || animationComponent.Animation == "")
            continue;

         if (sprite.Frames.HasAnimation(animationComponent.Animation)) {
            sprite.Play(animationComponent.Animation);
         }

         if (animationComponent.Animation == "die") {
            character.OnDeathAnimation();
         }

         animationComponent.Animation = "";
      }
   }
}