using Godot;
using SatiRogue.Entities;
using SatiRogue.Grid.MapGen;
using GoDotNet;
using SatiRogue.Turn;

namespace SatiRogue.Components;

public abstract partial class Component : GameObject, IComponent, IDependent {
   public override GameObject? EcOwner => base.EcOwner as Entity;
   [Dependency] protected MapGenerator MapGenerator => this.DependOn<MapGenerator>();

   public virtual void Loaded() { }

   public override void _EnterTree() {
      Name = "Component";
      Enabled = true;
   }

   public override void _Ready() {
      GD.Print("Test");
      this.Depend();
   }
}