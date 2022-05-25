using System;
using System.Collections.Generic;
using Godot;
using SatiRogue.Debug;
using SatiRogue.Turn;
using Array = Godot.Collections.Array;

namespace SatiRogue;

public interface IGameObjectParameters {
   string? Name { get; set; }
   GameObject? Parent { get; set; }
}

public class GameObjectParameters : IGameObjectParameters {
   public string? Name { get; set; }
   public GameObject? Parent { get; set; }
}

public interface IGameObject {
   public string Uuid { get; }
   public GameObject? Parent { get; set; }
   void InitialiseWithParameters(IGameObjectParameters parameters);
}

public abstract class GameObject : Node, IGameObject {
   public bool Enabled { get; set; } = true;
   protected virtual IGameObjectParameters? Parameters { get; set; }
   protected abstract List<Turn.Turn> TurnTypesToExecuteOn { get; set; }
   public string Uuid { get; protected set; } = Guid.NewGuid().ToString();
   public virtual GameObject? Parent { get; set; }

   public void InitialiseWithParameters(IGameObjectParameters parameters) {
      Parameters = parameters;
   }

   public override void _Notification(int what) {
      if (what == NotificationEnterTree) {
         Parent = Parameters?.Parent ?? GetParentOrNull<GameObject>();
         Name = Parameters?.Name ?? "GameObject";
         Systems.TurnHandler.Connect(nameof(TurnHandler.OnEnemyTurnStarted), this, nameof(FilterTurnTypesToExecuteOn),
            new Array {Turn.Turn.EnemyTurn});
         Systems.TurnHandler.Connect(nameof(TurnHandler.OnPlayerTurnStarted), this, nameof(FilterTurnTypesToExecuteOn),
            new Array {Turn.Turn.PlayerTurn});
      }
   }


   private void FilterTurnTypesToExecuteOn(Turn.Turn turn) {
      if (TurnTypesToExecuteOn.Contains(turn))
         HandleTurn();
   }

   public virtual void HandleTurn() {
      Logger.Debug("HandleTurn not implemented");
   }
}