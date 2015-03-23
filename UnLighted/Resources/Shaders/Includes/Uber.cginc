#ifndef UBER_INCLUDED
#define UBER_INCLUDED

v2f_light vert_light(appdata_full v)
{
	v2f_light o;

	o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
	o.uv = v.texcoord.xy;

	TANGENT_SPACE_ROTATION;

	o.twX = mul(rotation, _Object2World[0].xyz);
	o.twY = mul(rotation, _Object2World[1].xyz);
	o.twZ = mul(rotation, _Object2World[2].xyz);

	o.viewDir = mul(rotation, ObjSpaceViewDir(v.vertex));

	return o;
}

v2f_uber vert_uber(appdata_full v)
{
	v2f_uber o;

	o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
	o.color = v.color;
	o.uv.xy = v.texcoord.xy;
	o.screen = ComputeScreenPos(o.pos);
	o.worldPos = mul(_Object2World, v.vertex).xyz;

	float3 viewDir = ObjSpaceViewDir(v.vertex);
	float3 worldRefl = mul((float3x3)_Object2World, -viewDir);

	TANGENT_SPACE_ROTATION;

	o.twX = float4(mul(rotation, _Object2World[0].xyz), worldRefl.x);
	o.twY = float4(mul(rotation, _Object2World[1].xyz), worldRefl.y);
	o.twZ = float4(mul(rotation, _Object2World[2].xyz), worldRefl.z);

	o.viewDir = mul(rotation, viewDir);

#ifdef LIGHTMAP_ON
	o.uv.zw = v.texcoord1.xy * unity_LightmapST.xy;
	o.uv.zw += unity_LightmapST.zw;

	float4 fade = unity_ShadowFadeCenterAndType;

	o.multi.xyz = o.worldPos - fade.xyz;
	o.multi.xyz *= fade.w;

	o.multi.w = -mul(UNITY_MATRIX_MV, v.vertex).z;
	o.multi.w *= 1.0 - fade.w;
#else
	float3 worldNorm = mul((float3x3)_Object2World, v.normal);

	o.multi.rgb = ShadeSH9(float4(worldNorm, 1.0));
#endif

	return o;
}

v2f_shadow vert_shadow(appdata_full v)
{
	v2f_shadow o;

	o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
	o.uv = v.texcoord.xy;
	o.vec = mul(_Object2World, v.vertex).xyz - _LightPositionRange.xyz;

	return o;
}

inline float3 T2W(v2f_light i, float3 n)
{
	return float3(dot(i.twX, n), dot(i.twY, n), dot(i.twZ, n));
}

inline float3 T2W(v2f_uber i, float3 n)
{
	return float3(dot(i.twX.xyz, n), dot(i.twY.xyz, n), dot(i.twZ.xyz, n));
}

inline float3 BPCEM(v2f_uber i, float3 n)
{
	float3 r = reflect(float3(i.twX.w, i.twY.w, i.twZ.w), T2W(i, n));
	float3 s = _BoxPos - (_BoxSize * 0.5);

	float3 a = (s + _BoxSize - i.worldPos) / r;
	float3 b = (s - i.worldPos) / r;

	float3 p = (r > 0) ? a : b;

	return i.worldPos + (r * min(min(p.x, p.y), p.z)) - _BoxPos;
}

inline float D(float a, float NdH)
{
	float a2 = a * a;
	float NdH2 = NdH * NdH;

	float denominator = (NdH2 * (a2 - 1.0)) + 1.0;

	denominator *= denominator;
	denominator *= PI;

	return a2 / denominator;
}

inline float G(float a, float NdV, float NdL)
{
	float k = a * 0.5;
	float GV = NdV / ((NdV * (1.0 - k)) + k);
	float GL = NdL / ((NdL * (1.0 - k)) + k);

	return GV * GL;
}

inline float F(float spec, float a, float VdH)
{
	return spec + ((1.0 - spec) * pow(1.0 - saturate(VdH), _Fresnel));
}

