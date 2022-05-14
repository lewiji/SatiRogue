using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Turn;

namespace SatiRogue; 

public partial class Systems : Node {
   public static TurnHandler TurnHandler = new TurnHandler();
   
   [OnReady] private void AddSystemsToScene() {
      AddChild(TurnHandler);
   }
}