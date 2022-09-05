using Godot;
using SatiRogue.Debug;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Play.Nodes.Actors;
using SatiRogue.lib.RelEcsGodot.src;
namespace SatiRogue.Ecs.Play.Systems.Init;

public class SpawnPlayerSystem : GdSystem {
   readonly PackedScene _playerScene = GD.Load<PackedScene>("res://src/Ecs/Play/Nodes/Actors/Player3d.tscn");

   public override void Run() {
      Logger.Info("Spawning Player entity");
      var playerNode = _playerScene.Instance<Player>();
      World.GetElement<Entities>().AddChild(playerNode);
      Spawn(playerNode).Id();
      World.AddElement(playerNode);
   }
}