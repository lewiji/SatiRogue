using Godot;
using RelEcs;
using SatiRogue.Debug;

namespace SatiRogue.Ecs.Play.Systems.Init;

public class SpawnPlayerSystem : GdSystem {
   readonly PackedScene _playerScene = GD.Load<PackedScene>("res://src/Ecs/Play/Nodes/Actors/Player3d.tscn");

   public override void Run() {
      Logger.Info("Spawning Player entity");
      var playerNode = _playerScene.Instance<Nodes.Actors.Player>();
      World.GetElement<Core.Entities>().AddChild(playerNode);
      var playerEntity = Spawn(playerNode).Id();
      World.AddElement(playerNode);
   }
}