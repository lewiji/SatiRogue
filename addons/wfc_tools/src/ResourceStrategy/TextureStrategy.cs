using Godot;
using SatiRogue.addons.wfc_tools.src.WfcDock;

namespace SatiRogue.addons.wfc_tools.src.ResourceStrategy;

public class TextureStrategy : AbstractResourceStrategy, IResourceStrategy {
   public override Resource? LoadResource(string path) {
      return LoadTexture(path);
   }

   protected override void HandlePreviewForContainer(ResourcePreviewContainer previewContainer) {
      if (previewContainer.TheResource is Texture tex) {
         var image = tex.GetData();
         image.Resize(96, 96, Image.Interpolation.Nearest);
         var thumbTex = new ImageTexture();
         thumbTex.CreateFromImage(image);
         CallDeferred(nameof(OnPreviewGenerated), tex.ResourcePath, tex, thumbTex, previewContainer);
      }
   }

   private Texture? LoadTexture(string path) {
      if (!ResourceLoader.Exists(path, nameof(Texture)))
         return null;
      var res = GD.Load<Texture>(path);
      Resources.Add(res);
      return res;
   }
}