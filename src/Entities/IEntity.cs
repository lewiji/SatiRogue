using System.Collections.Generic;
using SatiRogue.Components;

namespace SatiRogue.Entities; 

public interface IEntity {
   IEnumerable<Component> Components { get; }

   Component AddComponent(Component component);
   void RemoveComponent(Component component);
   public IEnumerable<T>? GetComponents<T>() where T : Component;
   public T? GetComponent<T>() where T : Component;
   public void HandleTurn();
}