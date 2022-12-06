using System.Collections.Generic;
using Godot;
using Godot.Collections;
using SatiRogue.Debug;

namespace SatiRogue.assets.elthen; 
[Tool]
public partial class CreateTextureArray : Node
{
   public override void _Ready()
   {
      var texNames = new[]{"albedo", "normal", "occlusion", "parallax", "specular"};
      var textures = new List<Texture2D>();
      foreach (var texName in texNames) {
         textures.Add(GD.Load<Texture2D>($"res://assets/elthen/elthen_{texName}.png"));
      }

      var texArray = new Texture2DArray();
      var imgArray = new Array<Image>();
      for (var index = 0; index < textures.Count; index++) {
         var texture = textures[index];
         var image = texture.GetImage();

         if (image.IsCompressed()) {
            image.Decompress();
         }
         
         image.Convert(Image.Format.Rgba8);
         imgArray.Add(image);
      }
      
      texArray.CreateFromImages(imgArray);
      var texArrResPath = "res://assets/elthen/elthen_tex_array.tres";
      GD.Print($"Saving to {texArrResPath}");
      ResourceSaver.Save(texArray, texArrResPath);
   }
}