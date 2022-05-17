using System;
using Godot;

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
   public string Uuid { get; protected set; } = Guid.NewGuid().ToString();
   public virtual GameObject? Parent { get; set; }
   protected virtual IGameObjectParameters? Parameters { get; set; }
   public void InitialiseWithParameters(IGameObjectParameters parameters) => Parameters = parameters;

   public override void _EnterTree() {
      Parent = Parameters?.Parent ?? GetParentOrNull<GameObject>();
      Name = Parameters?.Name ?? "GameObject";
   }
}