using Godot;
using Godot.Collections;
using SatiRogue.Debug;
namespace SatiRogue.Ecs.Menu.Nodes;

public partial class Menu : Control {
   [Signal] public delegate void NewGameRequestedEventHandler();
   [Signal] public delegate void OptionsRequestedEventHandler();

   PointLight2D _light2D = default!;
   Button _newGame = default!;
   Button _continue = default!;
   Button _options = default!;
   Button _quit = default!;

   public override void _Ready()
   {
	   _light2D = GetNode<PointLight2D>("%PointLight2D");
	   _newGame = GetNode<Button>("%NewGame");
	   _continue = GetNode<Button>("%ContinueGame");
	   _options = GetNode<Button>("%Options");
	   _quit = GetNode<Button>("%Quit");
	   ConnectButtons();
	   SetLightSize();
   }

   void ConnectButtons() {
	  _newGame.Pressed += OnNewGamePressed;
	  var newGameOccluder = (LightOccluder2D) _newGame.GetNode("Control/LightOccluder2D");
	  _newGame.MouseEntered += () => { EnableOccluder(newGameOccluder); };
	  _newGame.MouseExited += () => { DisableOccluder(newGameOccluder); };
	  
	  _quit.Pressed += OnQuitPressed;
	  var quitOccluder = (LightOccluder2D) _quit.GetNode("Control/LightOccluder2D");
	  _quit.MouseEntered += () => { EnableOccluder(quitOccluder); };
	  _quit.MouseExited += () => { DisableOccluder(quitOccluder); };

	  _options.Pressed += OnOptionsPressed;
	  var optionsOccluder = (LightOccluder2D) _options.GetNode("Control/LightOccluder2D");
	  _options.MouseEntered += () => { EnableOccluder(optionsOccluder); };
	  _options.MouseExited += () => { DisableOccluder(optionsOccluder); };

	  GetViewport().SizeChanged += OnWindowSizeChanged;
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
	  GetTree().Notification((int)NotificationWmCloseRequest);
   }

   void OnWindowSizeChanged() {
	  var textureSize = _light2D.Texture.GetSize();
	  var viewportSize = GetViewport().GetVisibleRect().Size;
	  _light2D.TextureScale = 2f * Mathf.Max(viewportSize.x / textureSize.x, viewportSize.y / textureSize.y);
	  GD.Print(_light2D.TextureScale);
	  _light2D.Offset = viewportSize;
   }

   void SetLightSize() {
	  OnWindowSizeChanged();
   }
}
