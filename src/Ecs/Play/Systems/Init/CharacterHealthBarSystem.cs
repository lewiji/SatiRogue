using Godot;
using SatiRogue.Ecs.Play.Components.Actor;
using SatiRogue.Ecs.Play.Nodes.Actors;
using SatiRogue.lib.RelEcsGodot.src;
using SatiRogue.scenes.Hud;
namespace SatiRogue.Ecs.Play.Systems.Init;

public class CharacterHealthBarSystem : GdSystem {
   static readonly PackedScene HealthBarScene = GD.Load<PackedScene>("res://scenes/Hud/StatBar3D.tscn");

   public override void Run() {
      var query = Query<Entity, Character, HealthComponent>();

      foreach (var (entity, character, health) in query) {
         var healthBarNode = HealthBarScene.Instance<StatBar3D>();
         character.AddChild(healthBarNode);
         On(entity).Add(healthBarNode);
         healthBarNode.Percent = health.Percent;

         if (character is Player) {
            healthBarNode.Hidden = true;
         }
      }
   }
}