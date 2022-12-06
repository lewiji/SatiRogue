shader_type spatial;

render_mode blend_mix, depth_draw_opaque, cull_back, diffuse_lambert, specular_schlick_ggx;

uniform vec4 albedo : source_color = vec4(1.0, 1.0, 1.0, 1.0);
uniform sampler2D pbr_atlas;

uniform vec2 tile_size = vec2(32.0, 32.0);
uniform vec2 atlas_size = vec2(2048, 256);

varying float custom_tile_index;
varying vec2 tile_offset;
varying vec2 pixel_offset;

void vertex() {
	custom_tile_index = floor(INSTANCE_CUSTOM.r * 255.0);
	pixel_offset = 1.0 / atlas_size;
	tile_offset = tile_size / atlas_size;
	float albedo_tile_index = custom_tile_index * 5.0;
	vec2 albedo_tile_coords = vec2(mod(albedo_tile_index, 60.0), floor(albedo_tile_index / 60.0));
	
	vec3 uv1_scale = vec3(tile_size.x / atlas_size.x, tile_size.y / atlas_size.y, 1.0);
	vec3 uv1_offset = vec3((pixel_offset.x * ((albedo_tile_coords.x + 0.5) * 2.0)) + (albedo_tile_coords.x * tile_offset.x), (pixel_offset.y * ((albedo_tile_coords.y + 0.5) * 2.0)) + (albedo_tile_coords.y * tile_offset.y), 0.0);
	
	UV=UV*uv1_scale.xy+uv1_offset.xy;
}

// a,n,o,p,s

void fragment() {
	vec2 base_uv = UV;
	vec4 albedo_tex = texture(pbr_atlas, base_uv);
	albedo_tex.rgb = mix(pow((albedo_tex.rgb + vec3(0.055)) * (1.0 / (1.0 + 0.055)),vec3(2.4)),albedo_tex.rgb.rgb * (1.0 / 12.92),lessThan(albedo_tex.rgb,vec3(0.04045)));
	
	float alpha = albedo_tex.a;
	if (alpha > 0.5) {
		ALBEDO = albedo.rgb * albedo_tex.rgb;
	} else {
		ALPHA_SCISSOR_THRESHOLD = 0.5;
		ALPHA = 0.0;
	}
	
	vec4 normal_tex = texture(pbr_atlas,vec2(base_uv.x + tile_offset.x + pixel_offset.x * 2.0, base_uv.y));
	NORMAL_MAP = normal_tex.rgb;
	NORMAL_MAP_DEPTH = 1.0;
	
	SPECULAR = 0.5;
	
	vec4 occ_tex = texture(pbr_atlas,vec2(base_uv.x + (tile_offset.x * 2.0) + (pixel_offset.x * 4.0), base_uv.y));
	AO = dot(occ_tex,vec4(1.0, 0.0, 0.0, 0.0));
	AO_LIGHT_AFFECT = 0.3;
	
	vec4 metallic_tex = texture(pbr_atlas,vec2(base_uv.x + (tile_offset.x * 3.0) + (pixel_offset.x * 6.0), base_uv.y));
	METALLIC = dot(metallic_tex,vec4(1.0, 0.0, 0.0, 0.0)) * -1.0;
	
	
	vec4 roughness_tex = texture(pbr_atlas,vec2(base_uv.x + (tile_offset.x * 4.0) + (pixel_offset.x * 8.0), base_uv.y));
	ROUGHNESS = dot(roughness_tex,vec4(1.0, 0.0, 0.0, 0.0)) * 1.0;
	
	
	
}