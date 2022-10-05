using Godot;
using SatiRogue.Ecs.Play.Components.Actor;
using SatiRogue.Ecs.Play.Nodes.Actors;
using RelEcs;
using World = RelEcs.World;
using SatiRogue.scenes.Hud;

namespace SatiRogue.Ecs.Play.Systems.Init;

public class CharacterHealthBarSystem : ISystem {
   public World World { get; set; } = null!;
   static readonly PackedScene HealthBarScene = GD.Load<PackedScene>("res://scenes/Hud/StatBar3D.tscn");

   public void Run() {
      var query = this.Query<Entity, Character, HealthComponent>();

      foreach (var (entity, character, health) in query) {
         var healthBarNode = HealthBarScene.Instance<StatBar3D>();
         character.AddChild(healthBarNode);
         //this.On(entity).Add(healthBarNode);
         GodotExtensions.AddNodeComponent(World, entity, healthBarNode);
         healthBarNode.Percent = health.Percent;

         if (character is Player) {
            healthBarNode.Hidden = false;
         }
      }
   }
}