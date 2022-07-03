using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Debug;
using SatiRogue.Entities;
using SatiRogue.Grid;
using SatiRogue.scenes;

namespace SatiRogue.Components.Render; 

public partial class SpatialRendererComponent : RendererComponent {
   protected GridEntity? GridEntity => Entity as GridEntity;
   protected Spatial? RootNode;
   protected Vector3? TargetTranslation;
   protected bool Teleporting { get; set; }

   [OnReady]
   protected virtual void CreateRootNode() {
      if (GridEntity == null) return;
      if (!EntityResourceLocator.SceneNodePaths.TryGetValue(nameof(ThreeDee), out var threeDeePath)) return;
      var threeDeeNode = GetNode<ThreeDee>(threeDeePath);
      if (threeDeeNode.EnemiesSpatial != null && threeDeeNode.EnemiesSpatial.HasNode(GridEntity.Uuid)) {
         RootNode = threeDeeNode.EnemiesSpatial.GetNode<Spatial>(GridEntity.Uuid);
      }
      else {
         RootNode = new Spatial() {Translation = GridEntity.GridPosition.ToVector3(), Name = GridEntity.Uuid};
         threeDeeNode.EnemiesSpatial?.AddChild(RootNode);
      }
   }

   public override void _ExitTree() {
      base._ExitTree();
      RootNode?.QueueFree();
   }

   protected override void CreateVisualNodes()
   {
      if (RootNode == null || GridEntity == null) return;
      CallDeferred(nameof(HandleVisibilityChanged));
      CallDeferred(nameof(HandlePositionChanged));
   }
   
   [OnReady]
   private async void ConnectPositionChanged() {
      GridEntity?.Connect(nameof(GridEntity.PositionChanged), this, nameof(HandlePositionChanged));
      GridEntity?.Connect(nameof(GridEntity.VisibilityChanged), this, nameof(HandleVisibilityChanged));
      RuntimeMapNode.Instance?.Connect(nameof(RuntimeMapNode.MapChanged), this, nameof(HandleVisibilityChanged));
      await ToSignal(GetTree(), "idle_frame");
      HandleVisibilityChanged();
   }

   protected virtual void HandlePositionChanged() {
      if (GridEntity == null || RootNode == null) return;
      TargetTranslation = GridEntity.GridPosition.ToVector3();
   }

   protected virtual void HandleVisibilityChanged()
   {
      if (GridEntity == null || RootNode == null) return;
      RootNode.Visible = GridEntity.Visible;
   }
   
   private void OnFinishedTeleporting() {
      Teleporting = false;
   }

   public override void _PhysicsProcess(float delta) {
      if (RootNode == null) return;
      if (TargetTranslation.HasValue) {
         var distanceSq = RootNode.Translation.DistanceTo(TargetTranslation.Value);
         if (Teleporting) {
            RootNode.Translation = TargetTranslation.Value;
            TargetTranslation = null;
            CallDeferred(nameof(OnFinishedTeleporting));
            ResetPhysicsInterpolation();
         } else if (distanceSq < 0.025f) {
            RootNode.Translation = TargetTranslation.Value;
            ResetPhysicsInterpolation();
            TargetTranslation = null;
         } else {
            RootNode.Translation = RootNode.Translation.LinearInterpolate(TargetTranslation.Value, 14f * delta);
         }
      }
   }
}