tool
extends Control

export(NodePath) var inc_dec_options_nodepath
export(NodePath) var inc_button_nodepath
export(NodePath) var dec_button_nodepath
export(NodePath) var default_step_lineedit_path

onready var inc_dec_options: OptionButton = get_node(inc_dec_options_nodepath)
onready var inc_button: ToolButton = get_node(inc_button_nodepath)
onready var dec_button: ToolButton = get_node(dec_button_nodepath)
onready var default_step_lineedit: LineEdit = get_node(default_step_lineedit_path)

var _editorInterface: EditorInterface
var _editorSettings: EditorSettings
var _currentValue: float

func set_editor_interface(editorInterface: EditorInterface):
	_editorInterface = editorInterface
	_editorSettings = editorInterface.get_editor_settings()
	get_editor_setting_value()

func _ready():
	inc_button.connect("pressed", self, "on_increase_pressed")
	dec_button.connect("pressed", self, "on_decrease_pressed")
	inc_dec_options.add_item("*/0.1")
	
func get_editor_setting_value():
	_currentValue = _editorSettings.get_setting("interface/inspector/default_float_step")
	default_step_lineedit.text = var2str(_currentValue)

func set_editor_setting_value(value : float = -1) -> void:
	value = _currentValue if -1 else value
	_editorSettings.set_setting("interface/inspector/default_float_step", value)
	get_editor_setting_value()
	
	var inspObj : Object = InspectedObjectGetter.get_inspected_object()
	inspObj.property_list_changed_notify()
	print("Notified")

func on_increase_pressed():
	_currentValue *= 0.1
	set_editor_setting_value()
	pass
	
func on_decrease_pressed():
	_currentValue /= 0.1
	set_editor_setting_value()
	pass
