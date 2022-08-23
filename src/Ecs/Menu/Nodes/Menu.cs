using Godot;
using GodotOnReady.Attributes;

namespace SatiRogue.Ecs.Menu.Nodes;

[Tool]
public partial class Menu : Control {
   [Signal] public delegate void NewGameRequested();

   [OnReadyGet("%Light2D")] private Light2D _light2D = null!;
   [OnReadyGet("%NewGame")] private Button _newGame = null!;
   [OnReadyGet("%ContinueGame")] private Button _continue = null!;
   [OnReadyGet("%Options")] private Button _options = null!;
   [OnReadyGet("%Quit")] private Button _quit = null!;

   [OnReady] private void ConnectButtons() {
      _newGame.Connect("pressed", this, nameof(OnNewGamePressed));
   }

   private void OnNewGamePressed() {
      EmitSignal(nameof(NewGameRequested));
   }

   [OnReady] private void SetLightSize() {
      var textureSize = _light2D.Texture.GetSize();
      _light2D.TextureScale = Mathf.Max(RectSize.x / textureSize.x, RectSize.y / textureSize.y);
      _light2D.Offset = RectSize / 2f;
   }
}