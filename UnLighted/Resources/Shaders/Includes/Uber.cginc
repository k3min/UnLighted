#ifndef UBER_INCLUDED
#define UBER_INCLUDED

v2f_light vert_light(appdata_full v)
{
	v2f_light o;

	o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
	o.uv = v.texcoord.xy;

	TANGENT_SPACE_ROTATION;

	o.TtoW0 = mul(rotation, _Object2World[0].xyz);
	o.TtoW1 = mul(rotation, _Object2World[1].xyz);
	o.TtoW2 = mul(rotation, _Object2World[2].xyz);

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

	float3 viewDir = ObjSpaceViewDir(v.vertex);
	float3 worldRefl = mul((float3x3)_Object2World, -viewDir);

	TANGENT_SPACE_ROTATION;

	o.TtoW0 = float4(mul(rotation, _Object2World[0].xyz), worldRefl.x);
	o.TtoW1 = float4(mul(rotation, _Object2World[1].xyz), worldRefl.y);
	o.TtoW2 = float4(mul(rotation, _Object2World[2].xyz), worldRefl.z);

	o.worldPos = mul(_Object2World, v.vertex).xyz;
	o.viewDir = mul(rotation, viewDir);

#ifndef UNITY_PASS_FORWARDBASE
#ifndef LIGHTMAP_OFF
	o.uv.zw = v.texcoord1.xy * unity_LightmapST.xy;
	o.uv.zw += unity_LightmapST.zw;

	float4 fade = unity_ShadowFadeCenterAndType;

	o.multi.xyz = mul(_Object2World, v.vertex).xyz - fade.xyz;
	o.multi.xyz *= fade.w;

	o.multi.w = -mul(UNITY_MATRIX_MV, v.vertex).z;
	o.multi.w *= 1.0 - fade.w;
#else
	float3 worldNorm = mul((float3x3)_Object2World, v.normal);

	o.multi.rgb = ShadeSH9(float4(worldNorm, 1.0));
#endif
#else
	o.multi.xyz = mul(rotation, ObjSpaceLightDir(v.vertex));
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

float3 BPCEM(v2f_uber i, float3 n)
{
	float3 a = float3(i.TtoW0.w, i.TtoW1.w, i.TtoW2.w);
	float3 b = float3(dot(i.TtoW0.xyz, n), dot(i.TtoW1.xyz, n), dot(i.TtoW2.xyz, n));

	float3 r = reflect(a, b);
	float3 start = _BoxPos - (_BoxSize * 0.5);

	float3 A = (start + _BoxSize - i.worldPos) / r;
	float3 B = (start - i.worldPos) / r;
	float3 plane = (r > 0) ? A : B;

	return i.worldPos + (r * min(min(plane.x, plane.y), plane.z)) - _BoxPos;
}

float4 Lighting(v2f_uber i)
{
	float4 light = tex2Dproj(_LightBuffer, UNITY_PROJ_COORD(i.screen));

#ifndef LIGHTMAP_OFF
	float3 lightMap = DecodeLightmap(tex2D(unity_Lightmap, i.uv.zw));
	float3 lightMapInd = DecodeLightmap(tex2D(unity_LightmapInd, i.uv.zw));
	float2 lightMapFade = unity_LightmapFade.zw;

	float fade = (length(i.multi) * lightMapFade.x) + lightMapFade.y;

	light.rgb += lerp(lightMapInd, lightMap, saturate(fade));
#else
	light.rgb += i.multi.rgb;
#endif

	return light;
}

float D(float a, float NdH)
{
	float a2 = a*a;
	float NdH2 = NdH * NdH;

	float denominator = (NdH2 * (a2 - 1.0)) + 1.0;

	denominator *= denominator;
	denominator *= PI;

	return a2 / denominator;
}

float G(float a, float NdV, float NdL)
{
	float k = a * 0.5;
	float GV = NdV / ((NdV * (1.0 - k)) + k);
	float GL = NdL / ((NdL * (1.0 - k)) + k);

	return GV * GL;
}

float F(float spec, float a, float VdH)
{
	return spec + ((1.0 - spec) * pow(1.0 - saturate(VdH), _Fresnel));
	//return spec + (max(1.0 - a, spec) - spec) * pow(1.0 - saturate(VdH), _Fresnel);
}

float3 F3(float3 spec, float a, float VdH)
{
	//return spec + (1.0 - spec) * pow(1.0 - saturate(VdH), _Fresnel);
	return spec + ((max(spec, 1.0 - a) - spec) * pow(1.0 - saturate(VdH), _Fresnel));
}

float Specular(float spec, float a, float NdL, float NdV, float NdH, float VdH)
{
	return (D(a, NdH) * G(a, NdV, NdL) * F(spec, a, VdH)) / ((4.0 * NdL * NdV) + EPSILON);
}

float4 CalculateLight(float3 normal, float spec, float a, float3 lightColor, float3 lightDir, float3 viewDir)
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

float SampleShadowCube(float3 vec)
{
	float fade = length(vec) * _LightPositionRange.w * (1.0 - _Shadows.z);

#ifdef SHADOWS_SOFT
	float4 shadow = 0;

	for (int i = 0; i < 4; i++)
	{
		float4 sample;

		float idx = i;
		float2 z = float2(1.0, -1.0) * (idx + 1.0) * _Shadows.y;

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

#endif