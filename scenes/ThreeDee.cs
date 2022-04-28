using Godot;
using GodotOnReady.Attributes;
using RoguelikeMono.Grid;

namespace RoguelikeMono.scenes.ThreeDee;

public partial class ThreeDee : Spatial
{
    
    [OnReadyGet("../../../../GridGenerator", Export = true)] public GridGenerator? GridGenerator;
    
}