#ifndef VARS_INCLUDED
#define VARS_INCLUDED

static const float PI = 3.14159265358979323846;
static const float TWO_PI = PI * 2.0;
static const float HALF_PI = PI * 0.5;
static const float EPSILON = 0.001;
static const float3 FWD = float3(0, 0, 1);
static const float L = 1.0 / length(0.5.xx);

sampler2D _MainTex;
float4 _MainTex_TexelSize;
float4 _ProjInfo;

sampler2D _CameraNormalsTexture;
sampler2D _CameraDepthTexture;
sampler2D _CameraDepthNormalsTexture;

sampler2D _LightBuffer;
float4x4 _LightMatrix0;
sampler2D _LightTexture0;
float3 _LightColor0;

float4 unity_LightmapST;
sampler2D unity_Lightmap;
sampler2D unity_LightmapInd;
float4 unity_LightmapFade;

float _Fresnel;
float4 _Shadows;
float4 _Params;

samplerCUBE _Box;
float3 _BoxPos;
float3 _BoxSize;
sampler2D _Thickness;

struct Surface {
	float3 Albedo;
	float3 Normal;
	float Metallic;
	float Roughness;
	float AO;
	float Thickness;
	float Translucency;
	float3 Emission;
};

struct v2f_light {
	float4 pos : POSITION;
	float4 color : COLOR;
	float2 uv : TEXCOORD0;
	float3 twX : TEXCOORD1;
	float3 twY : TEXCOORD2;
	float3 twZ : TEXCOORD3;
	float3 viewDir : TEXCOORD4;
};

struct v2f_uber {
	float4 pos : POSITION;
	float4 color : COLOR;
	float4 uv : TEXCOORD0;
	float4 screen : TEXCOORD1;
	float4 twX : TEXCOORD2;
	float4 twY : TEXCOORD3;
	float4 twZ : TEXCOORD4;
	float3 viewDir : TEXCOORD5;
	float3 worldPos : TEXCOORD6;
	float4 multi : TEXCOORD7;
	float4 lightDir : TEXCOORD8;
};

struct v2f_shadow {
	float4 pos : POSITION;
	float2 uv : TEXCOORD0;
	float3 vec : TEXCOORD1;
};

#endif