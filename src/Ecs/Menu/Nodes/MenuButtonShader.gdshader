shader_type canvas_item;
render_mode blend_add;

uniform sampler2D normal_map: hint_normal;

void fragment(){
    vec4 c = texture(TEXTURE, UV) * COLOR;
    COLOR = c;
	NORMALMAP = texture(normal_map, UV).rgb;
}