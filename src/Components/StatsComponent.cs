using System;
using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Components.Stats;
using SatiRogue.Entities;

namespace SatiRogue.Components;

public abstract partial class StatsComponent : Component
{
    private int _value;

    protected StatsComponent(StatEffectTypes statType, int statTypeIndex, int maxValue, int minValue = 0, int? initialValue = null)
    {
        StatType = statType;
        StatTypeIndex = statTypeIndex;
        MaxValue = maxValue;
        MinValue = minValue;
        Value = initialValue ?? minValue;
    }
    
    [Signal] public delegate void Changed(int newValue);
    [OnReady] private void ConnectChangedSignal()
    {
        Connect(nameof(Changed), this, nameof(OnChanged));
    }
    public abstract void OnChanged();
    public StatEffectTypes StatType { get; set; }
    public int StatTypeIndex { get; set; }
    
    public int MaxValue { get; protected set; }
    public int MinValue { get; protected set; }

    public int Value
    {
        get => _value;
        private set
        {
            _value = value;
            EmitSignal(nameof(Changed), _value);
        }
    }

    public virtual string GetStatName()
    {
        try
        {
            switch (StatType)
            {
                case StatEffectTypes.Stat:
                    return Enum.GetName(typeof(StatTypes), StatTypeIndex) ?? throw new InvalidOperationException();
                case StatEffectTypes.Blessing:
                    return Enum.GetName(typeof(BlessingTypes), StatTypeIndex) ?? throw new InvalidOperationException();
                case StatEffectTypes.Curse:
                    return Enum.GetName(typeof(CurseTypes), StatTypeIndex) ?? throw new InvalidOperationException();
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        catch (Exception e)
        {
            return "";
        }
    }

}