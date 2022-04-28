using Godot;
using GodotOnReady.Attributes;

namespace RoguelikeMono.addons.wfc_tools.src.WfcDock;

[Tool]
public partial class MeshPreviewGridContainer : GridContainer
{
    [Signal] public delegate void MeshSelected(int index);

    [Signal] public delegate void LoadMoreMeshes();

    public RID? NewPhysicsSpaceRid;
    public RID? OldPhysicsSpaceRid;
    
    [OnReadyGet] private Control? _mainDockControl;
    [OnReady] private void ConnectResized()
    {
        _mainDockControl?.Connect("resized", this, nameof(OnResized));
    }

    [OnReadyGet("../")] private ScrollContainer _scrollContainer;
    [OnReadyGet] public MeshesPanel _meshesPanel;

    [OnReady]
    private async void GetAreaSpaceRid()
    {
        await ToSignal(GetTree(), "idle_frame");
        OldPhysicsSpaceRid = Physics2DServer.AreaGetSpace(_meshesPanel._area2D.GetRid());
    }

    [OnReady]
    private void DetectScroll()
    {
        _scrollContainer.GetVScrollbar().Connect("value_changed", this, nameof(OnScroll));
    }

    private float _scrollValue = 0f;
    private readonly Timer _physicsTimeoutTimer = new Timer();
    private float _physicsTimeout = 0.5f;
    [OnReady] private async void SetupTimer()
    {
        NewPhysicsSpaceRid = Physics2DServer.SpaceCreate();
        GD.Print($"Space RID: {NewPhysicsSpaceRid.GetId()}");
        GD.Print("Adding loading throttle timer to tree");
        await ToSignal(GetTree(), "idle_frame");
        GetParent().AddChild(_physicsTimeoutTimer);
        _physicsTimeoutTimer.OneShot = true;
        _physicsTimeoutTimer.Connect("timeout", this, nameof(OnPhysicsTimeout));

    }

    private void OnScroll(float value)
    {
        if (_physicsTimeoutTimer.IsStopped())
        {
            _physicsTimeoutTimer.Start(_physicsTimeout);
            EnablePhysics();
        }
        _scrollValue = value;
    }

    private void EnablePhysics()
    {
        GD.Print("Enabling physics");
        Physics2DServer.SpaceSetActive(NewPhysicsSpaceRid, true);
        Physics2DServer.SpaceSetActive(OldPhysicsSpaceRid, false);
        Physics2DServer.SetActive(true);
    }

    private void OnPhysicsTimeout()
    {
        GD.Print("Disabling physics");
        Physics2DServer.SpaceSetActive(NewPhysicsSpaceRid, false);
        Physics2DServer.SpaceSetActive(OldPhysicsSpaceRid, true);
        Physics2DServer.SetActive(false);
    }

    public void OnResized()
    {
        if (GetChildCount() == 0) return;
        if (_mainDockControl == null) return;
        
        var numColumns = (int) (_mainDockControl.RectSize.x - 96) / (int)GetChild<MeshPreviewContainer>(0).RectSize.x;
        GD.Print($"Resized, new columns: {numColumns}");
        Columns = numColumns;
    }
}