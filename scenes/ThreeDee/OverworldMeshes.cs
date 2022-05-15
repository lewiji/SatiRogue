using Godot;

namespace SatiRogue.scenes;

public class OverworldMeshes : Spatial {
   public override void _Ready() {
      var kids = GetChildren();
      GD.Print($"Saving {kids.Count} meshes");
      foreach (Node kid in kids)
         if (kid is MeshInstance meshInstance) {
            GD.Print($"Saving {meshInstance.Name}");
            ResourceSaver.Save($"res://scenes/ThreeDee/res_no_vertices/{meshInstance.Name}.mesh", meshInstance.Mesh);
         }
   }
}