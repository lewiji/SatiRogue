using Godot;
using RelEcs;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Dungeon.Nodes;
using SatiRogue.Ecs.MapGenerator.Components;
using World = RelEcs.World;
namespace SatiRogue.Ecs.Dungeon.Systems.Init;

public class PlaceStairs : ISystem {
   
   static readonly PackedScene StairsScene = GD.Load<PackedScene>("res://src/Ecs/Dungeon/Nodes/Stairs.tscn");

   public void Run(World world) {
      var mapGenData = world.GetElement<MapGenData>();
      var entitiesNode = world.GetElement<Entities>();
      var stairsNode = StairsScene.Instance<Stairs>();
      entitiesNode.AddChild(stairsNode);
      world.Spawn(stairsNode);
   }
}