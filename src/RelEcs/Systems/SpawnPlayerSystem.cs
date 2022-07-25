using Godot;
using RelEcs;
using SatiRogue.Debug;
using Entity = RelEcs.Entity;

namespace SatiRogue.RelEcs.Systems; 

public class SpawnPlayerSystem : GDSystem {
   private PackedScene _playerScene = GD.Load<PackedScene>("res://src/Player/Player3d.tscn");

   public override void Run() {
      Logger.Info("Spawning Player entity");
      var playerNode = _playerScene.Instance<Nodes.Actors.Player>();
      World.GetElement<Nodes.Entities>().AddChild(playerNode);
      Entity playerEntity = Spawn(playerNode).Id();
      World.AddElement(playerNode);
   }
}