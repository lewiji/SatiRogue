using System.Linq;
using Godot;
using SatiRogue.Debug;
using SatiRogue.Ecs.MapGenerator.Components;
using SatiRogue.Ecs.Play.Components;
using RelEcs;

namespace SatiRogue.Ecs.Play.Systems; 

public class SpawnPlayerSystem : GDSystem {
   private PackedScene _playerScene = GD.Load<PackedScene>("res://src/Player/Player3d.tscn");

   public override void Run() {
      Logger.Info("Spawning Player entity");
      var playerNode = _playerScene.Instance<Nodes.Actors.Player>();
      World.GetElement<Core.Entities>().AddChild(playerNode);
      var playerEntity = Spawn(playerNode).Id();
      World.AddElement(playerNode);
   }
}