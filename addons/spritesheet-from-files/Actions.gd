tool
extends HBoxContainer

signal files_selected(files)
signal process_started(output_file)

export(bool) var enabled : bool = false setget set_enabled, is_enabled

onready var select_files_button : Button = $SelectFilesButton
onready var process_button      : Button = $ProcessButton
var editor_interface

func set_enabled(new_enabled : bool):
	enabled = new_enabled
	if $ProcessButton:
		$ProcessButton.disabled = !enabled

func is_enabled() -> bool: return enabled

func _on_files_selected(files : PoolStringArray):
	var file_list : Array = []
	for file in files: file_list.append(file)
	file_list.sort()
	for i in range(file_list.size()):
		file_list[i] = load(file_list[i])
	var image_list = []
	var biggest_size = Vector2(0,0)
	for i in range(file_list.size()):
		if file_list[i] is Texture:
			image_list.append(file_list[i])
			if file_list[i].get_height() > biggest_size.y:
				biggest_size.y = file_list[i].get_height()
			if file_list[i].get_height() > biggest_size.x:
				biggest_size.x = file_list[i].get_width()
	emit_signal("files_selected", image_list, biggest_size)

func _on_select_files():
	var dialog = FileDialog.new()
	dialog.mode = FileDialog.MODE_OPEN_FILES
	dialog.filters = [ "*.png" ]
	dialog.window_title = "Select PNGs to convert"
	dialog.dialog_text = "You can select one or more PNG files to create a spritesheet"
	dialog.connect('modal_closed', dialog, 'queue_free')
	dialog.connect("files_selected", self, "_on_files_selected")
	var path : String = editor_interface.get_selected_path()
	dialog.current_path = path
	add_child(dialog)
	dialog.popup_centered_ratio()

func _on_output_file_selected(path : String):
	emit_signal("process_started", path)

func _on_process():
	var dialog = FileDialog.new()
	dialog.mode = FileDialog.MODE_SAVE_FILE
	dialog.filters = [ "*.png" ]
	dialog.window_title = "Save your spritesheet"
	dialog.dialog_text = "Select a directory and save your spritesheet"
	var path = editor_interface.get_selected_path()
	var p = path
	p.erase(0,6)
	var splits = p.split("/", false)
	if path != "res://":
		path = "res://"
		for i in range(splits.size() -1):
			path += splits[i] + "/"
	dialog.current_path = path
	if splits.size()> 1:
		var new_file_name = splits[splits.size() -1]
		if new_file_name.find(" ") != -1:
			new_file_name.to_lower() 
		new_file_name.capitalize()
		new_file_name = new_file_name.replace(" ", "")
		dialog.current_file = new_file_name + ".png"
	else:
		dialog.current_file = "Spritesheet.png" 
	dialog.connect('modal_closed', dialog, 'queue_free')
	dialog.connect("file_selected", self, "_on_output_file_selected")
	add_child(dialog)
	dialog.popup_centered_ratio()

func _ready():
	select_files_button.connect("pressed", self, "_on_select_files")
	process_button.connect("pressed", self, "_on_process")
	select_files_button.disabled = false
	process_button.disabled = true
