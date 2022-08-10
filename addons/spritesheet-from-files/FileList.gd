tool
extends ScrollContainer

onready var file_list_container  = $FileListContainer

func _on_options_changed(width : int, height : int, files_per_row : int):
	file_list_container.columns = files_per_row

func _on_files_selected(files : Array, _biggest_size):
	_clear_files()
	for file in files:
		file_list_container.add_child(_create_file(file))

func _create_file(file) -> Control:
	var r = TextureRect.new()
	r.texture = file
	r.expand = true
#	r.stretch_mode = TextureRect.STRETCH_KEEP_CENTERED
	r.stretch_mode = TextureRect.STRETCH_KEEP_ASPECT
	r.size_flags_horizontal = 3
	r.size_flags_vertical = 3
	return r

func _clear_files():
	for child in file_list_container.get_children():
		child.queue_free()
