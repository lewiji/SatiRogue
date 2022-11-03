// NOTE: Shader automatically converted from Godot Engine 3.4.4.stable.mono's SpatialMaterial.

shader_type spatial;
render_mode blend_mix,depth_test_disable,cull_back,diffuse_lambert,specular_disabled,async_visible,shadows_disabled;
uniform vec4 albedo : hint_color;
uniform sampler2D texture_albedo : hint_albedo;
uniform float alpha_scissor_threshold;
uniform vec4 emission : hint_color;
uniform float emission_energy;
uniform float percent_full;

varying vec3 atlas_uv;
varying vec3 atlas_uv_offset;
varying float is_progress_bar;
varying vec2 orig_uv;

void vertex() {
	atlas_uv = vec3(INSTANCE_CUSTOM.r, 1, 1);
	atlas_uv_offset = vec3(INSTANCE_CUSTOM.g, 0, 0);
	orig_uv = UV;
	UV=UV*atlas_uv.xy+atlas_uv_offset.xy;
	is_progress_bar = INSTANCE_CUSTOM.b;
	MODELVIEW_MATRIX = INV_CAMERA_MATRIX * mat4(CAMERA_MATRIX[0],CAMERA_MATRIX[1],CAMERA_MATRIX[2],WORLD_MATRIX[3]);
	MODELVIEW_MATRIX = MODELVIEW_MATRIX * mat4(vec4(length(WORLD_MATRIX[0].xyz), 0.0, 0.0, 0.0),vec4(0.0, length(WORLD_MATRIX[1].xyz), 0.0, 0.0),vec4(0.0, 0.0, length(WORLD_MATRIX[2].xyz), 0.0),vec4(0.0, 0.0, 0.0, 1.0));
}

void fragment() {
	vec2 base_uv = UV;
	vec4 albedo_tex = texture(texture_albedo,base_uv);
	
	if (albedo_tex.a <= 0.1f) {
		discard;
	} else {
		if (is_progress_bar > 0.1f) {
			vec2 coords = base_uv;
			vec2 pointOnLineSegment = vec2(clamp(coords.x, 0.5f, 1.f - 0.5f), 0.5f);
			float sdf = distance(pointOnLineSegment, coords) * 2.f - 1.f;
			
			float borderSdf = sdf + 1.f;
			float pd = fwidth(borderSdf);
			float borderMask = 1.f - clamp(borderSdf / pd, 0, 1);
			
			borderSdf = ( (borderSdf - 0.f) / (1.f - 0.f) ) * (1.03f + 0.3f) - 0.3f;
			
			bool fillMask = borderSdf > percent_full;
			
			if (fillMask) {
				discard;
			}
		}
		ALBEDO = albedo.rgb * albedo_tex.rgb;
		
		vec3 emission_tex = albedo_tex.rgb;
		EMISSION = (emission.rgb+emission_tex)*emission_energy;
	}
}
