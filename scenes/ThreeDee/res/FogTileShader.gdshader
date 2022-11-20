// NOTE: Shader automatically converted from Godot Engine 3.4.4.stable.mono's SpatialMaterial.

shader_type spatial;
render_mode blend_mix,depth_draw_opaque,cull_back,unshaded,async_visible;
uniform vec4 albedo : hint_color;

void fragment() {
	vec2 base_uv = UV;
	ALBEDO = albedo.rgb;
	ALPHA = COLOR.a;
}
