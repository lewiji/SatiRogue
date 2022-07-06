using Godot;
using GoDotNet;
using GodotOnReady.Attributes;
using SatiRogue.Entities;
using SatiRogue.Grid.MapGen;
using SatiRogue.Turn;

namespace SatiRogue;

public partial class Systems : Node, IProvider<MapGenerator>, IProvider<TurnHandler> {
   private TurnHandler? _turnHandler;
   [OnReadyGet("MapGenerator")] private MapGenerator _mapGenerator = null!;
   MapGenerator IProvider<MapGenerator>.Get() => _mapGenerator;
   TurnHandler IProvider<TurnHandler>.Get() => _turnHandler!;

   public override void _EnterTree() {
      _turnHandler = new TurnHandler();
      AddChild(_turnHandler);
   }
   
   [OnReady]
   private void DoProvideMapGenerator() {
      this.Provided();
   }
}