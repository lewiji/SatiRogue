using Godot;
using SatiRogue.addons.wfc_tools.src.WfcDock;

namespace SatiRogue.addons.wfc_tools.src.ResourceStrategy;

public class MeshStrategy : AbstractResourceStrategy, IResourceStrategy {
   public override Resource? LoadResource(string path) {
      return LoadMesh(path);
   }

   protected override void HandlePreviewForContainer(ResourcePreviewContainer previewContainer) {
      if (previewContainer.TheResource is Mesh mesh)
         TheEditorInterface.GetResourcePreviewer()
            .QueueEditedResourcePreview(mesh, this, nameof(OnPreviewGenerated), previewContainer);
   }

   private Mesh? LoadMesh(string path) {
      if (!ResourceLoader.Exists(path, nameof(Mesh)))
         return null;
      var res = GD.Load<Mesh>(path);
      Resources.Add(res);
      return res;
   }
}