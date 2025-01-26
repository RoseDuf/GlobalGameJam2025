shader_type canvas_item;
uniform sampler2D normal_map:hint_normal, repeat_enable;
uniform sampler2D colorRamp;
uniform sampler2D noise_tex:repeat_enable;
uniform vec3 albedo : source_color = vec3(0.02, 0.45, 0.8);
uniform float vertexDistortion = 0.1;
uniform float distortion = 0.1;
uniform vec3 spec_color:source_color = vec3(0.6,.6,0.8);
uniform vec3 lightDir = vec3(1,-1,1);


uniform float opacity;

varying vec2 world_pos;


float random (in vec2 _st) {
    return fract(sin(dot(_st.xy,
                         vec2(12.9898,78.233)))*
        43758.5453123);
}

// Based on Morgan McGuire @morgan3d
// https://www.shadertoy.com/view/4dS3Wd
float noise (in vec2 _st) {
    vec2 i = floor(_st);
    vec2 f = fract(_st);

    // Four corners in 2D of a tile
    float a = random(i);
    float b = random(i + vec2(1.0, 0.0));
    float c = random(i + vec2(0.0, 1.0));
    float d = random(i + vec2(1.0, 1.0));

    vec2 u = f * f * (3.0 - 2.0 * f);

    return mix(a, b, u.x) +
            (c - a)* u.y * (1.0 - u.x) +
            (d - b) * u.x * u.y;
}

#define NUM_OCTAVES 5

float fbm ( in vec2 _st) {
    float v = 0.0;
    float a = 0.5;
    vec2 shift = vec2(100.0);
    // Rotate to reduce axial bias
    mat2 rot = mat2(vec2(cos(0.5), sin(0.5)),vec2( -sin(0.5), cos(0.50)));
    for (int i = 0; i < NUM_OCTAVES; ++i) {
        v += a * noise(_st);
        _st = rot * _st * 2.0 + shift;
        a *= 0.5;
    }
    return v;
}

float fbmNoise(vec2 uv, float time) {
	
	vec2 st = uv;
    // st += st * abs(sin(u_time*0.1)*3.0);
    vec3 color = vec3(0.0);

    vec2 q = vec2(0.);
    q.x = fbm( st + 0.00*time);
    q.y = fbm( st + vec2(1.0));

    vec2 r = vec2(0.);
    r.x = fbm( st + 1.0*q + vec2(1.7,9.2)+ 0.15*time );
    r.y = fbm( st + 1.0*q + vec2(8.3,2.8)+ 0.126*time);

    float f = fbm(st+r);
	
	return f;
}


float get_fresnel(float amount, vec3 normal, vec3 view)
{
	return pow((1.0 - clamp(dot(normalize(normal), normalize(view)), 0.0, 1.0 )), amount);
}

vec3 DecodeNormals(vec2 n){
	n = n*2. - 1.;
	float z = sqrt(1. - n.x*n.x - n.y * n.y);
	return vec3(n.x,n.y,z);
} 

void vertex() {
	
	world_pos =(MODEL_MATRIX * vec4(VERTEX, 0.0, 1.0)).xy;
	// Called for every vertex the material is visible on.
}

void fragment() {
	
	vec2 uv = UV - 0.5;
	
	float n = texture(noise_tex,world_pos*0.002 + TIME*0.1).x *2. - 1.;
	
	
	vec3 normal1 =normalize(DecodeNormals( texture(normal_map,UV*2. + TIME).xy)*.01);
	vec3 normal2 = normalize(DecodeNormals(texture(normal_map,UV + TIME*.8).xy));
	
	vec3 normal = normalize(mix(normal1,normal2,0.5));
	float spec = smoothstep(0.4,0.92, dot(normal, lightDir));
	float fresnel = get_fresnel(2, normal, vec3(0,0,-1));
	float m = abs(uv.y + n*0.015);
	float f =1. - smoothstep(.4,.42, m);
	float f2 = 1.-smoothstep(0.35,0.37,m);
	
	float c = (f - f2)*n;
	
	float fbm_noise = fbmNoise(world_pos*0.01,TIME);
	c = fbm_noise;
	COLOR = vec4(c,c,c,1.);
	// Called for every pixel the material is visible on.
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
