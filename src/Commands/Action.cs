using SatiRogue.Entities;

namespace SatiRogue.Commands;

public abstract class Action : AbstractCommand {
   protected EntityData Owner;
   protected EntityData? Target;

   public Action(EntityData owner, EntityData? target = null) {
      Owner = owner;
      Target = target;
   }
}