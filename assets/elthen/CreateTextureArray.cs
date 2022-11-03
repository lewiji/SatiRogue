using System.Collections.Generic;
using Godot;
using SatiRogue.Debug;

namespace SatiRogue.assets.elthen; 
[Tool]
public class CreateTextureArray : Node
{
   public override void _Ready()
   {
      var texNames = new[]{"albedo", "normal", "occlusion", "parallax", "specular"};
      var textures = new List<Texture>();
      foreach (var texName in texNames) {
         textures.Add(GD.Load<Texture>($"res://assets/elthen/elthen_{texName}.png"));
      }

      var texArray = new TextureArray();
      texArray.Create(
         (uint)textures[0].GetWidth(), 
         (uint)textures[0].GetHeight(), 
         (uint)textures.Count, 
         Image.Format.Rgba8
      );
      texArray.Flags = (uint)TextureLayered.FlagsEnum.FlagMipmaps | (uint)TextureLayered.FlagsEnum.FlagAnisotropicFilter;
      for (var index = 0; index < textures.Count; index++) {
         var texture = textures[index];
         var image = texture.GetData();

         if (image.IsCompressed()) {
            image.Decompress();
         }
         image.Convert(Image.Format.Rgba8);
         texArray.SetLayerData(image,index);
      }
      var texArrResPath = "res://assets/elthen/elthen_tex_array.texarr";
      GD.Print($"Saving to {texArrResPath}");
      ResourceSaver.Save(texArrResPath, texArray, ResourceSaver.SaverFlags.ChangePath);
   }
}