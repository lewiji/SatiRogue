using Godot;
using RelEcs;
namespace SatiRogue.Ecs.Play.Nodes;

public abstract class GameObject : Spatial, ISpawnable {
   [Export] public bool BlocksCell = true;
   [Export] public bool Enabled = true;
   public virtual void Spawn(EntityBuilder entityBuilder) { }
}