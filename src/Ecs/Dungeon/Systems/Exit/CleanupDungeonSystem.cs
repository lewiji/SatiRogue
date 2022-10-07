using Godot;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.Dungeon.Components;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems.Exit;

public class CleanupDungeonSystem : ISystem {
   public World World { get; set; } = null!;

   public void Run() {
      var entitiesToCleanup = this.QueryBuilder().Has<DungeonObject>().Build();
      GD.Print("Running PlayState cleanup.");

      var despawnedCount = 0;

      foreach (var entity in entitiesToCleanup) {
         this.Despawn(entity);
         despawnedCount++;
      }

      Logger.Info($"Despawned {despawnedCount} entities.");
   }
}