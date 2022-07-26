using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Entities;
using SatiRogue.Grid.MapGen;
using GoDotNet;
using SatiRogue.Turn;

namespace SatiRogue;

public partial class Systems : Node, IProvider<TurnHandler> {
   private TurnHandler? _turnHandler;
   TurnHandler IProvider<TurnHandler>.Get() => _turnHandler!;

   public override void _Ready() {
      _turnHandler = new TurnHandler();
      AddChild(_turnHandler);
      this.Provided();
   }
   
}