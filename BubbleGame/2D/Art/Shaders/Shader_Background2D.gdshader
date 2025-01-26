shader_type canvas_item;
uniform float tilling = 0.01;
uniform sampler2D voronoi_mask:repeat_enable;
uniform sampler2D mask_noise:repeat_enable;
uniform sampler2D ramp_texture:repeat_enable;

uniform float offset_strength = 10;

varying vec2 world_pos;


mat2 rotate2d(float angle){
    return mat2(vec2(cos(angle),-sin(angle)),vec2(sin(angle),cos(angle)));
}

void vertex() {
world_pos =(MODEL_MATRIX * vec4(VERTEX, 0.0, 1.0)).xy;

	
	// Called for every vertex the material is visible on.
}

void fragment() {
	vec2 baseUv = world_pos*0.0005;
	float maskNoise = texture(mask_noise, world_pos*0.001).x;
	vec3 voronoiMask = texture(voronoi_mask,world_pos*0.0002 + maskNoise*0.01).xyz;
	
	mat2 rot = rotate2d(voronoiMask.x + voronoiMask.y);
	
	vec2 uvOffset = voronoiMask.xy * offset_strength;
	float voronoiEdge =  voronoiMask.z;
	float greyscale = max(0, mix(texture(TEXTURE,rot * (world_pos*0.02 + uvOffset) * tilling) , texture(TEXTURE,baseUv),voronoiEdge).x);
	
	vec3 color = texture(ramp_texture, vec2(greyscale,0)).xyz;
	float opacity = 1.;
	
	
	COLOR = vec4(color,opacity);
	// Called for every pixel the material is visible on.
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
