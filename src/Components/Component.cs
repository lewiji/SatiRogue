using SatiRogue.Entities;

namespace SatiRogue.Components;

public abstract partial class Component : GameObject, IComponent {
   public override GameObject? EcOwner => base.EcOwner as Entity;

   public override void _EnterTree() {
      base._EnterTree();
      Name = "Component";
   }
}