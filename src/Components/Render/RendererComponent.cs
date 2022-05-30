using GodotOnReady.Attributes;
using SatiRogue.Entities;

namespace SatiRogue.Components.Render; 

public abstract class RendererComponent : Component {
   public Entity? Entity => EcOwner as Entity;

   public override void _Ready() {
      CallDeferred(nameof(CreateVisualNodes));
   }

   protected abstract void CreateVisualNodes();
}