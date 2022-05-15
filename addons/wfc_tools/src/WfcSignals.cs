using Godot;

namespace SatiRogue.addons.wfc_tools.src;

// ReSharper disable once ClassNeverInstantiated.Global
public class WfcSignals : Node {
   [Signal]
   public delegate void EnableFolderButton();

   private static WfcSignals? _instance;

   public static void Emit(string signalName, object[]? args = null) {
      _instance?.EmitSignal(signalName, args);
   }

   public static WfcSignals? GetInstance() {
      return _instance;
   }

   public override void _EnterTree() {
      _instance = this;
   }

   public override void _ExitTree() {
      _instance = null;
   }
}