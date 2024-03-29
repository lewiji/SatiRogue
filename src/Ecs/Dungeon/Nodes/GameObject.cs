using Godot;
using RelEcs;
using SatiRogue.Ecs.Dungeon.Components;
namespace SatiRogue.Ecs.Dungeon.Nodes;

public abstract class GameObject : Spatial, ISpawnable {
   [Export] public bool BlocksCell = true;
   [Export] public bool Enabled = true;

   public void Spawn(EntityBuilder entityBuilder) {
      entityBuilder.Add<DungeonObject>();
      OnSpawn(entityBuilder);
   }

   public abstract void OnSpawn(EntityBuilder entityBuilder);
}