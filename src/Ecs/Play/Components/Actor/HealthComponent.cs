namespace SatiRogue.Ecs.Play.Components.Actor; 

public class HealthComponent {
   public int Value;
   public int Max;

   public HealthComponent() { }

   public HealthComponent(int value) {
      Max = value;
      Value = Max;
   }
}