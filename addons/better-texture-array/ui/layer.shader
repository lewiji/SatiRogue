shader_type canvas_item;

uniform bool is_3d = false;
uniform sampler3D tex3d : hint_black;
uniform sampler2DArray texarr : hint_black;
uniform float idx = 0;
uniform vec4 chn = vec4(1);

void fragment() {
	vec4 lyr;
	if (is_3d) {
		lyr = texture(tex3d, vec3(UV, idx));
	} else {
		lyr = texture(texarr, vec3(UV, idx));
	}
	
//	COLOR = all(bvec4(chn)) ? lyr : vec4(dot(lyr, chn));
	bvec4 bchn = bvec4(chn);
	lyr = lyr * chn;
	COLOR = all(bchn) ? lyr : (all(not(bchn.rgb)) && bchn.a ? vec4(lyr.a) : vec4(lyr.rgb, 1));
}