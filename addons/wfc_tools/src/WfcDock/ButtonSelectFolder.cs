using Godot;
using GodotOnReady.Attributes;

namespace RoguelikeMono.addons.wfc_tools.src.WfcDock;

[Tool]
public partial class ButtonSelectFolder : Button
{
    [Signal] public delegate void FolderSelected(string path);
    
    [OnReadyGet("FileDialog")] private FileDialog _fileDialog;
    [OnReady] private void ConnectPressedSignal()
    {
        Connect("pressed", this, nameof(OnPressed));
        _fileDialog.Connect("dir_selected", this, nameof(OnDirSelected));
    }

    private void OnPressed()
    {
        _fileDialog.PopupCentered();
    }

    private void OnDirSelected(string dir)
    {
        _fileDialog.Hide();
        GD.Print($"Selected dir {dir}");
        EmitSignal(nameof(FolderSelected), dir);
        Disabled = true;
    }
}