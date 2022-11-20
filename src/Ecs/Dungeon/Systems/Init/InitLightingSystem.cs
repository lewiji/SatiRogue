using Godot;
using RelEcs;
using SatiRogue.Ecs.Dungeon.Nodes;
using World = RelEcs.World;
namespace SatiRogue.Ecs.Dungeon.Systems.Init; 

public class InitLightingSystem : ISystem {
   
   static readonly PackedScene LightingScene = GD.Load<PackedScene>("res://src/Ecs/Dungeon/Nodes/DungeonDirectionalLight.tscn");

   public void Run(World world) {
      var mapGeometry = world.GetElement<MapGeometry>();
      var lighting = LightingScene.Instance<DungeonDirectionalLight>();
      mapGeometry.AddChild(lighting);
   }
}