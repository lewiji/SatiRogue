// NOTE: Shader automatically converted from Godot Engine 3.4.4.stable.mono's SpatialMaterial.

shader_type spatial;
render_mode blend_mix,depth_draw_never,cull_back,unshaded,depth_test_disabled;
uniform vec4 albedo : source_color;

void fragment() {
	vec2 base_uv = UV;
	ALBEDO = albedo.rgb;
	ALPHA = COLOR.a;
}
