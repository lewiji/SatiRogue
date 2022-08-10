tool
extends VBoxContainer

signal changed(width, height, items_per_row)

export(bool) var enabled : bool = false setget set_enabled, is_enabled

onready var width_value        : SpinBox = $WidthValue
onready var height_value       : SpinBox = $HeightValue
onready var items_column_value : SpinBox = $ItemsColumnValue

func set_enabled(new_enabled : bool):
	enabled = new_enabled
	if $WidthValue and $HeightValue and $ItemsColumnValue:
		$WidthValue.editable = enabled
		$HeightValue.editable = enabled
		$ItemsColumnValue.editable = enabled
		_on_changed(.0)

func is_enabled() -> bool: return enabled

func _on_changed(new_value : float):
	emit_signal("changed",
			int(width_value.value),
			int(height_value.value),
			int(items_column_value.value))

func _on_files_selected(files : Array, biggest_size):
	if int(items_column_value.value) > files.size():
		items_column_value.value = files.size()
	$WidthValue.value = biggest_size.x
	$HeightValue.value = biggest_size.y
	$ItemsColumnValue.value = _calculate_optimal_frames_per_row(files.size())

func _ready():
	width_value.connect("value_changed", self, "_on_changed")
	height_value.connect("value_changed", self, "_on_changed")
	items_column_value.connect("value_changed", self, "_on_changed")
	width_value.editable = false
	height_value.editable = false
	items_column_value.editable = false

func _calculate_optimal_frames_per_row(size: int):
	var biggest_int = 1
	for i in range(1, size):
		if size % i == 0:
			biggest_int = i
	return biggest_int
