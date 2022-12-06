shader_type canvas_item;

uniform sampler2D normal_map: hint_normal;
uniform float time_scale_x = 0.08;
uniform float time_scale_y = 0.05;
uniform vec4 shadow_color: source_color = vec4(0.05, 0.05, 0.05, 0.5);

void fragment(){
    vec2 newuv = UV;
    newuv.x -= TIME * time_scale_x;
    newuv.y -= TIME * time_scale_y;
    vec4 c = texture(TEXTURE, newuv);
    COLOR = c;
	NORMAL_MAP = texture(normal_map, newuv).rgb;
}

void light() {
	SHADOW_MODULATE = COLOR - shadow_color;
	LIGHT = SHADOW_MODULATE;
}

/*void fragment(){
    vec4 c = texture(TEXTURE, UV) * COLOR;
	vec4 b = textureLod(SCREEN_TEXTURE, SCREEN_UV, blur_amount);
    COLOR = mix(c, b, interp_amount);
	NORMALMAP = texture(normal_map, UV).rgb;
}*/