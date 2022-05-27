using System.Collections.Generic;
using System.Linq;
using Godot.Collections;
using SatiRogue.Components;
using SatiRogue.Turn;

namespace SatiRogue.Entities;

public class EntityParameters : GameObjectParameters {
   public Component[] Components { get; set; } = { };
}

public abstract class Entity : GameObject, IEntity {
   private EntityParameters? _parameters;
   
   public IEnumerable<Component> Components => _components;
   private List<Component> _components { get; } = new();
   
   protected abstract List<Turn.Turn> TurnTypesToExecuteOn { get; set; }
   
   
   public override void _EnterTree() {
      base._EnterTree();
      Name = Parameters?.Name ?? "Entity";
      
      Systems.TurnHandler.Connect(nameof(TurnHandler.OnEnemyTurnStarted), this, nameof(FilterTurnTypesToExecuteOn),
         new Array {Turn.Turn.EnemyTurn});
      Systems.TurnHandler.Connect(nameof(TurnHandler.OnPlayerTurnStarted), this, nameof(FilterTurnTypesToExecuteOn),
         new Array {Turn.Turn.PlayerTurn});
   }

   public override void _Ready() {
      base._Ready();
      if (Parameters is not EntityParameters entityParameters) return;
      foreach (var parametersComponent in entityParameters.Components) AddComponent(parametersComponent);
   }
   
   private void FilterTurnTypesToExecuteOn(Turn.Turn turn)
   {
      if (!TurnTypesToExecuteOn.Contains(turn)) return;
      ProcessComponents();
      HandleTurn();
   }

   protected override IGameObjectParameters? Parameters {
      get => _parameters;
      set => _parameters = value as EntityParameters;
   }

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

   private void ProcessComponents()
   {
      foreach (var component in Components.ToArray())
      {
         component.HandleTurn();
      }
   }

   public IEnumerable<T>? GetComponents<T>() where T : Component {
      return _components.Where(c => c.GetType() == typeof(T)) as IEnumerable<T>;
   }

   public T? GetComponent<T>() where T : Component {
      return _components.FirstOrDefault(c => c.GetType().IsSubclassOf(typeof(T)) || c.GetType() == typeof(T)) as T;
   }
}