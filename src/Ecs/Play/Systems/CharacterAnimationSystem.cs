using Godot;
using RelEcs;
using SatiRogue.Ecs.Play.Components.Actor;
using SatiRogue.Ecs.Play.Nodes.Actors;

namespace SatiRogue.Ecs.Play.Systems; 

public class CharacterAnimationSystem : GDSystem {
   public override void Run() {
      foreach (var (character, inputDir) in Query<Character, InputDirectionComponent>()) {
         if (inputDir.Direction != Vector2.Zero) {
            character.AnimatedSprite3D?.Play("walk");
         }
      }
   }
}