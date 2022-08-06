using Godot;
using RelEcs;
using SatiRogue.Ecs.MapGenerator.Triggers;
using SatiRogue.Ecs.Play.Components.Actor;
using SatiRogue.Ecs.Play.Nodes.Actors;

namespace SatiRogue.Ecs.Play.Systems; 

public class CharacterAnimationSystem : GDSystem {
   public override void Run() {
      foreach (var (character, inputDir, healthComponent) in Query<Character, InputDirectionComponent, HealthComponent>()) {
         if (inputDir.Direction != Vector2.Zero) {
            character.AnimatedSprite3D?.Play("walk");
         }
      }

      foreach (var charDiedTrigger in Receive<CharacterDiedTrigger>()) {
         charDiedTrigger.Character.AnimatedSprite3D?.Play("die");
      }
   }
}