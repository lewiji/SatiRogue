using Godot;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.MapGenerator.Components;
using SatiRogue.Ecs.Play.Nodes;
using RelEcs;
using World = RelEcs.World;
namespace SatiRogue.Ecs.Play.Systems.Init;

public class PlaceStairs : ISystem {
   public World World { get; set; } = null!;
   static readonly PackedScene StairsScene = GD.Load<PackedScene>("res://src/Ecs/Dungeon/Nodes/Stairs.tscn");

   public void Run() {
      var mapGenData = World.GetElement<MapGenData>();
      var entitiesNode = World.GetElement<Entities>();
      var stairsNode = StairsScene.Instance<Stairs>();
      entitiesNode.AddChild(stairsNode);
      this.Spawn(stairsNode);
   }
}