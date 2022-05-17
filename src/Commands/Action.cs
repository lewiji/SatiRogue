using SatiRogue.Entities;

namespace SatiRogue.Commands;

public abstract class Action : AbstractCommand {
   protected Entity? Owner;
   protected Entity? Target;

   public Action(Entity? owner, Entity? target = null) {
      Owner = owner;
      Target = target;
   }
}