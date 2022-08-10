tool
extends EditorPlugin

var cpm

func _enter_tree():
	cpm = preload("res://addons/spritesheet-from-files/SpritesheetFromFiles.tscn").instance()
#	cpm.undoredo = get_undo_redo()
	cpm.editor_interface = get_editor_interface()
	add_control_to_bottom_panel(cpm, "Spritesheet")

func _exit_tree():
	remove_control_from_bottom_panel(cpm)
#	cpm.free()
