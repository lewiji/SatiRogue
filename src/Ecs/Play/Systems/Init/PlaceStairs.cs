using Godot;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.MapGenerator.Components;
using SatiRogue.Ecs.Play.Nodes;
using SatiRogue.lib.RelEcsGodot.src;
namespace SatiRogue.Ecs.Play.Systems.Init;

public class PlaceStairs : GdSystem {
   static readonly PackedScene StairsScene = GD.Load<PackedScene>("res://src/Ecs/Play/Nodes/Stairs.tscn");

   public override void Run() {
      var mapGenData = GetElement<MapGenData>();
      var entitiesNode = World.GetElement<Entities>();
      var stairsNode = StairsScene.Instance<Stairs>();
      entitiesNode.AddChild(stairsNode);
      Spawn(stairsNode);
   }
}