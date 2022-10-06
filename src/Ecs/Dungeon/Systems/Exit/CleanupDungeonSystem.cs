using Godot;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Dungeon.Components;
using SatiRogue.Ecs.Dungeon.Nodes;
using SatiRogue.Ecs.Dungeon.Nodes.Actors;
using SatiRogue.Ecs.Dungeon.Nodes.Hud;
using SatiRogue.Ecs.Dungeon.Systems.Init;
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

      World.RemoveElement<DungeonState>();
      World.RemoveElement<Entities>();
      World.RemoveElement<MapGeometry>();
      World.RemoveElement<TurnHandlerSystem>();
      World.RemoveElement<InputSystem>();
      World.RemoveElement<FogMultiMeshes>();
      World.RemoveElement<HealthUi>();
      World.RemoveElement<FloorCounter>();
      World.RemoveElement<Loot>();
      World.RemoveElement<Inventory>();
      World.RemoveElement<StairsConfirmation>();
      World.RemoveElement<DeathScreen>();
      World.RemoveElement<Player>();
      World.RemoveElement<Turn>();
      World.RemoveElement<LevelChangeSystem>();
      World.RemoveElement<AudioNodes>();
   }
}