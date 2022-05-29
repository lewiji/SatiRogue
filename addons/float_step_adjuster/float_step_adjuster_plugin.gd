tool
extends EditorPlugin

var dock

func _enter_tree():
	dock = preload("float_step_adjuster_dock.tscn").instance()
	dock.call_deferred("set_editor_interface", get_editor_interface())
	add_control_to_container(EditorPlugin.CONTAINER_PROPERTY_EDITOR_BOTTOM, dock)


func _exit_tree():
	remove_control_from_container(EditorPlugin.CONTAINER_PROPERTY_EDITOR_BOTTOM, dock)
	dock.free()
	pass
