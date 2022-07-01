using Godot;
using SatiRogue.Debug;
using SatiRogue.Entities;
using SatiRogue.scenes;

namespace SatiRogue.Components.Render;

public class GridIndicatorSpatialComponent : RendererComponent {
   private static readonly PackedScene GridIndicatorPackedScene = GD.Load<PackedScene>("res://resources/grid/GridIndicator.tscn");
   private Spatial? _gridSpatial;
   private AnimationPlayer? _gridAnimation;

   protected override void CreateVisualNodes() {
      _gridSpatial = GridIndicatorPackedScene.Instance<Spatial>();
      _gridAnimation = _gridSpatial.GetNode<AnimationPlayer>("AnimationPlayer");
      if (!EntityResourceLocator.SceneNodePaths.TryGetValue(nameof(ThreeDee), out var threeDeePath)) return;
      var threeDeeNode = GetNode<ThreeDee>(threeDeePath);
      if (threeDeeNode.GridIndicatorSpatial != null) {
         threeDeeNode.GridIndicatorSpatial.AddChild(_gridSpatial);
         _gridAnimation.Play("RESET");
      }
   }

   public override void HandleTurn() {
      if (EntityRegistry.Player != null && _gridSpatial != null) {
         _gridSpatial.Translation = EntityRegistry.Player.GridPosition.ToVector3() + new Vector3(0, 0.01f, 0);
      }
   }

   public override void _Process(float delta) {
      base._Process(delta);
      if (Input.IsActionJustPressed("show_grid")) {
         if (_gridSpatial != null) {
            _gridSpatial.Visible = true;
            if (_gridAnimation != null) {
               if (_gridAnimation.CurrentAnimation != "RESET" && !_gridAnimation.IsPlaying()) {
                  _gridAnimation.Play("fade_in");
               }
            }
         }
      } else if (Input.IsActionJustReleased("show_grid")) {
         if (_gridAnimation != null) {
            if (_gridAnimation.AssignedAnimation == "fade_in") {
               _gridAnimation.PlayBackwards("fade_in");
            }
         }
      }
   }
}