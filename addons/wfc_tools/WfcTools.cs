using Godot;

namespace SatiRogue.addons;

[Tool]
public class WfcTools : EditorPlugin {
   private Control? _dock;
   private Control? _mainScreen;

   public override bool HasMainScreen() {
      return true;
   }

   public override string GetPluginName() {
      return "WFC Tools";
   }

   public override Texture GetPluginIcon() {
      return GetEditorInterface().GetBaseControl().GetIcon("minimap", "GraphEdit");
   }

   public override void _EnterTree() {
      _dock = GD.Load<PackedScene>("res://addons/wfc_tools/scenes/wfc_dock.tscn").Instance<Control>();
      AddControlToBottomPanel(_dock, "WfcTools");

      _mainScreen = GD.Load<PackedScene>("res://addons/wfc_tools/scenes/wfc_main_screen.tscn").Instance<Control>();
      GetEditorInterface().GetEditorViewport().AddChild(_mainScreen);

      AddAutoloadSingleton("SatiWfcSignals", "res://addons/wfc_tools/src/WfcSignals.cs");

      MakeVisible(false);
   }

   public override void _ExitTree() {
      if (_dock != null) {
         RemoveControlFromBottomPanel(_dock);
         _dock.Free();
      }

      _mainScreen?.QueueFree();
      RemoveAutoloadSingleton("SatiWfcSignals");
   }

   public override void MakeVisible(bool visible) {
      if (_mainScreen != null) _mainScreen.Visible = visible;
   }
}