using Godot;
using GodotOnReady.Attributes;
using SatiRogue.addons.wfc_tools.src.WfcDock;

namespace SatiRogue.addons.src.WfcDock;

[Tool]
public partial class PreviewGridContainer : GridContainer {
    [Signal] public delegate void LoadMoreMeshes();
    [Signal] public delegate void ResourceSelected(int index);
    
    public RID? NewSpaceRid;

    [OnReadyGet] private Control? _mainDockControl;
    [OnReadyGet] public ResourcePanel? ResourcesPanel;

    [OnReady] private void CreateNewPhysicsSpace() {
        NewSpaceRid = Physics2DServer.SpaceCreate();
    }

    [OnReady] private void ConnectResized() {
        _mainDockControl?.Connect("resized", this, nameof(OnResized));
    }

    public void OnResized() {
        if (GetChildCount() == 0) return;
        if (_mainDockControl == null) return;
        var numColumns = (int) ((_mainDockControl.RectSize.x - 96) / GetChild<ResourcePreviewContainer>(0).RectSize.x);
        Columns = numColumns;
    }
}