using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Godot;

namespace RoguelikeMono.Ecs;

public class Entity : Node {
   private readonly HashSet<Component> _components = new();
   public long Id { get; set; }

   public void AddComponent(Component component, bool addAsChild = true) {
      _components.Add(component);
      component.Entity = this;
      if (addAsChild) AddChild(component);
   }

   public IEnumerable GetComponents() {
      return _components.AsEnumerable();
   }

   public IEnumerable<T> GetComponents<T>() where T : Component {
      return _components.OfType<T>();
   }

   public T GetFirstComponent<T>() where T : Component {
      return _components.OfType<T>().First();
   }
}