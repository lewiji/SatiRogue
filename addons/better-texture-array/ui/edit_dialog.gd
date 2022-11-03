tool
extends EditorFileDialog

const Layer = preload("res://addons/better-texture-array/ui/layer.gd")
var channel_option: OptionButton
var channel_option_box: HBoxContainer
var selected_channel: int
var layer

func _init():
	window_title = "Load image..."
	mode = EditorFileDialog.MODE_OPEN_FILE
	add_filter("*.png, *.jpg, *.webp, *.exr; Images")
	connect("file_selected", self, "_on_file_selected")
	
	var file_dialog_box = get_vbox()
	channel_option_box = HBoxContainer.new()
	channel_option_box.alignment = BoxContainer.ALIGN_END
	var channel_option_label = Label.new()
	channel_option_label.text = "Choose the channel"
	
	channel_option = OptionButton.new()
	channel_option.add_item("RED", Layer.Channels.RED)
	channel_option.add_item("GREEN", Layer.Channels.GREEN)
	channel_option.add_item("BLUE", Layer.Channels.BLUE)
	channel_option.add_item("ALPHA", Layer.Channels.ALPHA)
	
	channel_option_box.add_child(channel_option_label)
	channel_option_box.add_child(channel_option)
	file_dialog_box.add_child(channel_option_box)

func _exit_tree():
	disconnect("file_selected", self, "_on_file_selected")

func popup_for_layer(lyr):
	layer = lyr
	# Only show the channel selection box if you have selected
	# a channel to view on the layer
	channel_option_box.visible = lyr.channel != Layer.Channels.ALL
	popup_centered_ratio()

func _on_file_selected(path):
	var src = load(path)
	var img = src if src is Image else src.get_data()
	layer.update_layer(img, channel_option.get_selected_id())
