using Active.Core;
using Godot;

namespace SatiRogue.Ecs.Play.Components.Actor; 

public class BehaviourTree {
   public Gig? TreeInstance;

   public BehaviourTree() { }

   public BehaviourTree(Gig treeInstance) {
      TreeInstance = treeInstance;
   }
}