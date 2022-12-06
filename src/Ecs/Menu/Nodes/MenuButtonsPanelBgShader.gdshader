shader_type canvas_item;

uniform sampler2D normal_map: hint_normal;
uniform float blur_amount : hint_range(0, 5);
uniform float interp_amount : hint_range(0, 1);

void fragment(){
    vec4 c = texture(TEXTURE, UV) * COLOR;
	vec4 b = textureLod(SCREEN_TEXTURE, SCREEN_UV, blur_amount);
    COLOR = mix(c, b, interp_amount);
	NORMAL_MAP = texture(normal_map, UV).rgb;
}