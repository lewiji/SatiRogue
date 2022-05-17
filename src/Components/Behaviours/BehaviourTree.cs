using Active;
using Active.Core;
using Active.Core.Details;
using SatiRogue.Debug;
using static Active.Raw;
using static Active.Status;
using SatiRogue.Entities;

namespace SatiRogue.Components.Behaviours; 

public abstract class BehaviourTreeComponent : Component {
   protected Gig? BehaviourTree { get; set; }

   protected Entity? ParentEntity;
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