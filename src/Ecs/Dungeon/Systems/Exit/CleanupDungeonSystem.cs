using Godot;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.Dungeon.Components;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems.Exit;

public class CleanupDungeonSystem : ISystem {
   public void Run(World world) {
      var entitiesToCleanup = world.Query().Has<DungeonObject>().Build();
      GD.Print("Running PlayState cleanup.");

      var despawnedCount = 0;

      foreach (var entity in entitiesToCleanup) {
         world.Despawn(entity);
         despawnedCount++;
      }

      Logger.Info($"Despawned {despawnedCount} entities.");
   }
}