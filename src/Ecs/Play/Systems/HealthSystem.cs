using System;
using Godot;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.Play.Components.Actor;
using SatiRogue.Ecs.Play.Nodes.Actors;
using SatiRogue.scenes.Hud;

namespace SatiRogue.Ecs.Play.Systems; 

public class HealthSystem : GDSystem {
   public override void Run() {
      foreach (var (entity, character, health) in Query<Entity, Character, HealthComponent>())
      {
         if (health.Value <= 0) {
            Logger.Info($"HealthSystem: Entity {character} is dead!");
            DespawnAndFree(entity);
         } else if (character.StatBar3D != null && !Mathf.IsEqualApprox(character.StatBar3D.Percent, health.Percent)) {
            character.StatBar3D.Percent = health.Percent;
         }
      }
   }
}