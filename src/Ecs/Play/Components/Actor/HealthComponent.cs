using Godot;
using SatiRogue.Debug;

namespace SatiRogue.Ecs.Play.Components.Actor; 

public class HealthComponent {
   private int _value;
   public int Value {
      get => _value;
      set {
         _value = value;
         Logger.Info($"Health: {_value}");
      }
   }
   public int Max;
   public bool IsAlive => Value > 0;

   public HealthComponent() { }

   public HealthComponent(int value) {
      Max = value;
      Value = Max;
   }

}