shader_type spatial;

render_mode cull_disabled, depth_draw_always, unshaded;

const float EPSILON = 1e-5;

uniform sampler2D SCREEN_TEXTURE : hint_screen_texture, filter_nearest;
uniform sampler2D DEPTH_TEXTURE : hint_depth_texture, filter_nearest;

uniform sampler2D colorRamp;
uniform vec3 albedo : source_color = vec3(0.02, 0.45, 0.8);
uniform float distortion = 0.1;
uniform vec3 spec_color:source_color = vec3(0.6,.6,0.8);
uniform float transparency : hint_range(0.0, 1.0, 0.01) = 0.6;
uniform float metallic : hint_range(0.0, 1.0, 0.01) = 0.45;
uniform float roughness : hint_range(0.0, 1.0, 0.01) = 0.05;

uniform float max_visible_depth : hint_range(0.1, 100.0, 0.1) = 10.0;
uniform vec3 color_deep : source_color = vec3(0.0, 0.1, 0.4);

uniform sampler2D wave_a;
uniform sampler2D wave_b;

uniform vec2 wave_move_direction_a = vec2(-1.0, 0.0);
uniform vec2 wave_move_direction_b = vec2(0.0, 1.0);
uniform float wave_noise_scale_a = 15.0;
uniform float wave_noise_scale_b = 15.0;
uniform float wave_time_scale_a = 0.025;
uniform float wave_time_scale_b = 0.025;
uniform float wave_height_scale = 0.4;
uniform float wave_normal_flatness : hint_range(0.1, 100.0, 0.1) = 16.0;

uniform sampler2D normal_map: hint_normal;
uniform sampler2D surface_normals_a;
uniform sampler2D surface_normals_b;
uniform vec2 surface_normals_move_direction_a = vec2(-1.0, 0.0);
uniform vec2 surface_normals_move_direction_b = vec2(0.0, 1.0);
uniform float surface_texture_roughness : hint_range(0.0, 1.0, 0.01) = 0.6;
uniform float surface_texture_scale : hint_range(0.001, 2.0, 0.001) = 0.3;
uniform float surface_texture_time_scale : hint_range(0.001, 2.0, 0.001) = 0.06;


uniform float ssr_resolution   : hint_range(0.01, 10.0, 0.1)	= 2.0;
uniform float ssr_max_travel   : hint_range(0.0, 200.0, 0.1) 	= 30.0;
uniform float ssr_max_diff     : hint_range(0.1, 10.0, 0.1) 	= 4.0;
uniform float ssr_mix_strength : hint_range(0.0, 1.0, 0.01) 	= 0.7;
uniform float ssr_screen_border_fadeout: hint_range(0.0, 1.0, 0.1)	= 0.3;
uniform int ssr_samples   : hint_range(1, 5, 1)	= 2;
uniform float ssr_sample_radius: hint_range(0.0, 1.0, 0.1) = 0.2;

uniform float refraction_intensity : hint_range(0.0, 1.0, 0.01) = 0.4; 

uniform vec3  border_color : source_color = vec3(1.0);
uniform float border_scale : hint_range(0.0, 5.0, 0.01) = 2.0;
uniform float border_near = 0.5;
uniform float border_far = 300.0;
uniform vec3 lightDir = vec3(-1,-1,-1);

uniform float cut_out_x = 0.0;
uniform float cut_out_z = 0.0;

varying vec2 vertex_uv; 
varying vec3 local_position;



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



float get_wave_height(vec2 uv_a, vec2 uv_b)
{
	float height1 = texture(wave_a, uv_a).y;
	float height2 = texture(wave_b, uv_b).y;
	return (height1 + height2) / 2.0;
}



bool is_within_screen_boundaries(vec2 position) 
{
	return position.x > 0.0 && position.x < 1.0 && position.y > 0.0 && position.y < 1.0;
}

vec2 get_uv_from_view_position(vec3 position_view_space, mat4 proj_m)
{
	vec4 position_clip_space = proj_m * vec4(position_view_space.xyz, 1.0);
	vec2 position_ndc = position_clip_space.xy / position_clip_space.w;
	return position_ndc.xy * 0.5 + 0.5;
}

vec3 get_view_position_from_uv(vec2 uv, float depth, mat4 inv_proj_m)
{
	vec4 position_ndc = vec4((uv * 2.0) - 1.0, depth, 1.0);
	vec4 view_position = inv_proj_m * position_ndc;
	return view_position.xyz /= view_position.w;
}

bool is_zero(float value) 
{
    return abs(value) < EPSILON;
}

