tool
extends EditorPlugin

var dock
var editorInspectorPlugin = preload("res://addons/float_step_adjuster/editor_inspector_plugin.gd").new()
	


func _enter_tree():
	dock = preload("float_step_adjuster_dock.tscn").instance()
	add_inspector_plugin(editorInspectorPlugin)
	dock.call_deferred("set_editor_interface", get_editor_interface())
	add_control_to_container(EditorPlugin.CONTAINER_PROPERTY_EDITOR_BOTTOM, dock)


func _exit_tree():
	remove_control_from_container(EditorPlugin.CONTAINER_PROPERTY_EDITOR_BOTTOM, dock)
	dock.free()
	remove_inspector_plugin(editorInspectorPlugin)
	pass
