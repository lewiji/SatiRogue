using Godot;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Dungeon.Nodes.Actors;
using SatiRogue.Tools;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Dungeon.Systems.Init;

public class SpawnPlayerSystem : ISystem {
   
   readonly PackedScene _playerScene = GD.Load<PackedScene>("res://src/Ecs/Dungeon/Nodes/Actors/Player3d.tscn");

   public void Run(World world) {
      Logger.Info("Spawning Player entity");
      var playerNode = _playerScene.Instance<Player>();
      world.GetElement<Entities>().AddChild(playerNode);
      var playerEntity = world.Spawn(playerNode);
      world.AddOrReplaceElement(playerNode);
   }
}