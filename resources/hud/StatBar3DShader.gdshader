// NOTE: Shader automatically converted from Godot Engine 3.4.4.stable.mono's SpatialMaterial.

shader_type spatial;
render_mode blend_mix,depth_draw_opaque,cull_back,diffuse_lambert_wrap,specular_schlick_ggx;
uniform vec4 albedo : hint_color;
uniform sampler2D texture_albedo : hint_albedo;
uniform float specular;
uniform float metallic;
uniform float alpha_scissor_threshold;
uniform float roughness : hint_range(0,1);
uniform sampler2D texture_metallic : hint_white;
uniform vec4 metallic_texture_channel;
uniform sampler2D texture_roughness : hint_white;
uniform vec4 roughness_texture_channel;
uniform sampler2D texture_emission : hint_black_albedo;
uniform vec4 emission : hint_color;
uniform float emission_energy;
uniform sampler2D texture_normal : hint_normal;
uniform float normal_scale : hint_range(-16,16);
uniform sampler2D texture_ambient_occlusion : hint_white;
uniform vec4 ao_texture_channel;
uniform float ao_light_affect;
uniform sampler2D texture_depth : hint_black;
uniform sampler2D texture_progress_gradient;
uniform float depth_scale;
uniform int depth_min_layers;
uniform int depth_max_layers;
uniform vec2 depth_flip;
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
}

void fragment() {
	vec2 base_uv = UV;
	{
		vec3 view_dir = normalize(normalize(-VERTEX)*mat3(TANGENT*depth_flip.x,-BINORMAL*depth_flip.y,NORMAL));
		float num_layers = mix(float(depth_max_layers),float(depth_min_layers), abs(dot(vec3(0.0, 0.0, 1.0), view_dir)));
		float layer_depth = 1.0 / num_layers;
		float current_layer_depth = 0.0;
		vec2 P = view_dir.xy * depth_scale;
		vec2 delta = P / num_layers;
		vec2 ofs = base_uv;
		float depth = textureLod(texture_depth, ofs, 0.0).r;
		float current_depth = 0.0;
		while(current_depth < depth) {
			ofs -= delta;
			depth = textureLod(texture_depth, ofs, 0.0).r;
			current_depth += layer_depth;
		}
		vec2 prev_ofs = ofs + delta;
		float after_depth  = depth - current_depth;
		float before_depth = textureLod(texture_depth, prev_ofs, 0.0).r - current_depth + layer_depth;
		float weight = after_depth / (after_depth - before_depth);
		ofs = mix(ofs,prev_ofs,weight);
		base_uv=ofs;
	}
	vec4 albedo_tex = texture(texture_albedo,base_uv);
	
	if (is_progress_bar > 0.1f && albedo_tex.a > 0f) {
		vec4 progress_gradient_tex = texture(texture_progress_gradient, orig_uv);
		if (all(lessThanEqual(progress_gradient_tex.rgb, vec3(0.1f, 0.1f, 0.1f)))) {
			discard;
		}
	}
	ALBEDO = albedo.rgb * albedo_tex.rgb;
	float metallic_tex = dot(texture(texture_metallic,base_uv),metallic_texture_channel);
	METALLIC = metallic_tex * metallic;
	float roughness_tex = dot(texture(texture_roughness,base_uv),roughness_texture_channel);
	ROUGHNESS = roughness_tex * roughness;
	SPECULAR = specular;
	NORMALMAP = texture(texture_normal,base_uv).rgb;
	NORMALMAP_DEPTH = normal_scale;
	vec3 emission_tex = texture(texture_emission,base_uv).rgb;
	EMISSION = (emission.rgb+emission_tex)*emission_energy;
	ALPHA = albedo.a * albedo_tex.a;
	AO = dot(texture(texture_ambient_occlusion,base_uv),ao_texture_channel);
	AO_LIGHT_AFFECT = ao_light_affect;
	ALPHA_SCISSOR=alpha_scissor_threshold;
}
