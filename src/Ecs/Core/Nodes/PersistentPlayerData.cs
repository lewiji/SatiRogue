using System.Collections.Generic;
using System.Linq;
using Godot;
using SatiRogue.Ecs.Play.Components.Actor;
using SatiRogue.Ecs.Play.Nodes.Items;

namespace SatiRogue.Ecs.Core.Nodes;

public class PersistentPlayerData : Node {
   public int Gold;
   public int Health = 10;
   public Stats Stats = new(10, 10, 1, 1, 0);
   public int Floor;
   List<Item> _inventory = new();

   public void AddItem(Item item) {
      _inventory.Add(item);
      GD.Print($"Added {item} to inventory");
   }

   public List<Item> GetItems() {
      return _inventory;
   }

   public void Reset() {
      Stats = new Stats(10, 10, 1, 1, 0);
      Health = Stats.Health;
      Gold = 0;
      Floor = 0;

      foreach (var item in _inventory.Where(IsInstanceValid)) {
         item.QueueFree();
      }
      _inventory.Clear();
   }
}