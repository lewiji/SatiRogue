using Godot;

namespace SatiRogue.Commands;

public abstract class AbstractCommand : ICommand {
   public abstract Error Execute();
}