float get_screen_border_alpha(vec2 screen_position)
{

	if (is_zero(ssr_screen_border_fadeout)) 
	{
		return 1.0;
	}

    vec2 shifted_screen_position = 4.0 * screen_position * (1.0 - screen_position);
	float mask = shifted_screen_position.x * shifted_screen_position.y; // ranging from 0.0 at the edges to 1.0 in the center 

	// An offset in the [0.0, 0.5] range for ssr_screen_border_fadeout values > 0.75
	// which is subtracted from the result of smoothstep.
	// This ensure alpha smoothly transitions to zero when ssr_screen_border_fadeout is approaching 1.0.	
	float offset = mix(0.0, 0.5, (clamp(ssr_screen_border_fadeout, 0.75, 1.0)-0.75) / 0.25);
 
	float alpha = clamp(smoothstep(0.0, 2.0 * ssr_screen_border_fadeout, mask) - offset, 0.0, 1.0);

	return alpha;
}

vec4 get_ssr_color(vec3 surface_view_position, vec3 normal_view_space, vec3 view_view_space, mat4 proj_m, mat4 inv_proj_m)
{
	vec3 current_position_view_space = surface_view_position;
	vec3 view_direction_view_space = view_view_space * -1.0;
	vec3 reflect_vector_view_space = normalize(reflect(view_direction_view_space.xyz, normal_view_space.xyz));
	
	vec2 current_screen_position = vec2(0.0);
	
	vec3 resulting_color = vec3(-1.0);
	for(float travel=0.0; resulting_color.x < 0.0 && travel < ssr_max_travel; travel = travel + ssr_resolution)
	{
		current_position_view_space += reflect_vector_view_space * ssr_resolution;
		current_screen_position = get_uv_from_view_position(current_position_view_space, proj_m);

		float depth_texture_probe_raw =1.0 - texture(DEPTH_TEXTURE, current_screen_position).x;
		vec3 depth_texture_probe_view_position = get_view_position_from_uv(current_screen_position, depth_texture_probe_raw, inv_proj_m);
		
		float depth_diff = depth_texture_probe_view_position.z - current_position_view_space.z;
		
		resulting_color = (is_within_screen_boundaries(current_screen_position) && depth_diff >= 0.0 && depth_diff < ssr_max_diff) ? texture(SCREEN_TEXTURE, current_screen_position.xy).rgb : vec3(-1.0);
	}

	float alpha = get_screen_border_alpha(current_screen_position);
	return vec4(resulting_color,alpha);
}

float linear_depth(float cur_depth)
{
    return border_far * border_near / (border_near + cur_depth * (border_far - border_near));
}

float normalize_float(float min_v, float max_v, float value) 
{
	float clamped_value = clamp(value, min_v, max_v);
	return  (clamped_value - min_v) / (max_v - min_v);
}

