using Godot;

namespace SatiRogue.Commands;

public abstract class AbstractCommand {
   public abstract Error Execute();
}