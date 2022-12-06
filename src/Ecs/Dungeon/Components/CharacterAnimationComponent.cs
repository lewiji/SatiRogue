using System;
using System.Collections.Generic;
using Godot.Collections;
namespace SatiRogue.Ecs.Dungeon.Components; 

public partial class CharacterAnimationComponent {
   readonly Stack<string> _queuedAnimations = new ();
   string _animation = "idle";
   public string Animation {
      get => _animation;
      set {
         if (_queuedAnimations.Contains(value)) return;
         _animation = value;
         _queuedAnimations.Push(_animation);
      }
   }

   public bool HasAnimations() => _queuedAnimations.Count > 0;
   public void ClearAnimations() => _queuedAnimations.Clear();
   
   public string PopAnimation() {
      _queuedAnimations.TryPop(out var animation);
      return animation ?? "";
   }

   public string PeekAnimation() {
      _queuedAnimations.TryPeek(out var animation);
      return animation ?? "";
   }

}