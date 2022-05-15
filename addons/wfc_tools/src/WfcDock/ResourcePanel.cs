using Godot;
using GodotOnReady.Attributes;
using SatiRogue.addons.wfc_tools.src.ResourceStrategy;

namespace SatiRogue.addons.src.WfcDock;

[Tool]
public partial class ResourcePanel : Panel {
   private const string GridPath = "MarginContainer/VBoxContainer/MarginContainer/ScrollContainer/GridContainer";
   [OnReadyGet(GridPath)] public static PreviewGridContainer PreviewGrid = null!;
   private readonly ResourceContext _resourceContext = new();

   [OnReadyGet("../MeshesToolbar/ButtonSelectFolder")]
   private ButtonSelectFolder _buttonSelectFolder = null!;

   [OnReadyGet("MarginContainer/VBoxContainer/HBoxContainer/FolderLabel")]
   private Label _labelFolder = null!;

   [OnReadyGet] private Control? _mainDockControl;
   [OnReadyGet] private ScrollContainer? _scrollContainer;
   [OnReadyGet("Node2D/Area2D")] public Area2D Area2D = null!;

   [OnReady]
   private void AddResourceContextToScene() {
      AddChild(_resourceContext);
   }

   [OnReady]
   private void AssignAreaToSpace() {
      Physics2DServer.AreaSetSpace(Area2D.GetRid(), PreviewGrid.NewSpaceRid);
   }

   [OnReady]
   private void ConnectResized() {
      _mainDockControl?.Connect("resized", this, nameof(OnResized));
   }

   [OnReady]
   private void ConnectLazyLoadSignal() {
      PreviewGrid.Connect(nameof(PreviewGridContainer.LoadMoreMeshes), this, nameof(OnLazyLoad));
   }

   [OnReady]
   private void ConnectFolderSelectedSignal() {
      _buttonSelectFolder.Connect(nameof(ButtonSelectFolder.FolderSelected), this, nameof(OnFolderSelected));
   }

   private void OnResized() {
      if (_mainDockControl == null) return;

      var colShape = (CollisionShape2D) Area2D.GetChild(0);
      var rectShape = (RectangleShape2D) colShape.Shape;
      rectShape.Extents = _mainDockControl.RectSize / 2f - new Vector2(0f, 58f);
      colShape.Position = _mainDockControl.RectPosition + rectShape.Extents;
   }

   private void ResetPreviewsPanel() {
      var kids = PreviewGrid.GetChildren();
      foreach (var kid in kids) ((Node) kid).QueueFree();
      _resourceContext.Reset();
      if (_scrollContainer != null) _scrollContainer.ScrollVertical = 0;
   }

   private async void OnFolderSelected(string path) {
      _labelFolder.Text = $"Folder: {path}";
      ResetPreviewsPanel();
      await ToSignal(GetTree(), "idle_frame");
      EnumerateResources(path);
   }

   private void OnLazyLoad() {
      GD.Print("Lazily loading");
      _resourceContext.CallDeferred(nameof(ResourceContext.Draw));
   }

   private void EnumerateResources(string path) {
      var dir = new Directory();
      if (dir.Open(path) == Error.Ok) {
         dir.ListDirBegin(true, true);
         var fileName = dir.GetNext();

         while (fileName != "") {
            if (!dir.CurrentIsDir()) {
               var filePath = $"{path}/{fileName}";
               if (ResourceLoader.Exists(filePath, nameof(Mesh)) && _resourceContext.GetStrategy() is not MeshStrategy)
                  _resourceContext.SetStrategy(new MeshStrategy());
               else if (ResourceLoader.Exists(filePath, nameof(Texture)) && _resourceContext.GetStrategy() is not TextureStrategy)
                  _resourceContext.SetStrategy(new TextureStrategy());
               _resourceContext.GenerateResourcePreview(filePath);
            }

            fileName = dir.GetNext();
         }

         dir.ListDirEnd();
         _resourceContext.CallDeferred(nameof(ResourceContext.Draw));
      }
      else {
         GD.PushWarning($"Couldn't open mesh directory at: {path}");
      }
   }
}