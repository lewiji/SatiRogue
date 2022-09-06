using SatiRogue.Debug;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Nodes;
using SatiRogue.lib.RelEcsGodot.src;
namespace SatiRogue.Ecs.Play.Systems.Exit;

public class CleanupDungeonSystem : GdSystem {
   public override void Run() {
      var entitiesToCleanup = QueryBuilder().Has<DungeonObject>().Build();
      Logger.Info("Running PlayState cleanup.");

      var despawnedCount = 0;

      foreach (var entity in entitiesToCleanup) {
         Despawn(entity);
         despawnedCount++;
      }

      Logger.Info($"Despawned {despawnedCount} entities.");
   }
}