using System;
using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;

namespace SatiRogue.addons.scene_reloader;

[Tool]
public class SceneReloaderPlugin : EditorPlugin {
   private ToolButton _buttonScene = GD.Load<PackedScene>("res://addons/scene_reloader/SceneReloaderButton.tscn").Instance<ToolButton>();

   public override void _Ready() {
      AddControlToContainer(CustomControlContainer.Toolbar, _buttonScene);
      _buttonScene.Connect("pressed", this, nameof(ReloadOpenScene));
   }

   public override void _ExitTree() {
      _buttonScene.Disconnect("pressed", this, nameof(ReloadOpenScene));
      RemoveControlFromContainer(CustomControlContainer.Toolbar, _buttonScene);
   }

   // alternatives
   //.FindNode("@@640", true,false);
   //.GetNode("@@605/@@607/@@611/@@615/@@616/@@617/@@633/@@634/@@639/@@640");
   private async void ReloadOpenScene() {
      var baseControl = GetEditorInterface().GetBaseControl();
      var editedSceneRoot = GetEditorInterface().GetEditedSceneRoot();

      var fileMenu = baseControl.FindNodeByType<PopupMenu>(t => t.HasNode("Export"));

      GD.Print(fileMenu);

      if (fileMenu != null) {
         var itemCount = fileMenu.GetItemCount();
         var closeId = -1;
         var reopenId = -1;

         for (var i = 0; i < itemCount; i++) {
            if (fileMenu.GetItemText(i) == "Close Scene") {
               closeId = fileMenu.GetItemId(i);
            }

            if (fileMenu.GetItemText(i) == "Reopen Closed Scene") {
               reopenId = fileMenu.GetItemId(i);
            }
         }
         fileMenu.EmitSignal("id_pressed", closeId);
         await ToSignal(GetTree(), "idle_frame");
         fileMenu.EmitSignal("id_pressed", reopenId);
      }
   }
}

public static class NodeExtensions {
   public static List<TNode> FindNodesByType<TNode>(this Node rootNode) where TNode : Node {
      var nodes = new List<TNode>();
      var stack = new Stack<Node>(new[] {rootNode});

      while (stack.Any()) {
         var n = stack.Pop();
         if (n is TNode tNode) nodes.Add(tNode);
         foreach (Node child in n.GetChildren()) stack.Push(child);
      }
      return nodes;
   }

   public static TNode? FindNodeByType<TNode>(this Node rootNode, Func<TNode, bool>? condition = null) where TNode : Node {
      var stack = new Stack<Node>(new[] {rootNode});

      while (stack.Any()) {
         var n = stack.Pop();

         if (n is TNode tNode) {
            if (condition != null) {
               if (condition(tNode)) return tNode;
            } else {
               return tNode;
            }
         }
         foreach (Node child in n.GetChildren()) stack.Push(child);
      }
      return null;
   }
}