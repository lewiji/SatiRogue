using Godot;

namespace SatiRogue.addons.wfc_tools.src; 

// ReSharper disable once ClassNeverInstantiated.Global
public class WfcSignals : Node {
    public static void Emit(string signalName, object[]? args = null) => _instance?.EmitSignal(signalName, args);
        
    [Signal] public delegate void EnableFolderButton();
    
    public static WfcSignals? GetInstance() => _instance;
    private static WfcSignals? _instance;
    public override void _EnterTree() { _instance = this; }
    public override void _ExitTree() { _instance = null; }
}