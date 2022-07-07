using System.Collections.Generic;
using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Debug;

namespace SatiRogue.Components.Stats;

public partial class StatHealthComponent : StatsComponent
{
    
    [OnReady] private void ConnectSignals()
    {
        Connect(nameof(Changed), this, nameof(OnChanged));
        Connect(nameof(Depleted), this, nameof(OnDepleted));
    }

    public override void OnChanged(int newValue)
    {
        if (Value < MinValue)
        {
            
        }
    }

    public override void OnDepleted() {
        Entity.Alive = false;
    }
}