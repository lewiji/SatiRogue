#Zoom/Mouse Position addon by Nobuyuki
#For more stuff check out https://github.com/nobuyukinyuu
tool
extends EditorPlugin


var c # A class member to hold the widget control during the plugin lifecycle

func _enter_tree():
	print("Activating zoomy..")
	# Initialization of the plugin goes here
	self.c = load("res://addons/zoomy/Zoom Level.tscn").instance()

	add_control_to_container(CONTAINER_CANVAS_EDITOR_BOTTOM, c)
	
#	c.editorb = self.get_editor_interface()
	c.activate()

	#Godot 3 specific
	connect("main_screen_changed",c, "refresh")
	connect("scene_changed",c, "refresh")

#	#Godot 4 specific
#	connect("main_screen_changed", Callable(c, "refresh"))
#	connect("scene_changed", Callable(c, "refresh"))


func _exit_tree():
	# Clean-up of the plugin goes here
	print("Deactivating zoomy..")
	remove_control_from_container(CONTAINER_CANVAS_EDITOR_BOTTOM, c) # Remove the dock
	c.queue_free() # Erase the control from the memory
	
