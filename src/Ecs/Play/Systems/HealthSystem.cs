using System;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.Play.Components.Actor;
using SatiRogue.Ecs.Play.Nodes.Actors;

namespace SatiRogue.Ecs.Play.Systems; 

public class HealthSystem : GDSystem {
   public override void Run() {
      foreach (var (entity, health) in Query<Entity, HealthComponent>())
      {
         if (health.Value <= 0) {
            Logger.Info("Dead!!!");
            DespawnAndFree(entity);
         }
      }
   }
}