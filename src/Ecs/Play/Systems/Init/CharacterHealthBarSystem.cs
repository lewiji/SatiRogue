using Godot;
using RelEcs;
using SatiRogue.Ecs.Play.Components.Actor;
using SatiRogue.Ecs.Play.Nodes.Actors;
using SatiRogue.scenes.Hud;

namespace SatiRogue.Ecs.Play.Systems.Init; 

public class CharacterHealthBarSystem : GDSystem {
   private static readonly PackedScene HealthBarScene = GD.Load<PackedScene>("res://scenes/Hud/StatBar3D.tscn");
   public override void Run() {
      var query = Query<Root, Character, HealthComponent>();

      foreach (var (rootNode, character, health) in query) {
         var healthBarNode = HealthBarScene.Instance<StatBar3D>();
         character.AddChild(healthBarNode);
         character.StatBar3D = healthBarNode;
         //healthBarNode.Percent = health.Percent;
      }
   }
}