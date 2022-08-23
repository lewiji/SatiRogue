namespace SatiRogue.Ecs.Play.Components.Actor;

public class Stats {
   public int Health;
   public int SightRange;
   public float Speed;
   public int Strength;
   public int Defence;

   public Stats() { }

   public Stats(int health, int sightRange, float speed, int strength, int defence) {
      Health = health;
      SightRange = sightRange;
      Speed = speed;
      Strength = strength;
      Defence = defence;
   }
}