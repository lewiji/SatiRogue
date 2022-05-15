using System.Collections.Generic;
using Godot;

namespace RoguelikeMono.Ecs;

public class BaseSystem<T> : Node where T : Component {
   protected static HashSet<T> Components = new();
   public bool Enabled = true;

   public static void Register(T component) {
      Components.Add(component);
   }

   public static void Tick(float delta) {
      foreach (var component in Components)
         if (component.Ticking)
            component.Tick(delta);
   }

   public override void _Process(float delta) {
      if (Enabled) Tick(delta);
   }
}