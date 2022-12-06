// NOTE: Shader automatically converted from Godot Engine 3.5.1.stable.mono's SpatialMaterial.

shader_type spatial;
render_mode blend_mix,depth_draw_opaque,cull_back,diffuse_toon,specular_schlick_ggx,shadows_disabled;
uniform vec4 albedo : source_color;
uniform sampler2DArray texture_array;
uniform float specular;
uniform float metallic;
uniform float roughness : hint_range(0,1);
uniform float ao_light_affect;
uniform vec3 uv1_scale;
uniform vec3 uv1_offset;
uniform vec3 uv2_scale;
uniform vec3 uv2_offset;
const vec4 texture_channel = vec4(1.0, 0.0, 0.0, .0);

void vertex() {
	UV=UV*uv1_scale.xy+uv1_offset.xy;
	MODELVIEW_MATRIX = VIEW_MATRIX * mat4(INV_VIEW_MATRIX[0],INV_VIEW_MATRIX[1],INV_VIEW_MATRIX[2],INV_VIEW_MATRIX[3]);
}

void fragment() {
	vec2 base_uv = UV;
	vec4 albedo_tex = texture(texture_array, vec3(base_uv.x, base_uv.y, float(0)));
	if (albedo_tex.a < 0.1) {
		discard;
	} else {
		albedo_tex.rgb = mix(pow((albedo_tex.rgb + vec3(0.055)) * (1.0 / (1.0 + 0.055)),vec3(2.4)),albedo_tex.rgb.rgb * (1.0 / 12.92),lessThan(albedo_tex.rgb,vec3(0.04045)));
		ALBEDO = albedo.rgb * albedo_tex.rgb;
				
		NORMAL_MAP = (texture(texture_array,vec3(base_uv.x, base_uv.y, float(1))).xyz * 2.0);
		NORMAL_MAP_DEPTH = 0.8;
		
		float metallic_tex = dot(texture(texture_array,vec3(base_uv.x, base_uv.y, float(4))),texture_channel);
		METALLIC = metallic_tex * metallic;

		float roughness_tex = dot(texture(texture_array,vec3(base_uv.x, base_uv.y, float(3))),texture_channel);
		ROUGHNESS = roughness_tex * roughness;
		SPECULAR = specular;
		AO = dot(texture(texture_array,vec3(base_uv.x, base_uv.y, float(2))),texture_channel);
		AO_LIGHT_AFFECT = ao_light_affect;
	}
}
