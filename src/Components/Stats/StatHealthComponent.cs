using System.Collections.Generic;

namespace SatiRogue.Components.Stats;

public class StatHealthComponent : StatsComponent
{
    public StatHealthComponent(int maxValue, int? initialValue = null) : base(StatEffectTypes.Stat, (int)StatTypes.Health, maxValue, 0, initialValue ?? maxValue)
    {
        
    }

    public override void OnChanged()
    {
        if (Value < MinValue)
        {
            
        }
    }

    public override void OnDepleted() {
        Entity.Alive = false;
    }
}