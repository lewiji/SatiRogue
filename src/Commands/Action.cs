using Godot;
using SatiRogue.Entities;

namespace SatiRogue.Commands;

public abstract class Action : Command {
   protected Entity? Owner;
   protected GameObject? Target;

   public Action(Entity? owner, GameObject? target = null) {
      Owner = owner;
      Target = target;
   }

   public bool IsOwnerEnabled() {
      return Owner?.Enabled ?? false;
   }
}