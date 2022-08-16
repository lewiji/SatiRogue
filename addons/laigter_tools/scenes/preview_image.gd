tool
extends Control

signal save_changed(checked)

onready var checkbox_save_image: CheckBox = get_node("%SaveImage")
onready var checkbox_pixellated: CheckBox = get_node("%Pixel")
onready var texture_rect: TextureRect = get_node("%TextureRect")
onready var fade_color: ColorRect = get_node("%FadeColor")

func _ready():
	checkbox_save_image.connect("toggled", self, "on_save_toggled")
	checkbox_pixellated.connect("toggled", self, "on_pixel_toggled")
	
func on_save_toggled(checked: bool):
	fade_color.visible = !checked
	emit_signal("save_changed", checked)
	
func on_pixel_toggled(checked: bool):
	if (checked):
		texture_rect.texture.flags = Texture.FLAG_ANISOTROPIC_FILTER | Texture.FLAG_MIPMAPS
	else:
		texture_rect.texture.flags = Texture.FLAG_ANISOTROPIC_FILTER | Texture.FLAG_MIPMAPS | Texture.FLAG_FILTER
