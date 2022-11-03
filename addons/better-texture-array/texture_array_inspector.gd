tool
extends EditorInspectorPlugin

const Editor = preload("res://addons/better-texture-array/ui/editor.gd")
var undo_redo: UndoRedo

func can_handle(object):
	return object is Texture3D or object is TextureArray

func parse_property(object, type, path, hint, hint_text, usage):
	# Use the `flags` property as our anchor because there is no layers / data prop exported,
	# make sure not to return `true` at the end because that would remove the flags UI
	if path != "flags":
		return
	
	var ed = Editor.new()
	ed.undo_redo = undo_redo
	add_property_editor("data", ed)
