using Godot;
using RelEcs;
using SatiRogue.Debug;
using SatiRogue.Entities;
using Entity = RelEcs.Entity;

namespace SatiRogue.RelEcs.Systems; 

public class SpawnPlayerSystem : GodotSystem {
   private PackedScene _playerScene = GD.Load<PackedScene>("res://src/Player/Player3d.tscn");
   public override void Run() {
      Logger.Info("Spawning Player entity");
      var playerNode = _playerScene.Instance<Nodes.Actors.Player>();
      Entity playerEntity = Spawn(playerNode).Id();
   }
}