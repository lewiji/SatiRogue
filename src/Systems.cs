using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Entities;
using SatiRogue.Turn;

namespace SatiRogue;

public partial class Systems : Node {
   public static TurnHandler? TurnHandler;
   public static string? Path;

   public override void _EnterTree() {
      Path = GetPath();
      TurnHandler = new TurnHandler();
      AddChild(TurnHandler);
   }

   public override void _ExitTree() {
      Path = null;
      TurnHandler = null;
   }
}