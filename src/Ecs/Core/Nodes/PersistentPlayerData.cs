using System.Collections.Generic;
using System.Linq;
using Godot;
using SatiRogue.Ecs.Dungeon.Components.Actor;
using SatiRogue.Ecs.Dungeon.Nodes.Items;

namespace SatiRogue.Ecs.Core.Nodes;

public partial class PersistentPlayerData : Node {
   public int Gold;
   public int Health = 10;
   public Stats Stats = new(Stats.DefaultPlayerClass);
   public int Floor;
   public string PlayerName = "Player";
   List<Item> _inventory = new();

   public void AddItem(Item item) {
      _inventory.Add(item);
      GD.Print($"Added {item} to inventory");
   }

   public List<Item> GetItems() {
      return _inventory;
   }

   public void Reset() {
      Stats = new Stats(Stats.DefaultPlayerClass);
      Health = Stats.Record.Health;
      Gold = 0;
      Floor = 0;

      foreach (var item in _inventory.Where(IsInstanceValid)) {
         item.QueueFree();
      }
      _inventory.Clear();
   }
}