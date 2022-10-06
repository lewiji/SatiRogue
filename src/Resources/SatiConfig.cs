using Godot;

namespace SatiRogue.resources;

public class SatiConfig : Resource {
   [Export]
   public bool DisableManualShaderPrecompiler { get; set; } = false;
}