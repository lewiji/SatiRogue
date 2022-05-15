using Godot;
using Godot.Collections;
using GodotOnReady.Attributes;
using SatiRogue.addons.src.WfcDock;

namespace SatiRogue.addons.wfc_tools.src.WfcDock;

[Tool]
public partial class ResourcePreviewContainer : Container {
   [OnReadyGet("Node2D/Area2D")] private Area2D _area2D = null!;
   [OnReadyGet("VBoxContainer/MeshName")] private Label _nameLabel = null!;
   [OnReadyGet("../")] private PreviewGridContainer? _previewGridContainer;
   [OnReadyGet("ColorRect")] private Control _selectionTint = null!;

   [OnReadyGet("VBoxContainer/TexturePreview")]
   private TextureRect _texturePreview = null!;

   private float _updateDeltaTime;
   public bool GeneratedPreview;
   public Resource? TheResource;

   [OnReady]
   private void AssignAreaToSpace() {
      Physics2DServer.AreaSetSpace(_area2D.GetRid(), _previewGridContainer?.NewSpaceRid);
   }

   [OnReady]
   private void ListenToClicks() {
      Connect("gui_input", this, nameof(OnGuiInput));
   }

   [OnReady]
   private void ListenToMeshSelectedSignal() {
      _previewGridContainer!.Connect(nameof(PreviewGridContainer.ResourceSelected), this, nameof(OnMeshSelected));
   }

   public void EnableAreaMonitoring() {
      _area2D.Monitoring = true;
      _area2D.Monitorable = true;
      ((CollisionShape2D) _area2D.GetChild(0)).Disabled = false;
      GD.Print($"Enabled area monitoring for child {GetIndex()}");
   }

   public void DisableAreaMonitoring() {
      _area2D.Monitoring = false;
      _area2D.Monitorable = false;
      ((CollisionShape2D) _area2D.GetChild(0)).Disabled = true;
      GD.Print($"Disabled area monitoring for child {GetIndex()}");
   }

   public override void _PhysicsProcess(float delta) {
      if (_previewGridContainer == null) return;
      if (_updateDeltaTime >= 0.25f && _area2D.Monitoring) {
         _updateDeltaTime = 0;
         var state = Physics2DServer.SpaceGetDirectState(Physics2DServer.AreaGetSpace(_area2D.GetRid()));
         var query = new Physics2DShapeQueryParameters();
         query.SetShape(((CollisionShape2D) _area2D.GetChild(0)).Shape);
         query.CollideWithAreas = true;
         query.CollideWithBodies = false;
         query.Transform = _area2D.GlobalTransform;
         var result = state.IntersectShape(query);
         if (result.Count > 0) {
            var first = (Dictionary) result[0];
            if (first["collider_id"].ToString() == _previewGridContainer?.ResourcesPanel?.Area2D.GetInstanceId().ToString()) {
               CallDeferred(nameof(DisableAreaMonitoring));
               _previewGridContainer.EmitSignal(nameof(PreviewGridContainer.LoadMoreMeshes));
            }
         }
      }

      _updateDeltaTime += delta;
   }

   private void OnMeshSelected(int index) {
      _selectionTint.Visible = index == GetIndex();
   }

   private void OnGuiInput(InputEvent @event) {
      if (@event is InputEventMouseButton ieMouse && ieMouse.IsPressed()) {
         if (ieMouse.ButtonIndex == (int) ButtonList.Left)
            GetParent<PreviewGridContainer>().EmitSignal(nameof(PreviewGridContainer.ResourceSelected), GetIndex());
         else if (ieMouse.ButtonIndex == (int) ButtonList.Right)
            if (TheResource != null)
               new EditorPlugin().GetEditorInterface().GetFileSystemDock().NavigateToPath(TheResource.ResourcePath);
      }
   }

   public void SetResource(Resource res) {
      _nameLabel.Text = res.ResourceName;
      HintTooltip = res.ResourceName;
      TheResource = res;
   }

   public void SetPreviewTexture(Texture? tex) {
      _texturePreview.Texture = tex;
      GeneratedPreview = true;
      _texturePreview.SelfModulate = Colors.White;
   }
}