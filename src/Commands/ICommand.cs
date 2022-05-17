using Godot;

namespace SatiRogue.Commands; 

public interface ICommand {
   Error Execute();
}