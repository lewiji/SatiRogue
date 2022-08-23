using Godot;

namespace SatiStream;

[Tool]
public partial class SceneReloaderButton : ToolButton {
   public override void _EnterTree() {
      Icon = GetIcon("Refresh", "EditorIcons");
   }
}