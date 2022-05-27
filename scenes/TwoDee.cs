using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Grid.MapGen;

namespace SatiRogue.scenes;

public partial class TwoDee : Node {
   [OnReadyGet("../GridGenerator", Export = true)]
   public MapGenerator? GridGenerator;
}