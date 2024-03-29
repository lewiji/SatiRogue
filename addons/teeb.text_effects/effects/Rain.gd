tool
extends RichTextEffect

export var speed = 1.0
export var offset = 2.0
export var factor_t = 1.618
export var target_color = Color8(127, 127, 127, 200)

# Syntax: [rain][/rain]
var bbcode = "rain"

func get_rand(char_fx):
	return fmod(get_rand_unclamped(char_fx), 1.0)


func get_rand_unclamped(char_fx):
	return char_fx.character * 33.33 + char_fx.absolute_index * 4545.5454


func _process_custom_fx(char_fx):
	var time = char_fx.elapsed_time
	var r = get_rand(char_fx)
	var t = fmod(r + time * speed, speed * factor_t)
	char_fx.offset.y += t * offset
	char_fx.color = lerp(char_fx.color, target_color, t)
	return true
