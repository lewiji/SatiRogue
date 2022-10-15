using Godot;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.Dungeon.Components;
using SatiRogue.Ecs.Dungeon.Components.Actor;
using SatiRogue.Ecs.Dungeon.Nodes;
using SatiRogue.Ecs.Dungeon.Nodes.Actors;
using SatiRogue.Ecs.Dungeon.Nodes.Hud;
using SatiRogue.resources;
using SatiRogue.Tools;
using World = RelEcs.World;
namespace SatiRogue.Ecs.Dungeon.Systems.Init; 

public class DebugToolsSystem : Reference, ISystem {
   static readonly PackedScene DebugUiScene = GD.Load<PackedScene>("res://src/Ecs/Dungeon/Nodes/DebugUi.tscn");
   public World World { get; set; }

   public void Run() {
      var debugUi = DebugUiScene.Instance<DebugUi>();
      debugUi.Enabled = World.GetElement<SatiConfig>().DebugTools;
      World.GetElement<Hud>().GetNode("%HudItems").AddChild(debugUi);
      World.AddOrReplaceElement(debugUi);

      debugUi.Connect(nameof(DebugUi.WarpToStairs), this, nameof(OnWarpToStairs));
      debugUi.Connect(nameof(DebugUi.GodModeChanged), this, nameof(OnGodModeChanged));
      debugUi.Connect(nameof(DebugUi.HealPlayer), this, nameof(OnHealPlayer));
   }

   void OnWarpToStairs() {
      Logger.Info("Debug: Warp to stairs requested.");
      foreach (var (player, playerGridPos) in this.Query<Player, GridPositionComponent>()) {
         foreach (var (stairs, stairsGridPos) in this.Query<Stairs, GridPositionComponent>()) {
            Logger.Info($"Debug: Warping player {player.Name}@{playerGridPos.Position} to stairs {stairs}@{stairsGridPos.Position}.");
            //playerGridPos.Position = stairsGridPos.Position;
            World.GetElement<PlayerMovementSystem>().TeleportToCell(player, stairsGridPos.Position);
            return;
         }
      }
   }

   void OnGodModeChanged(bool enabled) {
      Logger.Info($"Debug: God Mode {(enabled ? "on" : "off")}.");
      foreach (var (player, playerHealth) in this.Query<Player, HealthComponent>()) {
         playerHealth.Invincible = enabled;
      }
   }

   void OnHealPlayer() {
      Logger.Info($"Debug: Player heal requested.");

      foreach (var (player, playerHealth) in this.Query<Player, HealthComponent>()) {
         playerHealth.Value = playerHealth.Max;
      }
   }
   
   
}