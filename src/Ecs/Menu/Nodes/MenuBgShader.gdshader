shader_type canvas_item;

uniform sampler2D normal_map: hint_normal;

void fragment(){
    vec2 newuv = UV;
    newuv.x -= TIME * 0.08;
    newuv.y -= TIME * 0.05;
    vec4 c = texture(TEXTURE, newuv);
    COLOR = c;
	NORMALMAP = texture(normal_map, newuv).rgb;
}