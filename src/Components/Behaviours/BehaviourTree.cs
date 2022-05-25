using Active.Core;
using Active.Core.Details;
using SatiRogue.Debug;
using SatiRogue.Entities;

namespace SatiRogue.Components.Behaviours;

public abstract class BehaviourTreeComponent : Component {
   protected Entity? ParentEntity;
   protected Gig? BehaviourTree { get; set; }

   public override GameObject? Parent {
      get => ParentEntity;
      set => ParentEntity = value as Entity;
   }

   public override void HandleTurn() {
      if (ParentEntity == null || BehaviourTree == null) return;
      var status = BehaviourTree.Step();
      if (status.failing) Logger.Info(StatusFormat.Status(status));
   }
}