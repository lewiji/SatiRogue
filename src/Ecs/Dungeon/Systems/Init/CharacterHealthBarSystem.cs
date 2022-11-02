using Godot;
using RelEcs;
using SatiRogue.Ecs.Dungeon.Components.Actor;
using SatiRogue.Ecs.Dungeon.Nodes.Actors;
using SatiRogue.scenes.Hud;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems.Init;

public class CharacterHealthBarSystem : ISystem {
   public World World { get; set; } = null!;
   static readonly PackedScene HealthBarScene = GD.Load<PackedScene>("res://scenes/Hud/StatBar3D.tscn");

   public void Run() {
      var query = World.Query<Entity, Character, HealthComponent>();

      foreach (var (entity, character, health) in query.Build()) {
         var healthBarNode = HealthBarScene.Instance<StatBar3D>();
         character.AddChild(healthBarNode);
         //World.On(entity).Add(healthBarNode);
         GodotExtensions.AddNodeComponent(World, entity, healthBarNode);
         healthBarNode.Percent = health.Percent;

         if (character is Player) {
            healthBarNode.Hidden = false;
         }
      }
   }
}