using Godot;
using SatiRogue.Debug;

namespace SatiRogue.Ecs.Play.Components.Actor; 

public class HealthComponent {
   public int Value { get; set; }

   public int Max;
   public bool IsAlive => Value > 0;
   public float Percent => (float)Value / (float)Max;

   public HealthComponent() { }

   public HealthComponent(int value) {
      Max = value;
      Value = Max;
   }

}