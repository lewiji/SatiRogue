using System.Collections.Generic;
using System.Linq;
using SatiRogue.Components;

namespace SatiRogue.Entities;

public class EntityParameters : GameObjectParameters {
   public Component[] Components { get; set; } = { };
}

public abstract class Entity : GameObject, IEntity {
   private EntityParameters? _parameters;
   private List<Component> _components { get; } = new();

   protected override IGameObjectParameters? Parameters {
      get => _parameters;
      set => _parameters = value as EntityParameters;
   }

   public IEnumerable<Component> Components => _components;

   public Component AddComponent(Component component) {
      component.Parent = this;
      _components.Add(component);
      AddChild(component);
      return component;
   }

   public void RemoveComponent(Component component) {
      _components.Remove(component);
      RemoveChild(component);
      component.QueueFree();
   }

   public IEnumerable<T>? GetComponents<T>() where T : Component {
      return _components.Where(c => c.GetType() == typeof(T)) as IEnumerable<T>;
   }

   public T? GetComponent<T>() where T : Component {
      return _components.FirstOrDefault(c => c.GetType().IsSubclassOf(typeof(T)) || c.GetType() == typeof(T)) as T;
   }

   public override void _EnterTree() {
      base._EnterTree();
      Name = Parameters?.Name ?? "Entity";
   }

   public override void _Ready() {
      base._Ready();
      if (Parameters is not EntityParameters entityParameters) return;
      foreach (var parametersComponent in entityParameters.Components) AddComponent(parametersComponent);
   }
}