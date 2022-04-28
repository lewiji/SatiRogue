using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using GodotOnReady.Attributes;

namespace RoguelikeMono.addons.wfc_tools.src.WfcDock;

[Tool]
public partial class MeshesPanel : Panel
{
    [OnReadyGet("../MeshesToolbar/ButtonSelectFolder")]
    private ButtonSelectFolder _buttonSelectFolder;

    [OnReadyGet("MarginContainer/VBoxContainer/HBoxContainer/FolderLabel")]
    private Label _labelFolder;

    [OnReadyGet("MarginContainer/VBoxContainer/MarginContainer/ScrollContainer/GridContainer")]
    private MeshPreviewGridContainer _meshPreviewGrid;

    private readonly EditorInterface _editorInterface = new EditorPlugin().GetEditorInterface();

    [OnReadyGet("Node2D/Area2D")] public Area2D _area2D;

    [OnReady]
    private void AssignAreaToSpace()
    {
        Physics2DServer.AreaSetSpace(_area2D.GetRid(), _meshPreviewGrid.NewPhysicsSpaceRid);
    }
    
    [OnReadyGet] private Control? _mainDockControl;
    [OnReady] private void ConnectResized()
    {
        _mainDockControl?.Connect("resized", this, nameof(OnResized));
    }
    
    private void OnResized()
    {
        var colShape = (CollisionShape2D) _area2D.GetChild(0);
        var rectShape = (RectangleShape2D) colShape.Shape;
        rectShape.Extents = (_mainDockControl.RectSize / 2f) - new Vector2(0f, 58f);
        colShape.Position = _mainDockControl.RectPosition + (rectShape.Extents);
    }

    [OnReady]
    private void ConnectLazyLoadSignal()
    {
        _meshPreviewGrid.Connect(nameof(MeshPreviewGridContainer.LoadMoreMeshes), this, nameof(OnLazyLoad));
    }

    private string? _selectedDirPath;

    private PackedScene _meshPreviewContainerScene =
        GD.Load<PackedScene>("res://addons/wfc_tools/scenes/WfcDock/MeshPreviewContainer.tscn");

    private List<Mesh> _meshes = new List<Mesh>();
    private int? _meshPreviewIconSize = null;
    private int _meshesPerPage = 64;
    private Queue<MeshPreviewContainer> _meshesToProcess = new Queue<MeshPreviewContainer>();

    private List<MeshPreviewContainer> _meshPreviewContainersToGeneratePreviewsFor = new List<MeshPreviewContainer>();

    [OnReady]
    private void ConnectFolderSelectedSignal()
    {
        _buttonSelectFolder.Connect(nameof(ButtonSelectFolder.FolderSelected), this, nameof(OnFolderSelected));
    }

    private void OnFolderSelected(string path)
    {
        _selectedDirPath = path;
        _labelFolder.Text = $"Folder: {path}";
        EnumerateMeshes(path);
    }

    private void OnLazyLoad()
    {
        GD.Print("Lazily loading");
        if (_meshes.Count > 0)
        {
            DrawMeshList();
        }
    }

    private void EnumerateMeshes(string path)
    {
        var dir = new Directory();
        if (dir.Open(path) == Error.Ok)
        {
            dir.ListDirBegin(true, true);
            var fileName = dir.GetNext();

            while (fileName != "")
            {
                if (!dir.CurrentIsDir())
                {
                    var filePath = $"{path}/{fileName}";
                    if (ResourceLoader.Exists(filePath, nameof(Mesh)))
                    {
                        _meshes.Add(GD.Load<Mesh>(filePath));
                    }
                }

                fileName = dir.GetNext();
            }

            dir.ListDirEnd();
            GD.Print($"Found {_meshes.Count} meshes in {path}.");

            CallDeferred(nameof(DrawMeshList));
        }
        else
        {
            GD.PushWarning($"Couldn't open mesh directory at: {path}");
        }
    }

    private void DrawMeshList()
    {
        var someMeshes = _meshes.Take(_meshesPerPage);
        foreach (var mesh in someMeshes)
        {
            var preview = _meshPreviewContainerScene.Instance<MeshPreviewContainer>();
            _meshPreviewGrid.AddChild(preview);
            preview.SetMesh(mesh);
            _meshPreviewContainersToGeneratePreviewsFor.Add(preview);
            if (_meshPreviewContainersToGeneratePreviewsFor.Count == _meshesPerPage - 1)
            {
                preview.CallDeferred(nameof(MeshPreviewContainer.EnableAreaMonitoring));
            }
        }
        _meshes.RemoveRange(0, _meshesPerPage);
        CreatePreviewJobStack();
    }

    private void CreatePreviewJobStack()
    {
        var unpreviewed = _meshPreviewContainersToGeneratePreviewsFor
            .Where(s => !s.GeneratedPreview);

        var meshPreviewContainers = unpreviewed.ToList();
        foreach (var meshPreviewContainer in meshPreviewContainers)
        {
            if (meshPreviewContainer.Mesh == null) continue;
            _meshesToProcess.Enqueue(meshPreviewContainer);
            _meshPreviewIconSize ??= meshPreviewContainer.IconSize;
        }
        
        _meshPreviewContainersToGeneratePreviewsFor.Clear();
    }

    public override void _Process(float delta)
    {
        if (_meshesToProcess.Count > 0)
        {
            var meshPreviewContainer = _meshesToProcess.Dequeue();
            //var icon = _editorInterface.MakeMeshPreviews(new Array{meshPreviewContainer.Mesh}, _meshPreviewIconSize!.Value)[0];
            //meshPreviewContainer.SetPreviewTexture(meshPreviewContainer.Mesh.);
            _editorInterface.GetResourcePreviewer()
                .QueueEditedResourcePreview(meshPreviewContainer.Mesh, this, nameof(OnPreviewGenerated), meshPreviewContainer);
        }
        else
        {
            _buttonSelectFolder.Disabled = false;
        }
    }

    private void OnPreviewGenerated(string path, Texture preview, Texture thumbnail_preview, object userdata)
    {
        var meshPreviewContainer = (MeshPreviewContainer) userdata;
        meshPreviewContainer.SetPreviewTexture(preview);
    } 
}