namespace SatiRogue.Entities;

public struct EntityStats {
   private int _maxHealth;

   public int MaxHealth {
      get => _maxHealth;
      set {
         _maxHealth = value;
         CurrentHealth ??= _maxHealth;
      }
   }

   public int? CurrentHealth { get; set; }
   public int Level { get; set; }
}