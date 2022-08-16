tool
extends EditorPlugin

func get_plugin_name():
	return "laigter_tools"

const DOCK_CACHE_KEY = "dock_instance_ids"

# initialise user settings if needed, and load the dock PackedScene
func _enter_tree():
	initialise_settings()
	add_docks()

# rescan filesystem after laigter has outputted images to the project
func on_images_saved():
	get_editor_interface().get_resource_filesystem().scan()

# add dock scenes to the editor
func add_docks():
	var bottom_panel = preload("scenes/bottom_panel.tscn").instance()
	add_control_to_bottom_panel(bottom_panel, "Laigter Tools")
	# connect "generate" button
	var dock_drag_drop = bottom_panel.get_node("%DragNDrop")
	var dock_preview = bottom_panel.get_node("%PreviewDock")
	dock_preview.editor_filesystem = get_editor_interface().get_resource_filesystem()
	dock_drag_drop.connect("on_images_generated", dock_preview, "on_images_generated")
	dock_preview.connect("on_images_saved", self, "on_images_saved")
	dock_preview.connect("on_reset", dock_drag_drop, "on_reset")
	# save docks' instance ids to temp config
	LTConfig.set_temp_config_value(DOCK_CACHE_KEY, bottom_panel.get_instance_id())

# set some ProjectSettings up, if they don't exist
func initialise_settings():
	for setting in LTConfig.ConfigKeys:
		var setting_id = LTConfig.ConfigKeys[setting]
		var full_name = LTConfig.get_qualified_setting_name(setting_id)
		var config: Dictionary = LTConfig.get_config_defaults()[setting_id]
		
		if (ProjectSettings.has_setting(full_name) or config == null):
			continue
		
		ProjectSettings.set_setting(full_name, config["default"])
		ProjectSettings.set_initial_value(full_name, config["default"])
		ProjectSettings.add_property_info({ 
			"type": config["type"] if config.has("type") else null,
			"hint": config["hint"] if config.has("hint") else null, 
			"name": full_name 
		})
		ProjectSettings.save()

# cleanup
func _exit_tree():
	clear_cache()
	# retrieve saved temp config dock instance id if possible
	var dock_id = LTConfig.get_temp_config_value(DOCK_CACHE_KEY)
	if dock_id == null:
		return
	
	var dockInstance = instance_from_id(dock_id)
	if (is_instance_valid(dockInstance) and dockInstance is Control):
		if (dockInstance.name.begins_with("Laigter")):
			remove_control_from_bottom_panel(dockInstance)
			dockInstance.free()

	# it's too late for you now, instance id
	LTConfig.set_temp_config_value(DOCK_CACHE_KEY, null)

func clear_cache():
	remove_recursive(LTConfig.TEMP_CACHE_DIR)
			
func remove_recursive(path):
	var directory = Directory.new()	
	# Open directory
	var error = directory.open(path)
	if error == OK:
		# List directory content
		directory.list_dir_begin(true)
		var file_name = directory.get_next()
		while file_name != "":
			if directory.current_is_dir():
				remove_recursive(path + "/" + file_name)
			else:
				directory.remove(file_name)
			file_name = directory.get_next()
		
		# Remove current path
		directory.remove(path)
