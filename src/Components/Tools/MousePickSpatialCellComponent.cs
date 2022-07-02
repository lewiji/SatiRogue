using System.Collections;
using Godot;
using Godot.Collections;
using SatiRogue.Components.Render;
using SatiRogue.Debug;
using SatiRogue.Entities;
using SatiRogue.resources.grid;
using SatiRogue.scenes;

namespace SatiRogue.Components.Tools; 

public class MousePickSpatialCellComponent : RendererComponent {
   private GridIndicatorSpatialComponent? _gridIndicator;
   private static GridMarker GridMarkerMeshInstance = GD.Load<PackedScene>("res://resources/grid/GridMarker.tscn").Instance<GridMarker>();
   private RayCast? _rayCast;
   public override void _EnterTree() {
      _gridIndicator = Entity?.GetComponent<GridIndicatorSpatialComponent>();
   }

   public override void _Input(InputEvent @event) {
      if (@event is not InputEventMouseButton {Pressed: true, ButtonIndex: (int)ButtonList.Left}) return;
      
      var camera = GetTree().Root.GetCamera();
      _rayCast ??= camera.GetNode<RayCast>("../../RayCast");
      
      var mousePos = GetTree().Root.GetMousePosition();
      
      var to =  camera.ProjectPosition(mousePos, 20f);

      _rayCast.CastTo = _rayCast.ToLocal(to);
      _rayCast.Enabled = true;
   }

   public override void _PhysicsProcess(float delta) {
      if (_rayCast is {Enabled: true} && _rayCast.IsColliding()) {
         var result = (Node)_rayCast.GetCollider();
         GD.Print(result.GetParent().Name);
         _rayCast.Enabled = false;
         GridMarkerMeshInstance.Move(_rayCast.GetCollisionPoint());
      }
   }


   protected override void CreateVisualNodes() {
      if (!EntityResourceLocator.SceneNodePaths.TryGetValue(nameof(ThreeDee), out var threeDeePath)) return;
      var threeDeeNode = GetNode<ThreeDee>(threeDeePath);
      threeDeeNode.GridIndicatorSpatial?.AddChild(GridMarkerMeshInstance);
   }
}