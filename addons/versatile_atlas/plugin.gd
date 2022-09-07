tool
extends EditorPlugin


const PanelScn = preload("res://addons/versatile_atlas/bottom_panel.tscn")

var BottomPanel: Control
var bottom_btn: ToolButton
var current_obj: VersatileAtlasTexture


func _enter_tree():
	BottomPanel = PanelScn.instance()
	bottom_btn = add_control_to_bottom_panel(BottomPanel, "VersatileAtlasTexture")
	BottomPanel.connect("close", self, "close_bottom")
	BottomPanel.connect("request_changes", self, "_on_request_changes")
	bottom_btn.hide()


func _exit_tree():
	remove_control_from_bottom_panel(BottomPanel)
	BottomPanel.queue_free()


func handles(object):
	return object is VersatileAtlasTexture


func edit(object):
	if object is VersatileAtlasTexture:
		bottom_btn.show()
		if current_obj and current_obj.is_connected("changed", self, "update_bottom"):
			current_obj.disconnect("changed", self, "update_bottom")
		current_obj = object
		current_obj.connect("changed", self, "update_bottom")
		update_bottom()
#		print("EDITED")


func _on_request_changes(rect: Rect2):
	current_obj.region = rect


func update_bottom():
	if not current_obj: return
	BottomPanel.atlas = current_obj.atlas
	BottomPanel.set_used_rect(current_obj.region)
	BottomPanel.set_scroll_bars()
	BottomPanel._on_general_scroll()


func close_bottom():
	bottom_btn.hide()
	hide_bottom_panel()
