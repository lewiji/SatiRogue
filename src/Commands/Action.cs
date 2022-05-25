namespace SatiRogue.Commands;

public abstract class Action : Command {
   protected GameObject? Owner;
   protected GameObject? Target;

   public Action(GameObject? owner, GameObject? target = null) {
      Owner = owner;
      Target = target;
   }
}