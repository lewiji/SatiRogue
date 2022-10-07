using System;
using Godot;
using Godot.Collections;
using SatiRogue.Ecs.Menu.Nodes;
using RelEcs;
using SatiRogue.Tools;
using World = RelEcs.World;
using Option = SatiRogue.Ecs.Menu.Nodes.Option;

namespace SatiRogue.Ecs.Menu.Systems;

public class InitOptions : Reference, ISystem {
   public World World { get; set; } = null!;
   static readonly PackedScene OptionsScene = GD.Load<PackedScene>("res://src/Ecs/Menu/Nodes/Options.tscn");
   Options? _options;
   readonly string _configPath = "user://satirogue.cfg";

   public void Run() {
      var menuState = World.GetElement<MenuState>();
      _options = OptionsScene.Instance<Options>();
      _options.Connect(nameof(Options.OptionChanged), this, nameof(OnOptionChanged));
      _options.Connect("ready", this, nameof(OnOptionsReady));
      menuState.AddChild(_options);
      World.AddOrReplaceElement(_options);
   }

   ConfigFile GetOrCreateConfigFile() {
      var config = new ConfigFile();

      if (config.Load(_configPath) == Error.Ok)
         return config;
      config.Save(_configPath);
      return config;
   }

   void OnOptionsReady() {
      var optionsChildren = _options!.GetNode<Control>("%OptionsContainer").GetChildren();
      var cfg = GetOrCreateConfigFile();
      var worldEnv = World.GetElement<WorldEnvironment>();

      foreach (Node optionsChild in optionsChildren) {
         if (optionsChild is not Option option)
            return;

         switch (option.OptionLocation) {
            case Option.OptionType.ProjectSetting:
               break;
            case Option.OptionType.EnvironmentSetting:
               SetInitialEnvironmentSetting(option, cfg, worldEnv);
               break;
            default:
               throw new ArgumentOutOfRangeException();
         }
      }
   }

   static void SetInitialEnvironmentSetting(Option option, ConfigFile cfg, WorldEnvironment worldEnv) {
      if (option.CheckBox.Disabled)
         return;
      var splitKeys = option.OptionKey.Split(",");
      var initialVal = GetInitialConfigVal(cfg, option, worldEnv, splitKeys);
      option.GetNode<CheckBox>("%CheckBox").Pressed = (bool) initialVal;

      foreach (var key in splitKeys) {
         worldEnv.Environment.Set(key, initialVal);
      }
   }

   static object GetInitialConfigVal(ConfigFile cfg, Option option, WorldEnvironment worldEnv, string[] splitKeys) {
      var initialVal = cfg.HasSectionKey(option.OptionLocation.ToString(), option.OptionKey)
         ? cfg.GetValue(option.OptionLocation.ToString(), option.OptionKey)
         : (bool) worldEnv.Environment.Get(splitKeys[0]);
      return initialVal;
   }

   void OnOptionChanged(Option.OptionType optionLocation, Dictionary keyValue) {
      GD.Print("Option changed:");
      GD.Print(optionLocation);
      var cfg = GetOrCreateConfigFile();

      foreach (string optionKey in keyValue.Keys) {
         GD.Print(optionKey);
         GD.Print(keyValue[optionKey]);

         var splitKeys = optionKey.Split(",");

         foreach (var splitKey in splitKeys) {
            switch (optionLocation) {
               case Option.OptionType.ProjectSetting:
                  break;
               case Option.OptionType.EnvironmentSetting:
                  World.GetElement<WorldEnvironment>().Environment.Set(splitKey, keyValue[optionKey]);
                  break;
               default:
                  throw new ArgumentOutOfRangeException(nameof(optionLocation), optionLocation, null);
            }
         }

         cfg.SetValue(optionLocation.ToString(), optionKey, keyValue[optionKey]);
         cfg.Save(_configPath);
      }
   }
}