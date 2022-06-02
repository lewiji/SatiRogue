using Active.Core;
using Active.Core.Details;
using SatiRogue.Debug;
using SatiRogue.Entities;

namespace SatiRogue.Components.Behaviours;

public abstract class BehaviourTreeComponent : Component {
   protected Gig? BehaviourTree { get; set; }


   public override void HandleTurn() {
      if (EcOwner == null || BehaviourTree == null) return;
      var status = BehaviourTree.Step();
      if (status.failing) Logger.Debug(StatusFormat.Status(status));
   }
}