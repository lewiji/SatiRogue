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

      foreach (var (character, animationComponent) in World.Query<Character, CharacterAnimationComponent>().Build()) {
         if (!IsInstanceValid(character) || character.AnimatedSprite3D is not { } sprite || !animationComponent.HasAnimations())
            continue;
         
         if (animationComponent.PeekAnimation() != "die" && sprite.Animation != "idle" && sprite.Playing)
            continue;

         var animation = animationComponent.PopAnimation();

         if (sprite.Frames.HasAnimation(animation)) {
            sprite.Play(animation);
            Logger.Info($"Animation: {character.CharacterName} playing {animation}");
         } else {
            Logger.Info($"Animation: {character.CharacterName} has no animation called {animation}");
         }

         if (animationComponent.Animation == "die") {
            character.OnDeathAnimation();
         }
      }
   }
}