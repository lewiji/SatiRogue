using Godot;
using Godot.Collections;
using GodotOnReady.Attributes;

namespace RoguelikeMono.addons.wfc_tools.src.WfcDock;

[Tool]
public partial class MeshPreviewContainer : CenterContainer
{
    [OnReadyGet("VBoxContainer/TexturePreview")]
    private TextureRect _texturePreview;

    private RID _originalSpaceRid; 
    
    [OnReadyGet("VBoxContainer/MeshName")]
    private Label _meshNameLabel;

    [OnReadyGet("ColorRect")] private Control _selectionTint;
    [OnReadyGet("../")] private MeshPreviewGridContainer _meshPreviewGridContainer;
    [OnReadyGet("Node2D/Area2D")] private Area2D _area2D;
    [OnReady] private void AssignAreaToSpace()
    {
        _originalSpaceRid = Physics2DServer.AreaGetSpace(_area2D.GetRid());
        Physics2DServer.AreaSetSpace(_area2D.GetRid(), _meshPreviewGridContainer.NewPhysicsSpaceRid);
    }

    public bool GeneratedPreview = false;
    public Mesh? Mesh;
    public int IconSize;

    [OnReady] private void SetIconSize() { IconSize = (int)_texturePreview.RectSize.x; }

    [OnReady] private void ListenToClicks() { Connect("gui_input", this, nameof(OnGuiInput)); }

    [OnReady] private void ListenToMeshSelectedSignal() { 
        _meshPreviewGridContainer.Connect(nameof(MeshPreviewGridContainer.MeshSelected), this, nameof(OnMeshSelected));
    }

    public void EnableAreaMonitoring()
    {
        _area2D.Monitoring = true;
        _area2D.Monitorable = true;
        ((CollisionShape2D) _area2D.GetChild(0)).Disabled = false;
        //_area2D.Connect("area_shape_entered", this, nameof(OnAreaEntered));
        GD.Print($"Enabled area monitoring for child {GetIndex()}");
    }

    public void DisableAreaMonitoring()
    {
        _area2D.Monitoring = false;
        _area2D.Monitorable = false;
        ((CollisionShape2D) _area2D.GetChild(0)).Disabled = true;
        //_area2D.Disconnect("area_shape_entered", this, nameof(OnAreaEntered));
        
        GD.Print($"Disabled area monitoring for child {GetIndex()}");
    }

    private void OnAreaEntered(RID area_rid, Area2D area, int shape_index, int local_shape_index)
    {
        CallDeferred(nameof(DisableAreaMonitoring));
        GD.Print($"{GetIndex()} is visible!!");
        _meshPreviewGridContainer.CallDeferred("emit_signal", nameof(MeshPreviewGridContainer.LoadMoreMeshes));
    }

    public override void _PhysicsProcess(float delta)
    {
        if (_area2D.Monitoring && Physics2DServer.SpaceIsActive(_meshPreviewGridContainer.NewPhysicsSpaceRid))
        {
            var state = Physics2DServer.SpaceGetDirectState(Physics2DServer.AreaGetSpace(_area2D.GetRid()));
            var query = new Physics2DShapeQueryParameters();
            query.SetShape(((CollisionShape2D) _area2D.GetChild(0)).Shape);
            query.CollideWithAreas = true;
            query.CollideWithBodies = false;
            query.Transform = _area2D.GlobalTransform;
            var result = state.IntersectShape(query);
            if (result.Count > 0)
            {
                var first = (Dictionary) result[0];
                if (first["collider_id"].ToString() == _meshPreviewGridContainer._meshesPanel._area2D.GetInstanceId().ToString())
                {
                    CallDeferred(nameof(DisableAreaMonitoring));
                    Physics2DServer.SpaceSetActive(_meshPreviewGridContainer.NewPhysicsSpaceRid, false);
                    Physics2DServer.SpaceSetActive(_meshPreviewGridContainer.OldPhysicsSpaceRid, true);
                    Physics2DServer.SetActive(false);
                    _meshPreviewGridContainer.EmitSignal(nameof(MeshPreviewGridContainer.LoadMoreMeshes));
                }
            }
        }
    }

    private void OnMeshSelected(int index)
    {
        _selectionTint.Visible = index == GetIndex();
    }

    private void OnGuiInput(InputEvent @event)
    {
        if (@event is InputEventMouseButton ieMouse && ieMouse.IsPressed())
        {
            if (ieMouse.ButtonIndex == (int) ButtonList.Left)
            {
                GetParent<MeshPreviewGridContainer>().EmitSignal(nameof(MeshPreviewGridContainer.MeshSelected), GetIndex());
            } else if (ieMouse.ButtonIndex == (int) ButtonList.Right)
            {
                if (Mesh != null)
                    (new EditorPlugin()).GetEditorInterface().GetFileSystemDock().NavigateToPath(Mesh.ResourcePath);
            }
            
        }
    }

    public void SetMesh(Mesh mesh)
    {
        _meshNameLabel.Text = mesh.ResourceName;
        HintTooltip = mesh.ResourceName;
        Mesh = mesh;
    }

    public void SetPreviewTexture(Texture? tex)
    {
        _texturePreview.Texture = tex;
        GeneratedPreview = true;
    }
}