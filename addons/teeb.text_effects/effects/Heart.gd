tool
extends RichTextEffect


# Syntax: [heart scale=1.0 freq=8.0][/heart]
var bbcode = "heart"

const HEART = ord("♡")
const TO_CHANGE = [ord("o"), ord("O"), ord("a"), ord("A")]

func _process_custom_fx(char_fx):
	var scale:float = char_fx.env.get("scale", 16.0)
	var freq:float = char_fx.env.get("freq", 2.0)

	var x =  char_fx.absolute_index / scale - char_fx.elapsed_time * freq
	var t = abs(cos(x)) * max(0.0, smoothstep(0.712, 0.99, sin(x))) * 2.5;
	char_fx.color = lerp(char_fx.color, lerp(Color.blue, Color.red, t), t)
	char_fx.offset.y -= t * 4.0
	
	var c = char_fx.character
	if char_fx.offset.y < -1.0:
		if char_fx.character in TO_CHANGE:
			char_fx.character = HEART
	
	return true
