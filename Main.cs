using Godot;
using SatiRogue.Debug;
using SatiRogue.Tools;

namespace SatiRogue; 

public class Main : Node {
    private Rng _rng = new();
    public override void _EnterTree() {
        Logger.Print("Test");
    }
}