using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Entities;
using SatiRogue.scenes;

namespace SatiRogue.Components.Render; 

public partial class SpatialRendererComponent : RendererComponent {
   protected GridEntity? GridEntity => Entity as GridEntity;
   protected Spatial? RootNode;

   [OnReady]
   private void CreateRootNode() {
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

   protected override void CreateVisualNodes() { }
   
   [OnReady]
   private void ConnectPositionChanged() {
      GridEntity?.Connect(nameof(GridEntity.PositionChanged), this, nameof(HandlePositionChanged));
   }

   private void HandlePositionChanged() {
      if (GridEntity == null || RootNode == null) return;
      RootNode.Translation = GridEntity.GridPosition.ToVector3();
      RootNode.Visible = GridEntity.Visible;
   }
}