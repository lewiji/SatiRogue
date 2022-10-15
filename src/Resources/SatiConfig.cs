using Godot;

namespace SatiRogue.resources;

public class SatiConfig : Resource {
   [Export]
   public bool DisableManualShaderPrecompiler { get; set; } = false;

   [Export]
   public bool DebugTools { get; set; } = false;
   public static bool IsMobile {
      get => OS.GetName() is "Android" or "iOS";
   }
}