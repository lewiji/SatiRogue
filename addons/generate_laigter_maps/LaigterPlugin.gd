tool
extends EditorPlugin

var settings: LaigterSettings = LaigterSettings.new()
const dock_instance_ids = "dock_instance_id"

func get_plugin_name():
   return "laigter_godot_tools"

# initialise user settings if needed, and load the dock PackedScene
func _enter_tree():
   initialise_settings();
   var dock = preload("LaigterDock.tscn").instance()
   var dockPreview = preload("LaigterPreviewDock.tscn").instance()
   add_control_to_dock(EditorPlugin.DOCK_SLOT_LEFT_BR, dock)
   add_control_to_bottom_panel(dockPreview, "LaigterTools Preview")
   # save dock's instance id to temp config
   settings.set_temp_config_value(dock_instance_ids, [dock.get_instance_id(), dockPreview.get_instance_id()])
   dock.setup_options(settings)
   dock.connect("on_images_generated", dockPreview, "on_images_generated")
   

# cleanup
func _exit_tree():
   # retrieve saved temp config dock instance id if possible
   var dock_id = settings.get_temp_config_value(dock_instance_ids)
   if dock_id == null:
      return
   
   for dock_instance_id in dock_id:
      var dockInstance = instance_from_id(int(dock_instance_id))
      if (is_instance_valid(dockInstance) and dockInstance is Control):
         match (dockInstance as Control).name:
            "LaigterDock":
               remove_control_from_docks(dockInstance)
            "LaigterPreviewDock":
               remove_control_from_bottom_panel(dockInstance)
         dockInstance.free()

   # it's too late for you now, instance id
   settings.set_temp_config_value(dock_instance_ids, null)
   
# set some ProjectSettings up, if they don't exist
func initialise_settings():
   for setting in LaigterSettings.PROJECT_SETTING:
      var setting_id = LaigterSettings.PROJECT_SETTING[setting]
      var full_name = LaigterSettings.get_qualified_setting_name(setting_id)
      if (ProjectSettings.has_setting(full_name)):
         continue              
      
      ProjectSettings.set_setting(full_name, LaigterSettings.SettingDefaults[setting_id])
      ProjectSettings.set_initial_value(full_name, LaigterSettings.SettingDefaults.get(setting_id))
            
      var property_info = {}
      if LaigterSettings.SettingTypes.has(setting_id):
         property_info["type"] = LaigterSettings.SettingTypes[setting_id]
                     
      if LaigterSettings.SettingPropertyHints.has(setting_id):
         property_info["hint"] = LaigterSettings.SettingPropertyHints[setting_id ]
         
      if (property_info.size() > 0):
         property_info["name"] = full_name
         ProjectSettings.add_property_info(property_info)
         
      ProjectSettings.save()
