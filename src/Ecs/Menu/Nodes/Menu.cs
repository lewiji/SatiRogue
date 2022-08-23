using Godot;
using GodotOnReady.Attributes;

namespace SatiRogue.Ecs.Menu.Nodes;

[Tool]
public partial class Menu : Control {
   [OnReadyGet("%Light2D")] private Light2D _light2D = null!;

   [OnReady] private void SetLightSize() {
      var textureSize = _light2D.Texture.GetSize();
      _light2D.TextureScale = Mathf.Max(RectSize.x / textureSize.x, RectSize.y / textureSize.y);
      _light2D.Offset = RectSize / 2f;
   }
}