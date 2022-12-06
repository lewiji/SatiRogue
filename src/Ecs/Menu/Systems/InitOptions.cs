using System;
using Godot;
using Godot.Collections;
using SatiRogue.Ecs.Menu.Nodes;
using RelEcs;
using SatiRogue.Tools;
using World = RelEcs.World;
using Option = SatiRogue.Ecs.Menu.Nodes.Option;

namespace SatiRogue.Ecs.Menu.Systems;

public partial class InitOptions : RefCounted, ISystem {
   
   static readonly PackedScene OptionsScene = GD.Load<PackedScene>("res://src/Ecs/Menu/Nodes/Options.tscn");
   Options? _options;
   readonly string _configPath = "user://satirogue.cfg";
   World? _world;
   public void Run(World world)
   {
      _world ??= world;
      var menuState = world.GetElement<MenuState>();
      _options = OptionsScene.Instantiate<Options>();
      _options.Connect(nameof(Options.OptionChanged),new Callable(this,nameof(OnOptionChanged)));
      _options.Connect("ready",new Callable(this,nameof(OnOptionsReady)));
      menuState.AddChild(_options);
      world.AddOrReplaceElement(_options);
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
      var worldEnv = _world!.GetElement<WorldEnvironment>();

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
      option.GetNode<CheckBox>("%CheckBox").ButtonPressed = (bool) initialVal;

      foreach (var key in splitKeys) {
         worldEnv.Environment.Set(key, initialVal);
      }
   }

   static Variant GetInitialConfigVal(ConfigFile cfg, Option option, WorldEnvironment worldEnv, string[] splitKeys) {
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
                  _world!.GetElement<WorldEnvironment>().Environment.Set(splitKey, keyValue[optionKey]);
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