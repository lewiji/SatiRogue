tool
extends Panel

signal save_spatial_mat_changed(checked)

onready var checkbox_save_mat: CheckBox = get_node("%SaveMaterial")

func _ready():
	checkbox_save_mat.connect("toggled", self, "on_save_toggled")
	
func on_save_toggled(checked: bool):
	emit_signal("save_spatial_mat_changed", checked)
