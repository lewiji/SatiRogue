using Godot;
using SatiRogue.Ecs.Play.Components;
using RelEcs;
using World = RelEcs.World;
namespace SatiRogue.Ecs.Play.Nodes;

public abstract class GameObject : Spatial, ISpawnable {
   [Export] public bool BlocksCell = true;
   [Export] public bool Enabled = true;

   public void Spawn(EntityBuilder entityBuilder) {
      entityBuilder.Add<DungeonObject>();
      OnSpawn(entityBuilder);
   }

   public abstract void OnSpawn(EntityBuilder entityBuilder);
}