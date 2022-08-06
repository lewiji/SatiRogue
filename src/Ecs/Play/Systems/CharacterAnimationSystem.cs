using RelEcs;
using SatiRogue.Ecs.MapGenerator.Triggers;
namespace SatiRogue.Ecs.Play.Systems;

public class CharacterAnimationSystem : GDSystem {
   public override void Run() {
      foreach (var (character, name) in Receive<CharacterAnimationTrigger>()) {
         if (!IsInstanceValid(character) || character.AnimatedSprite3D is not { } sprite) continue;
         if (sprite.Frames.HasAnimation(name)) sprite.Play(name);

         if (name == "die") { character.OnDeathAnimation(); }
      }
   }
}