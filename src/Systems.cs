using Godot;
using GoDotNet;
using GodotOnReady.Attributes;
using SatiRogue.Entities;
using SatiRogue.Grid.MapGen;
using SatiRogue.Turn;

namespace SatiRogue;

public partial class Systems : Node, IProvider<TurnHandler> {
   private TurnHandler? _turnHandler;
   TurnHandler IProvider<TurnHandler>.Get() => _turnHandler!;

   public override void _EnterTree() {
      _turnHandler = new TurnHandler();
      AddChild(_turnHandler);
      this.Provided();
   }
   
}