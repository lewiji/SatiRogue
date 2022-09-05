using Godot;
using Godot.Collections;
using GodotOnReady.Attributes;
using SatiRogue.Debug;

namespace SatiRogue.Ecs.Menu.Nodes;

[Tool]
public partial class Menu : Control {
   [Signal] public delegate void NewGameRequested();
   [Signal] public delegate void OptionsRequested();

   [OnReadyGet("%Light2D")] Light2D _light2D = null!;
   [OnReadyGet("%NewGame")] Button _newGame = null!;
   [OnReadyGet("%ContinueGame")] Button _continue = null!;
   [OnReadyGet("%Options")] Button _options = null!;
   [OnReadyGet("%Quit")] Button _quit = null!;

   [OnReady] void ConnectButtons() {
      _newGame.Connect("pressed", this, nameof(OnNewGamePressed));
      var newGameOccluder = (LightOccluder2D) _newGame.GetNode("Control/LightOccluder2D");
      _newGame.Connect("mouse_entered", this, nameof(EnableOccluder), new Array {newGameOccluder});
      _newGame.Connect("mouse_exited", this, nameof(DisableOccluder), new Array {newGameOccluder});

      _quit.Connect("pressed", this, nameof(OnQuitPressed));
      var quitOccluder = (LightOccluder2D) _quit.GetNode("Control/LightOccluder2D");
      _quit.Connect("mouse_entered", this, nameof(EnableOccluder), new Array {quitOccluder});
      _quit.Connect("mouse_exited", this, nameof(DisableOccluder), new Array {quitOccluder});

      _options.Connect("pressed", this, nameof(OnOptionsPressed));
      var optionsOccluder = (LightOccluder2D) _options.GetNode("Control/LightOccluder2D");
      _options.Connect("mouse_entered", this, nameof(EnableOccluder), new Array {optionsOccluder});
      _options.Connect("mouse_exited", this, nameof(DisableOccluder), new Array {optionsOccluder});

      GetViewport().Connect("size_changed", this, nameof(OnWindowSizeChanged));
   }

   void EnableOccluder(LightOccluder2D occluder) {
      occluder.Visible = true;
   }

   void DisableOccluder(LightOccluder2D occluder) {
      occluder.Visible = false;
   }

   void OnOptionsPressed() {
      EmitSignal(nameof(OptionsRequested));
   }

   void OnNewGamePressed() {
      EmitSignal(nameof(NewGameRequested));
   }

   void OnQuitPressed() {
      Logger.Info("Goodbye.");
      GetTree().Notification(NotificationWmQuitRequest);
   }

   void OnWindowSizeChanged() {
      var textureSize = _light2D.Texture.GetSize();
      var viewportSize = GetViewport().Size;
      _light2D.TextureScale = 2f * Mathf.Max(viewportSize.x / textureSize.x, viewportSize.y / textureSize.y);
      GD.Print(_light2D.TextureScale);
      _light2D.Offset = viewportSize;
   }

   [OnReady] void SetLightSize() {
      OnWindowSizeChanged();
   }
}