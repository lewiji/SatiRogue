using SatiRogue.Math;

namespace SatiRogue.Grid.Entities; 

public enum EnemyTypes { Maw, Ratfolk }

public class EnemyData : EntityData {
   public EnemyTypes EnemyType { get; private set; }

   public EnemyData(Vector3i? gridPosition = null, EnemyTypes? enemyType = null) : base(gridPosition, true) {
      EnemyType = enemyType.GetValueOrDefault();
   }
}