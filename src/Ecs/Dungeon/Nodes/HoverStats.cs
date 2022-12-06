using System.Globalization;
using Godot;
using SatiRogue.Ecs.Dungeon.Components.Actor;
using SatiRogue.Ecs.Dungeon.Nodes.Hud;

namespace SatiRogue.Ecs.Dungeon.Nodes;

public partial class HoverStats : Control {
   static PackedScene _contentRowScene = GD.Load<PackedScene>("res://src/Ecs/Dungeon/Nodes/HoverStatsKeyValueLabel.tscn");
   Stats.StatsRecordClass? _statsRecordClass;
   Control? _contentParent;

   public override void _Ready() {
      _contentParent = GetNode<Control>("%ContentCol");
   }

   public void SetStatsRecord(Stats.StatsRecordClass statsRecordClass) {
      _statsRecordClass = statsRecordClass;
      AddContentRow("Strength", _statsRecordClass.Strength.ToString());
      AddContentRow("Defence", _statsRecordClass.Defence.ToString());
      AddContentRow("Speed", _statsRecordClass.Speed.ToString());
      AddContentRow("Sight", _statsRecordClass.SightRange.ToString());
   }

   void AddContentRow(string name, string stat) {
      var row = _contentRowScene.Instantiate<HoverStatsKeyValueLabel>();
      _contentParent?.AddChild(row);
      row.SetKeyValue(name, stat);
   }
}