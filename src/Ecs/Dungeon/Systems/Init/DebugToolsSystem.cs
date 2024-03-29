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
   World? _world;
   public void Run(World world)
   {
      _world ??= world;
      var debugUi = DebugUiScene.Instance<DebugUi>();
      debugUi.Enabled = world.GetElement<SatiConfig>().DebugTools;
      world.GetElement<Hud>().GetNode("%HudItems").AddChild(debugUi);
      world.AddOrReplaceElement(debugUi);

      debugUi.Connect(nameof(DebugUi.WarpToStairs), this, nameof(OnWarpToStairs));
      debugUi.Connect(nameof(DebugUi.GodModeChanged), this, nameof(OnGodModeChanged));
      debugUi.Connect(nameof(DebugUi.HealPlayer), this, nameof(OnHealPlayer));
   }

   void OnWarpToStairs() {
      Logger.Info("Debug: Warp to stairs requested.");
      foreach (var (player, playerGridPos) in _world!.Query<Player, GridPositionComponent>().Build()) {
         foreach (var (stairs, stairsGridPos) in _world!.Query<Stairs, GridPositionComponent>().Build()) {
            Logger.Info($"Debug: Warping player {player.Name}@{playerGridPos.Position} to stairs {stairs}@{stairsGridPos.Position}.");
            //playerGridPos.Position = stairsGridPos.Position;
            _world!.GetElement<PlayerMovementSystem>().TeleportToCell(player, stairsGridPos.Position);
            return;
         }
      }
   }

   void OnGodModeChanged(bool enabled) {
      Logger.Info($"Debug: God Mode {(enabled ? "on" : "off")}.");
      foreach (var (player, playerHealth) in _world!.Query<Player, HealthComponent>().Build()) {
         playerHealth.Invincible = enabled;
      }
   }

   void OnHealPlayer() {
      Logger.Info($"Debug: Player heal requested.");

      foreach (var (player, playerHealth) in _world!.Query<Player, HealthComponent>().Build()) {
         playerHealth.Value = playerHealth.Max;
      }
   }
   
   
}