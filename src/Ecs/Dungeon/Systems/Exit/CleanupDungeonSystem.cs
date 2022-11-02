using Godot;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.Dungeon.Components;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems.Exit;

public class CleanupDungeonSystem : ISystem {
   public World World { get; set; } = null!;

   public void Run() {
      var entitiesToCleanup = World.Query().Has<DungeonObject>().Build();
      GD.Print("Running PlayState cleanup.");

      var despawnedCount = 0;

      foreach (var entity in entitiesToCleanup) {
         World.Despawn(entity);
         despawnedCount++;
      }

      Logger.Info($"Despawned {despawnedCount} entities.");
   }
}