inline float3 F3(float3 spec, float a, float VdH)
{
	return spec + ((max(spec, 1.0 - a) - spec) * pow(1.0 - saturate(VdH), _Fresnel));
}

inline float Specular(float spec, float a, float NdL, float NdV, float NdH, float VdH)
{
	return (D(a, NdH) * G(a, NdV, NdL) * F(spec, a, VdH)) / max(4.0 * NdL * NdV, EPSILON);
}

inline float4 CalculateLight(float3 normal, float spec, float a, float3 lightColor, float3 lightDir, float3 viewDir)
{
	float3 h = normalize(viewDir + lightDir);
	float NdL = saturate(dot(normal, lightDir));
	float NdV = abs(dot(normal, viewDir));
	float NdH = saturate(dot(normal, h));
	float VdH = saturate(dot(viewDir, h));

	float4 res;

	res.rgb = lightColor * NdL;
	res.a = Specular(spec, a, NdL, NdV, NdH, VdH);

	return res;
}

inline float SampleShadowCube(float3 vec)
{
	float fade = length(vec) * _LightPositionRange.w * (1.0 - _Shadows.z);

#ifdef SHADOWS_SOFT
	float4 shadow = 0;

	float idx;
	float2 z;
	float4 sample;

	for (int i = 0; i < 4; i++)
	{
		idx = i;

		z = (idx + 1.0) * float2(1.0, -1.0) * _Shadows.y;

		if ((i % 2) == 1)
		{
			z = -z;
		}

		sample.x = DecodeFloatRGBA(texCUBE(_ShadowMapTexture, vec + z.xxx));
		sample.y = DecodeFloatRGBA(texCUBE(_ShadowMapTexture, vec + z.yyx));
		sample.z = DecodeFloatRGBA(texCUBE(_ShadowMapTexture, vec + z.yxy));
		sample.w = DecodeFloatRGBA(texCUBE(_ShadowMapTexture, vec + z.xyy));

		shadow += (sample < (fade - (idx * _Shadows.w))) ? _LightShadowData.r : 1.0;
	}

	return dot(shadow, 1.0 / (4.0 * 4.0));
#else
	return DecodeFloatRGBA(texCUBE(_ShadowMapTexture, vec)) < fade ? _LightShadowData.r : 1.0;
#endif
}

inline float4 PrePassBase(v2f_light i, Surface s)
{
	float4 res;

	res.rg = EncodeNormal(T2W(i, s.Normal));
	res.b = max(s.Roughness, EPSILON);
	res.a = lerp(0.03, Luminance(s.Albedo), s.Metallic);

	return res;
}

inline float3 PrePassFinal(v2f_uber i, Surface s)
{
	float4 light = tex2Dproj(_LightBuffer, UNITY_PROJ_COORD(i.screen));

#ifdef LIGHTMAP_ON
	float fade = LightmapFade(length(i.multi));

	if (fade > EPSILON)
	{
		light.rgb += DecodeLightmap(tex2D(unity_Lightmap, i.uv.zw)) * fade;
	}

	if (fade < (1.0 - EPSILON))
	{
		light.rgb += DecodeLightmap(tex2D(unity_LightmapInd, i.uv.zw)) * (1.0 - fade);
	}
#else
	light.rgb += i.multi.rgb;
#endif

	float3 res = s.Albedo * (1.0 - s.Metallic);

#ifdef REFLECTIONS_ON
#ifdef LIGHTMAP_ON
	if (fade < 1.0) {
#endif

	float a = max(s.Roughness, EPSILON);
	float4 rlf = float4(BPCEM(i, s.Normal), a * 8.0);
	float3 env = HDRDecode(texCUBElod(_Box, rlf));
	float3 spec = lerp(0.03.xxx, s.Albedo, s.Metallic);

	env *= F3(spec, a, dot(normalize(i.viewDir), s.Normal));
	env *= s.AO;

#ifdef LIGHTMAP_ON
	env *= 1.0 - fade;
#endif

	res += env;

#ifdef LIGHTMAP_ON
	}
#endif
#endif

	res *= light.rgb;
	res += light.a;

	return res;
}

#endif