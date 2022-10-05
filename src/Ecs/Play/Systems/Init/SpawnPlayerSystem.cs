using Godot;
using SatiRogue.Debug;
using SatiRogue.Ecs.Core;
using SatiRogue.Ecs.Play.Nodes.Actors;
using RelEcs;
using SatiRogue.Ecs.Core.Nodes;
using SatiRogue.Ecs.Play.Components.Actor;
using World = RelEcs.World;

namespace SatiRogue.Ecs.Play.Systems.Init;

public class SpawnPlayerSystem : ISystem {
   public World World { get; set; } = null!;
   readonly PackedScene _playerScene = GD.Load<PackedScene>("res://src/Ecs/Play/Nodes/Actors/Player3d.tscn");

   public void Run() {
      Logger.Info("Spawning Player entity");
      var playerNode = _playerScene.Instance<Player>();
      World.GetElement<Entities>().AddChild(playerNode);
      var playerEntity = this.Spawn(playerNode).Id();
      World.AddElement(playerNode);
   }
}