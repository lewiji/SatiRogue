using Godot;

namespace RoguelikeMono.addons.wfc_tools;

[Tool]
public class WfcTools : EditorPlugin
{
    private Control? _dock;
    
    public override void _EnterTree()
    {
        _dock = GD.Load<PackedScene>("res://addons/wfc_tools/scenes/wfc_dock.tscn").Instance<Control>();
        AddControlToBottomPanel(_dock, "WfcTools");
    }

    public override void _ExitTree()
    {
        if (_dock == null) return;
        RemoveControlFromBottomPanel(_dock);
        _dock.Free();
    }
}