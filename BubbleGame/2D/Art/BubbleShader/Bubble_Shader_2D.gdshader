shader_type canvas_item;
uniform sampler2D normal_map:hint_normal, repeat_enable;
uniform sampler2D colorRamp;
uniform sampler2D noise_tex:repeat_enable;
uniform sampler2D bubbles_tex:repeat_enable;
uniform vec3 albedo : source_color = vec3(0.02, 0.45, 0.8);
uniform float vertexDistortion = 0.1;
uniform float distortion = 0.1;
uniform vec3 spec_color:source_color = vec3(0.6,.6,0.8);
uniform vec3 lightDir = vec3(1,-1,1);
uniform float noiseColorStrength = .5;
uniform vec3 skyColor:source_color = vec3(0,0,0);
uniform sampler2D bubble_dir_tex:repeat_enable;
uniform vec3 bubblesColor:source_color;

uniform sampler2D SCREEN_TEXTURE:hint_screen_texture;

uniform float opacity;

varying vec2 world_pos;


float random (in vec2 _st) {
    return fract(sin(dot(_st.xy,
                         vec2(12.9898,78.233)))*
        43758.5453123);
}

vec2 bulgeUv(vec2 uv, float bias, float scale)
{
	uv = uv*scale + bias;
	vec2 baseUv = uv;

	uv *= 0.5;

	uv = (1. - max(dot(uv,uv),0.)) * uv;

	return baseUv - uv;
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
	vec2 raw_uv = UV;
	vec2 uv = UV - 0.5;
	//uv =fract( bulgeUv(uv,0,0.1));

	float n = texture(noise_tex,world_pos*0.002 + TIME*0.1).x *2. - 1.;

	uv.y += n*0.1;
	vec3 normal1 =normalize(DecodeNormals( texture(normal_map,world_pos*0.01 + TIME*0.1).xy)*.01);
	vec3 normal2 = normalize(DecodeNormals(texture(normal_map,world_pos*0.001 + TIME*.1).xy));

	vec3 normal = normalize(mix(normal1,normal2,0.5));
	float m = abs(uv.y);
	float f =1. - smoothstep(.4,.42, m);
	float f2 = 1.-smoothstep(0.35,0.37,m);

	float edge = f - f2;
	float c = (f - f2)*n;

	float fbm_noise = fbmNoise(world_pos*0.01 + vec2(uv.x*0.1,pow(abs(uv.y),1./15.)),TIME);
	c = fbm_noise;
	vec3 fbmColor = texture(colorRamp,vec2(fbm_noise,0.5)).xyz;

	vec2 screen_uv = SCREEN_PIXEL_SIZE/ TEXTURE_PIXEL_SIZE;

	vec3 screenColor = textureLod(SCREEN_TEXTURE, SCREEN_UV + normal.xy*0.03, 0.0).xyz;
	vec3 reflectedColor = textureLod(SCREEN_TEXTURE,SCREEN_UV - (uv + normal.xy*5.)*0.0,0.).xyz;

	vec3 color = mix(albedo ,fbmColor, noiseColorStrength);

	float topMask = max(dot(uv,vec2(0.,1.)),0.) * fbm_noise;
	float bottomMask = 1. - topMask;
	topMask = smoothstep(0.,0.2,topMask);
	float thickness = smoothstep(0.1,.3,abs(uv.y));
	color = mix(screenColor,color,thickness+.4) + reflectedColor*0.1;
	float edgeNoise =  texture(noise_tex,world_pos*0.006 + TIME*0.08).x *  texture(noise_tex,world_pos*0.001 + TIME*0.08).x;
	vec2 bubble_dir = texture(bubble_dir_tex, world_pos*0.01 ).xy;
	color = mix(color,skyColor,topMask) +  smoothstep(0.4,0.45, (edge * edgeNoise)) + texture(bubbles_tex,world_pos*0.01 + bubble_dir + TIME*0.2).xyz*bubblesColor;

	COLOR = vec4(color,f);
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
