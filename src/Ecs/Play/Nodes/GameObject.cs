using Godot;
using SatiRogue.Ecs.Play.Components;
using SatiRogue.lib.RelEcsGodot.src;
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