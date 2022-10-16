// NOTE: Shader automatically converted from Godot Engine 3.4.4.stable.mono's SpatialMaterial.

shader_type spatial;
render_mode blend_mix,depth_draw_opaque,cull_back,unshaded,async_visible;
uniform vec4 albedo : hint_color;
uniform sampler2D texture_albedo : hint_albedo;
uniform float specular;
uniform float metallic;
uniform float distance_fade_min;
uniform float distance_fade_max;
uniform float roughness : hint_range(0,1);
uniform float point_size : hint_range(0,128);
uniform sampler2D texture_emission : hint_black_albedo;
uniform vec4 emission : hint_color;
uniform float emission_energy;
uniform float subsurface_scattering_strength : hint_range(0,1);
uniform sampler2D texture_subsurface_scattering : hint_white;
uniform vec3 uv1_scale;
uniform vec3 uv1_offset;
uniform vec3 uv2_scale;
uniform vec3 uv2_offset;


void vertex() {
	ROUGHNESS=roughness;
	UV=UV*uv1_scale.xy+uv1_offset.xy;
}




void fragment() {
	vec2 base_uv = UV;
	vec4 albedo_tex = texture(texture_albedo,base_uv);
	ALBEDO = albedo.rgb * albedo_tex.rgb;
	METALLIC = metallic;
	ROUGHNESS = roughness;
	SPECULAR = specular;
	vec3 emission_tex = texture(texture_emission,base_uv).rgb;
	EMISSION = (emission.rgb+emission_tex)*emission_energy;
	{
		float fade_distance=-VERTEX.z;
		float fade=clamp(smoothstep(distance_fade_min,distance_fade_max,fade_distance),0.0,1.0);
		int x = int(FRAGCOORD.x) % 4;
		int y = int(FRAGCOORD.y) % 4;
		int index = x + y * 4;
		float limit = 0.0;

		if (x < 8) {
			if (index == 0) limit = 0.0625;
			if (index == 1) limit = 0.5625;
			if (index == 2) limit = 0.1875;
			if (index == 3) limit = 0.6875;
			if (index == 4) limit = 0.8125;
			if (index == 5) limit = 0.3125;
			if (index == 6) limit = 0.9375;
			if (index == 7) limit = 0.4375;
			if (index == 8) limit = 0.25;
			if (index == 9) limit = 0.75;
			if (index == 10) limit = 0.125;
			if (index == 11) limit = 0.625;
			if (index == 12) limit = 1.0;
			if (index == 13) limit = 0.5;
			if (index == 14) limit = 0.875;
			if (index == 15) limit = 0.375;
		}

	if (fade < limit)
		discard;
	}

	//float sss_tex = texture(texture_subsurface_scattering,base_uv).r;
	//SSS_STRENGTH=subsurface_scattering_strength*sss_tex;
}
