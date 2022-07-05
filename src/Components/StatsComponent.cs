using System;
using Godot;
using GodotOnReady.Attributes;
using SatiRogue.Components.Stats;
using SatiRogue.Entities;

namespace SatiRogue.Components;

public abstract partial class StatsComponent : Component
{
    private int _value;
    public StatEffectTypes StatType { get; set; }
    public int StatTypeIndex { get; set; }
    public int MaxValue { get; protected set; }
    public int MinValue { get; protected set; }
    public bool CanBeNegative { get; protected set; }
    public int Value
    {
        get => _value;
        private set {
            if (!IsInstanceValid(this)) return;
            if (value <= MinValue && !CanBeNegative) {
                _value = MinValue;
                EmitSignal(nameof(Changed), _value);
                EmitSignal(nameof(Depleted));
            } else {
                var diff = value - _value;
                _value = value;
                
                EmitSignal(nameof(Changed), _value);
                if (diff < 0f) {
                    EmitSignal(nameof(TookDamage), -diff);
                }
            }
        }
    }

    public Entity Entity => (Entity)EcOwner!;

    protected StatsComponent(StatEffectTypes statType, int statTypeIndex, int maxValue, int minValue = 0, int? initialValue = null)
    {
        StatType = statType;
        StatTypeIndex = statTypeIndex;
        MaxValue = maxValue;
        MinValue = minValue;
        Value = initialValue ?? minValue;
    }
    
    [Signal] public delegate void Changed(int newValue);
    [Signal] public delegate void TookDamage(int damage);
    [Signal] public delegate void Depleted();

    public abstract void OnChanged(int newValue);
    public abstract void OnDepleted();

    public void Subtract(int deductBy = 1) {
        Value -= deductBy;
    }
    
    public void Add(int increaseBy = 1) {
        Value += increaseBy;
    }
    
    public float Percentage {
        get => (float) Value / MaxValue;
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