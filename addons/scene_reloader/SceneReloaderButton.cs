#if TOOLS
using Godot;
namespace SatiRogue.addons.scene_reloader;

[Tool]
public partial class SceneReloaderButton : ToolButton {
   public override void _EnterTree() {
      Icon = GetIcon("Refresh", "EditorIcons");
   }
}
#endif