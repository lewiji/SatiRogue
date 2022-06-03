using System.Collections.Generic;
using System.Linq;
using Godot;
using Godot.Collections;
using SatiRogue.Components;
using SatiRogue.Turn;

namespace SatiRogue.Entities;

public class EntityParameters : GameObjectParameters {
   public Component[] Components { get; set; } = { };
}

public abstract class Entity : GameObject, IEntity {
   private EntityParameters? _parameters;
   private bool _alive = true;
   
   public IEnumerable<Component> Components => _components;
   private List<Component> _components { get; } = new();
   protected abstract List<Turn.Turn> TurnTypesToExecuteOn { get; set; }
   public bool Alive {
      get => _alive;
      set {
         if (_alive && !value) {
            // uh oh we dead
            EmitSignal(nameof(Died));
         }
         _alive = value;
      }
   }
   [Signal] public delegate void Died();

   public override void _EnterTree() {
      base._EnterTree();
      Name = Parameters?.Name ?? "Entity";
      
      Systems.TurnHandler.Connect(nameof(TurnHandler.OnEnemyTurnStarted), this, nameof(FilterTurnTypesToExecuteOn),
         new Array {Turn.Turn.EnemyTurn});
      Systems.TurnHandler.Connect(nameof(TurnHandler.OnPlayerTurnStarted), this, nameof(FilterTurnTypesToExecuteOn),
         new Array {Turn.Turn.PlayerTurn});
      Connect(nameof(Died), this, nameof(OnDead));
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

   protected virtual async void OnDead() {
      Enabled = false;
      TurnTypesToExecuteOn.Clear();
      DisableComponents();
      await ToSignal(GetTree().CreateTimer(0.5f), "timeout");
      EntityRegistry.UnregisterEntity(this);
      ClearComponents();
      QueueFree();
   }

   public Component AddComponent(Component component) {
      component.EcOwner = this;
      _components.Add(component);
      AddChild(component);
      component.Owner = this;
      return component;
   }

   public void RemoveComponent(Component component) {
      _components.Remove(component);
      RemoveChild(component);
      component.QueueFree();
   }

   public void ClearComponents() {
      foreach (var component in _components) {
         RemoveChild(component);
         component.QueueFree();
      }
      _components.Clear();
   }
   
   public void DisableComponent(Component component) {
      component.Enabled = false;
   }

   public void DisableComponents() {
      foreach (var component in _components) {
         component.Enabled = false;
      }
   }

   private void ProcessComponents() {
      if (!Enabled) return;
      foreach (var component in Components.ToArray())
      {
         if (component.Enabled) component.HandleTurn();
      }
   }

   public IEnumerable<T>? GetComponents<T>() where T : Component {
      return _components.Where(c => c.GetType() == typeof(T)) as IEnumerable<T>;
   }

   public IEnumerable<Component> GetComponents() {
      return _components;
   }

   public T? GetComponent<T>() where T : Component {
      return _components.FirstOrDefault(c => c.GetType().IsSubclassOf(typeof(T)) || c.GetType() == typeof(T)) as T;
   }
}