vec2 get_refracted_uv(vec2 raw_screen_uv, float screen_depth_raw, vec3 view, vec3 normal, mat4 proj_m, mat4 inv_proj_m)
{
		vec3 screen_view_position_original = get_view_position_from_uv(raw_screen_uv, screen_depth_raw, inv_proj_m);
		float screen_center_distance = clamp(abs(length(raw_screen_uv - vec2(0.5, 0.5))) * 2.0, 0.0, 1.0);
		float refraction_intensity_deglitched = mix(1.0 - refraction_intensity, 1.0, screen_center_distance);
		vec3 refraction_position_view_space = screen_view_position_original + normalize(refract(view, -normal, refraction_intensity_deglitched));
		vec2 refraction_uv = get_uv_from_view_position(refraction_position_view_space, proj_m);
		return refraction_uv;
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

void vertex()
{
	local_position = VERTEX;
	
	vec3 normal_tex = textureLod(normal_map,UV + TIME*.1,0).xyz;
	
	normal_tex = DecodeNormals(normal_tex.xy);
	
	vec3 n = normal_tex.x *TANGENT + normal_tex.y * BINORMAL + normal_tex.z*NORMAL;
	n = normalize(n);
	
	vertex_uv = (MODEL_MATRIX * vec4(local_position, 1.0)).xz;
	vec2 uv_a = vertex_uv / wave_noise_scale_a + (wave_move_direction_a * TIME * wave_time_scale_a);
	vec2 uv_b = vertex_uv / wave_noise_scale_b + (wave_move_direction_b * TIME * wave_time_scale_b);
	VERTEX = VERTEX + n*0.02;
	
	float normal_height_scale = wave_height_scale / wave_normal_flatness;
	vec2 e = vec2(0.01, 0.0);
	vec3 normal = normalize(vec3(
		get_wave_height(uv_a - e, uv_b - e) * normal_height_scale - get_wave_height(uv_a + e, uv_a + e) * normal_height_scale, 
		2.0 * e.x, 
		get_wave_height(uv_a - e.yx, uv_b - e.yx) * normal_height_scale - get_wave_height(uv_a + e.yx, uv_b + e.yx) * normal_height_scale
		));
}

void fragment() 
{
	vec3 normal = FRONT_FACING ? NORMAL : -NORMAL;
	vec2 normal_tex = texture(normal_map, UV + vec2(0,TIME*0.1)).rg *2. - 1.;
	vec2 normal_tex_2 =texture(normal_map, UV*1.3 + vec2(0,TIME*0.06)).rg *2. - 1.;
	
	normal_tex = mix(normal_tex,normal_tex_2,.5);
	float z = sqrt(1. - normal_tex.x*normal_tex.x - normal_tex.y * normal_tex.y);
	vec3 worldNormal = ( INV_VIEW_MATRIX* vec4(NORMAL,0)).rgb;
	
	
	normal = normal_tex.x * TANGENT + normal_tex.y * BINORMAL + z * NORMAL;
	normal = normalize(normal);
	
	vec3 viewDir =normalize( (VERTEX - CAMERA_POSITION_WORLD));
	
	float screen_depth_raw = 1.0 - texture(DEPTH_TEXTURE, SCREEN_UV).x;
	
	vec2 refraction_uv = refraction_intensity > 0.0 ? get_refracted_uv(SCREEN_UV, screen_depth_raw, VIEW, normal, PROJECTION_MATRIX, INV_PROJECTION_MATRIX) : SCREEN_UV;
	
	float screen_depth = 1.0 - texture(DEPTH_TEXTURE, refraction_uv).x;
	float surface_depth = FRAGCOORD.z;
	
	float border_diff = linear_depth(1.0 - screen_depth_raw) - linear_depth(1.0 - surface_depth);
	
	vec2 time_vector = (TIME * surface_normals_move_direction_a) * surface_texture_time_scale;
	vec2 time_vector2 = (TIME * surface_normals_move_direction_b) * surface_texture_time_scale;
	
	vec3 normal_texture_blend 	= mix(texture(surface_normals_a, vertex_uv * surface_texture_scale + time_vector).xyz, texture(surface_normals_b, vertex_uv * surface_texture_scale + time_vector2).xyz, 0.5);
	vec3 normal_blend 			= mix(normal, normal_texture_blend, surface_texture_roughness);
	
	vec3 screen_view_position 	= get_view_position_from_uv(refraction_uv, screen_depth, INV_PROJECTION_MATRIX);
	vec3 surface_view_position 	= get_view_position_from_uv(SCREEN_UV, surface_depth, INV_PROJECTION_MATRIX);
	float depth_opacity = 1.0 - normalize_float(0.0, max_visible_depth, length(surface_view_position - screen_view_position));
	
	float noise = fbmNoise(UV*2.,TIME*.5);
	float fresnel =smoothstep(.2,.3, get_fresnel(2.0, normal,VIEW));
	noise = fresnel + noise;
	vec3 ramp_color = texture(colorRamp,vec2(noise,.5)).rgb;
	
	vec3 screen_color 				= texture(SCREEN_TEXTURE, refraction_uv).rgb;
	vec3 surface_color 				= albedo;
	vec4 ssr_color 					= get_ssr_color(surface_view_position, NORMAL, VIEW, PROJECTION_MATRIX, INV_PROJECTION_MATRIX);
	vec3 color_with_transparency 	= mix(surface_color, screen_color, transparency);
	
		
	float f = get_fresnel(1.,NORMAL,VIEW);
	float upMask =smoothstep(.5,.8, max(dot(normal, vec3(0,1,0)),0));
	vec3 sky_color = vec3(0.1,0.2,1.0);
	float spec = smoothstep(.88,.9, pow( clamp( dot(normal, normalize(1. - lightDir)),0,1), 5));
	float vertexSpec =smoothstep(.95,1., 1. - abs(dot(worldNormal + normal*0.3,viewDir)));
	spec = min(spec + vertexSpec, 1);
	vec3 final_color = color_with_transparency*albedo + ramp_color*.6*noise + spec;
	
	final_color = mix(final_color, sky_color,upMask*.6);
	
	ALBEDO.rgb = final_color;
	METALLIC = metallic;
	ROUGHNESS = roughness;
	NORMAL_MAP = normal_blend;
	NORMAL = normal;
}

void light(){
	DIFFUSE_LIGHT = vec3(1,0,0);
}

