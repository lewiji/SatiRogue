using System;
using System.Collections.Generic;
using Godot;
using GoDotNet;
using GodotOnReady.Attributes;
using SatiRogue.Components;
using SatiRogue.Debug;
using SatiRogue.Entities;
using SatiRogue.Turn;
using Array = Godot.Collections.Array;

namespace SatiRogue;

public interface IGameObjectParameters {
   string? Name { get; set; }
   GameObject? EcOwner { get; set; }
}

public class GameObjectParameters : IGameObjectParameters {
   public string? Name { get; set; }
   public GameObject? EcOwner { get; set; }
}

public interface IGameObject {
   public string Uuid { get; }
   public GameObject? EcOwner { get; set; }
   void InitialiseWithParameters(IGameObjectParameters parameters);
}

public abstract partial class GameObject : Node, IGameObject, IDependent {
   private bool _enabled = false;

   public bool Enabled {
      get => _enabled;
      set {
         _enabled = value;
         if (this is PlayerEntity) {
            InputHandlerComponent.InputEnabled = _enabled;
         }
      }
   }
   
   [Dependency] protected TurnHandler TurnHandler => this.DependOn<TurnHandler>();

   [OnReady]
   private void GetDependencies() {
      this.Depend();
   }

   public virtual void Loaded() {
      Enabled = true;
   }

   protected virtual IGameObjectParameters? Parameters { get; set; }
   public string Uuid { get; protected set; } = Guid.NewGuid().ToString();
   public virtual GameObject? EcOwner { get; set; }

   public void InitialiseWithParameters(IGameObjectParameters parameters) {
      Parameters = parameters;
   }

   public override void _Notification(int what) {
      if (what == NotificationEnterTree) {
         EcOwner = Parameters?.EcOwner ?? GetParentOrNull<GameObject>();
         Name = Parameters?.Name ?? "GameObject";
         Owner = EcOwner;
      }
   }

   public virtual void HandleTurn() {
      Logger.Debug("HandleTurn not implemented");
   }
}