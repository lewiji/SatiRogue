shader_type canvas_item;

uniform sampler2D normal_map: hint_normal;
uniform float time_scale_x = 0.08;
uniform float time_scale_y = 0.05;

void fragment(){
    vec2 newuv = UV;
    newuv.x -= TIME * time_scale_x;
    newuv.y -= TIME * time_scale_y;
    vec4 c = texture(TEXTURE, newuv);
    COLOR = c;
	NORMALMAP = texture(normal_map, newuv).rgb;
}