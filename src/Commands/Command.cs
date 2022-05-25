using Godot;

namespace SatiRogue.Commands;

public abstract class Command : ICommand {
   public abstract Error Execute();
}