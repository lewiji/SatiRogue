using System;
using Godot;
using SatiRogue.Entities;
using SatiRogue.scenes;

namespace SatiRogue.Components.Render; 

public class Stairs3DRendererComponent : AnimatedSprite3DRendererComponent {
   private static readonly SpriteFrames SpriteFrames = GD.Load<SpriteFrames>("res://resources/props/stairs_spriteframes.tres");
   protected override void CreateVisualNodes() {
      if (GridEntity == null) throw new Exception("No parent stairs entity found for Stairs3dRenderer");
      
      AnimatedSprite = new AnimatedSprite3D {
         Frames = SpriteFrames,
         Playing = true,
         Animation = "default",
         Centered = true,
         PixelSize = 0.1f,
         RotationDegrees = new Vector3(-20f, 0, 0)
      };
      RootNode?.AddChild(AnimatedSprite);
      base.CreateVisualNodes();
   }

   protected override void CreateRootNode() {
      if (GridEntity == null) return;
      if (!EntityResourceLocator.SceneNodePaths.TryGetValue(nameof(ThreeDee), out var threeDeePath)) return;
      var threeDeeNode = GetNode<ThreeDee>(threeDeePath);
      if (threeDeeNode.StairsSpatial != null && threeDeeNode.StairsSpatial.HasNode(GridEntity.Uuid)) {
         RootNode = threeDeeNode.StairsSpatial.GetNode<Spatial>(GridEntity.Uuid);
      }
      else {
         RootNode = new Spatial() {Translation = GridEntity.GridPosition.ToVector3(), Name = GridEntity.Uuid};
         threeDeeNode.StairsSpatial?.AddChild(RootNode);
      }
   }
}