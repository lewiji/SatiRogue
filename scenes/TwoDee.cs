using Godot;
using GodotOnReady.Attributes;
using RoguelikeMono.Grid;

namespace RoguelikeMono.scenes; 

public partial class TwoDee : Node
{ 
    [OnReadyGet("../GridGenerator", Export = true)] public GridGenerator? GridGenerator;
}