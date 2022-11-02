// NOTE: Shader automatically converted from Godot Engine 3.5.1.stable.mono's SpatialMaterial.

shader_type spatial;
render_mode async_visible,blend_mix,depth_draw_opaque,cull_back,diffuse_toon,specular_schlick_ggx,shadows_disabled;
uniform vec4 albedo : hint_color;
uniform sampler2D texture_atlas;
uniform float specular;
uniform float metallic;
uniform float roughness : hint_range(0,1);
uniform float ao_light_affect;
uniform vec3 uv1_scale;
uniform vec3 uv1_offset;
uniform vec3 uv2_scale;
uniform vec3 uv2_offset;
const vec4 texture_channel = vec4(1.0, 0.0, 0.0, 0.0);
uniform vec2 normal_offset;
uniform vec2 occ_offset;
uniform vec2 rough_offset;
uniform vec2 met_offset;

void vertex() {
	UV=UV*uv1_scale.xy+uv1_offset.xy;
	MODELVIEW_MATRIX = INV_CAMERA_MATRIX * mat4(CAMERA_MATRIX[0],CAMERA_MATRIX[1],CAMERA_MATRIX[2],WORLD_MATRIX[3]);
}

void fragment() {
	vec2 base_uv = UV;
	vec4 albedo_tex = texture(texture_atlas,base_uv);
	if (albedo_tex.a < 0.1) {
		discard;
	} else {
		albedo_tex.rgb = mix(pow((albedo_tex.rgb + vec3(0.055)) * (1.0 / (1.0 + 0.055)),vec3(2.4)),albedo_tex.rgb.rgb * (1.0 / 12.92),lessThan(albedo_tex.rgb,vec3(0.04045)));
		ALBEDO = albedo.rgb * albedo_tex.rgb;
		float metallic_tex = dot(texture(texture_atlas,base_uv + met_offset),texture_channel);
		METALLIC = metallic_tex * metallic;
		float roughness_tex = dot(texture(texture_atlas,base_uv + rough_offset),texture_channel);
		ROUGHNESS = roughness_tex * roughness;
		SPECULAR = specular;
		NORMALMAP = texture(texture_atlas,base_uv + normal_offset).rgb;
		NORMALMAP_DEPTH = 1.0;
		AO = dot(texture(texture_atlas,base_uv + occ_offset),texture_channel);
		AO_LIGHT_AFFECT = ao_light_affect;
	}
}
