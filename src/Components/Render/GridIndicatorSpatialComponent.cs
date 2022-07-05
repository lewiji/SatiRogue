using Godot;
using SatiRogue.Debug;
using SatiRogue.Entities;
using SatiRogue.scenes;

namespace SatiRogue.Components.Render;

public class GridIndicatorSpatialComponent : RendererComponent {
   private static readonly PackedScene GridIndicatorPackedScene = GD.Load<PackedScene>("res://resources/grid/GridIndicator.tscn");
   public Spatial? GridSpatial;
   private AnimationPlayer? _gridAnimation;
   public bool IsVisible = false;

   protected override void CreateVisualNodes() {
      GridSpatial = GridIndicatorPackedScene.Instance<Spatial>();
      _gridAnimation = GridSpatial.GetNode<AnimationPlayer>("AnimationPlayer");
      if (!EntityResourceLocator.SceneNodePaths.TryGetValue(nameof(ThreeDee), out var threeDeePath)) return;
      var threeDeeNode = GetNode<ThreeDee>(threeDeePath);
      if (threeDeeNode.GridIndicatorSpatial != null) {
         threeDeeNode.GridIndicatorSpatial.AddChild(GridSpatial);
         _gridAnimation.Play("RESET");
      }
   }

   public override void HandleTurn() {
      if (EntityRegistry.Player != null && GridSpatial != null) {
         GridSpatial.Translation = EntityRegistry.Player.GridPosition.ToVector3() + new Vector3(0, 0.01f, 0);
      }
   }

   public override void _Process(float delta) {
      base._Process(delta);
      if (Input.IsActionJustPressed("show_grid")) {
         if (GridSpatial == null || _gridAnimation == null) return;
         IsVisible = true;
         GridSpatial.Visible = true;
         if (_gridAnimation.CurrentAnimation != "RESET" && !_gridAnimation.IsPlaying()) {
            _gridAnimation.Play("fade_in");
         }
      } else if (Input.IsActionJustReleased("show_grid") && _gridAnimation is {AssignedAnimation: "fade_in"}) {
         IsVisible = false;
         _gridAnimation.PlayBackwards("fade_in");
      }
   }
}