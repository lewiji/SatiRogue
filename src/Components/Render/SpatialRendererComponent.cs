using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Entities;
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
      
      RootNode.Visible = GridEntity.Visible;
   }

   public override void _ExitTree() {
      base._ExitTree();
      RootNode?.QueueFree();
   }

   protected override void CreateVisualNodes() { }
   
   [OnReady]
   private void ConnectPositionChanged() {
      GridEntity?.Connect(nameof(GridEntity.PositionChanged), this, nameof(HandlePositionChanged));
   }

   protected virtual void HandlePositionChanged() {
      if (GridEntity == null || RootNode == null) return;
      TargetTranslation = GridEntity.GridPosition.ToVector3();
   }

   public override void HandleTurn() {
      base.HandleTurn();
      if (GridEntity == null || RootNode == null) return;
      RootNode.Visible = GridEntity.Visible;
   }

   private void OnFinishedTeleporting() {
      Teleporting = false;
   }

   public override void _PhysicsProcess(float delta) {
      if (RootNode == null) return;
      if (TargetTranslation.HasValue && !RootNode.Translation.IsEqualApprox(TargetTranslation.Value)) {
         if (Teleporting) {
            RootNode.Translation = TargetTranslation.Value;
            CallDeferred(nameof(OnFinishedTeleporting));
         }
         else {
            RootNode.Translation = RootNode.Translation.LinearInterpolate(TargetTranslation.Value, 0.16f);
         }
      } else if (TargetTranslation.HasValue) {
         TargetTranslation = null;
      }
   }
}