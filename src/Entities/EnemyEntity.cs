using System;
using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Turn;

namespace SatiRogue.Entities;

public class EnemyEntityParameters : GridEntityParameters {
   public EntityTypes EntityType;
   public int? SightRange;
}

public partial class EnemyEntity : GridEntity {
   [Signal] public delegate void EnemyPositionChanged();
   public EntityTypes EntityType { get; protected set; }
   public int SightRange { get; protected set; }
   
   private EnemyEntityParameters? _parameters;
   protected override IGameObjectParameters? Parameters {
      get => _parameters;
      set => _parameters = value as EnemyEntityParameters;
   }

   public override void _EnterTree() {
      base.Parameters = _parameters;
      base._EnterTree();
      if (_parameters == null) {
         throw new Exception("EnemyEntity was added to the tree without EnemyEntityParameters; call InitialiseWithParameters before registering the entity.");
      }
      EntityType = _parameters.EntityType;
      SightRange = _parameters.SightRange ?? 6;
      Name = _parameters.Name ?? "Enemy";
   }

   protected override void OnPositionChanged() {
      base.OnPositionChanged();
      EmitSignal(nameof(EnemyPositionChanged));
   }

   [OnReady]
   private void ConnectEnemyTurnSignal() {
      Systems.TurnHandler.Connect(nameof(TurnHandler.OnEnemyTurnStarted), this, nameof(HandleTurn));
   }
}