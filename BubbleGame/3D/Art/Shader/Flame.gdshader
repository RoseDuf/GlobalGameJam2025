shader_type spatial;
render_mode cull_disabled, unshaded;


uniform sampler2D noiseTexture;
uniform sampler2D distortionRamp;
uniform sampler2D colorRamp;
uniform float distortionStrength = 0.2;

void vertex() {
	VERTEX += (NORMAL* (textureLod(noiseTexture,UV + TIME,0).x  + (sin(UV.y*10.14 + TIME*2.)*.5 + .5)))*distortionStrength *COLOR.x * textureLod(distortionRamp, vec2(COLOR.x,0),0).x;
	// Called for every vertex the material is visible on.
}

void fragment() {
	
	float noise = texture(noiseTexture, UV + vec2(0.,TIME)).x * (1. -COLOR.y*.7);
	float noise_2 =	texture(noiseTexture, UV*1.3 + vec2(0.,TIME*1.2)).x * (1. -COLOR.y*.7);
noise = mix(noise,noise_2,0.5) + (1. - COLOR.x)*.3;
	float noise_mask = smoothstep(0.45,0.46,noise);
	vec3 flame_color = texture(colorRamp, vec2(1. -noise*noise,0)).rgb;
	
	ALBEDO = flame_color;
	ALPHA = noise_mask * smoothstep(.0,.1,COLOR.x) * smoothstep(0.4,1.,1. - COLOR.x);
	// Called for every pixel the material is visible on.
}

//void light() {
	// Called for every pixel for every light affecting the material.
	// Uncomment to replace the default light processing function with this one.
//}
