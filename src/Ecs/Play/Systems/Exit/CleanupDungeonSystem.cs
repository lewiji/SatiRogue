using Godot;
using SatiRogue.Debug;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.Ecs.Play.Nodes;
using RelEcs;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Play.Nodes.Actors;
using SatiRogue.Ecs.Play.Nodes.Hud;
using SatiRogue.Ecs.Play.Systems.Init;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Play.Systems.Exit;

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

      World.RemoveElement<PlayState>();
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