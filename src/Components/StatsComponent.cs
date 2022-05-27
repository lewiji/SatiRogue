using SatiRogue.Entities;

namespace SatiRogue.Components;

public class StatsComponent : Component
{
    public EntityStats Stats { get; set; }
    public int Health { get; set; }

    public override void _EnterTree()
    {
        base._EnterTree();
        Health = Stats.MaxHealth;
    }
}