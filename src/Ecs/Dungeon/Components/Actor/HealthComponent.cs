namespace SatiRogue.Ecs.Dungeon.Components.Actor;

public class HealthComponent {
   public int Value { get; set; }

   public int Max;
   public bool IsAlive {
      get => Value > 0;
   }
   public float Percent {
      get => Value / (float) Max;
   }

   public HealthComponent() { }

   public HealthComponent(int max, int? current = null) {
      Max = max;
      Value = current ?? max;
   }
}