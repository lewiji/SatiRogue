using System.Collections.Generic;
using Godot;

namespace SatiRogue.Entities;

public struct EnemyResourceBundle {
   public SpriteFrames Frames { get; set; }
   public Material MaterialInstance { get; set; }
}

public static class EnemyResourceLocator {
   public static Dictionary<EnemyTypes, EnemyResourceBundle> ResourceBundles = new() {
      {
         EnemyTypes.Maw,
         new EnemyResourceBundle {
            Frames = GD.Load<SpriteFrames>("res://scenes/ThreeDee/res/enemy/maw/maw_purple_sprite_Frames.tres"),
            MaterialInstance = GD.Load<Material>("res://scenes/ThreeDee/res/enemy/maw/maw_purple_spatial_mat.tres")
         }
      }
   };
}