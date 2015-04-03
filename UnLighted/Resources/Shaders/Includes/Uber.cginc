#ifndef UBER_INCLUDED
#define UBER_INCLUDED

v2f_light vert_light(appdata_full v)
{
	v2f_light o;

	o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
	o.color = v.color;
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

	float3 viewDir = ObjSpaceViewDir(v.vertex);
	float4 worldPos = mul(_Object2World, v.vertex);
	float3 worldRefl = mul((float3x3)_Object2World, -viewDir);

	o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
	o.color = v.color;
	o.uv.xy = v.texcoord.xy;
	o.screen = ComputeScreenPos(o.pos);
	o.worldPos = worldPos.xyz;

	TANGENT_SPACE_ROTATION;

	o.twX = float4(mul(rotation, _Object2World[0].xyz), worldRefl.x);
	o.twY = float4(mul(rotation, _Object2World[1].xyz), worldRefl.y);
	o.twZ = float4(mul(rotation, _Object2World[2].xyz), worldRefl.z);

	o.viewDir = mul(rotation, viewDir);

#ifdef LIGHTMAP_ON
	o.uv.zw = v.texcoord1.xy * unity_LightmapST.xy;
	o.uv.zw += unity_LightmapST.zw;

	float4 fade = unity_ShadowFadeCenterAndType;

	o.multi.xyz = worldPos.xyz - fade.xyz;
	o.multi.xyz *= fade.w;

	o.multi.w = -mul(UNITY_MATRIX_MV, v.vertex).z;
	o.multi.w *= 1.0 - fade.w;
#else
	float3 n = mul((float3x3)_Object2World, v.normal);
	o.multi.rgb = ShadeSH9(float4(n, 1.0));
#endif

	float3 light = mul(_LightMatrix0, worldPos).xyz;

	o.lightDir.xyz = mul(rotation, ObjSpaceLightDir(v.vertex)).xyz;
	o.lightDir.w = dot(light, light);

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

// http://gamedev.net/topic/568829-box-projected-cubemap-environment-mapping/?view=findpost&p=4637262
inline float3 BPCEM(v2f_uber i, float3 n)
{
	float3 r = reflect(float3(i.twX.w, i.twY.w, i.twZ.w), T2W(i, n));
	float3 s = _BoxPos - (_BoxSize * 0.5);

	float3 a = (s + _BoxSize - i.worldPos) / r;
	float3 b = (s - i.worldPos) / r;

	float3 p = (r > 0) ? a : b;

	return i.worldPos + (r * min(min(p.x, p.y), p.z)) - _BoxPos;
}

inline float3 Refl(v2f_uber i, float3 normal, float3 spec, float a)
{
	float VdN = saturate(dot(normal, normalize(i.viewDir)));

	a = max(a, EPSILON);

	float4 rlf = float4(BPCEM(i, normal), a * 8.0);
	float3 env = HDRDecode(texCUBElod(_Box, rlf));

	return env * (spec + ((max(spec, 1.0 - a) - spec) * pow(1.0 - VdN, _Fresnel)));
}

inline float4 PrePassBase(v2f_light i, Surface s)
{
	float4 res;

	res.rg = EncodeNormal(T2W(i, s.Normal));
	res.b = max(s.Roughness, EPSILON);
	res.a = lerp(0.03, Luminance(s.Albedo), s.Metallic);

	return res;
}

inline float4 PrePassFinal(v2f_uber i, Surface s)
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

	float3 spec = lerp(0.03.xxx, s.Albedo, s.Metallic);
	float3 env = Refl(i, s.Normal, spec, s.Roughness) * s.AO;

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

	res += s.Emission;

	return float4(res, 1.0);
}

// http://slideshare.net/slideshow/embed_code/7170855
inline float4 Translusency(v2f_uber i, Surface s)
{
	float a = max(s.Roughness, EPSILON);

#ifdef REFLECTIONS_ON
	s.Albedo += Refl(i, s.Normal, 0.03, a);
#endif

	float3 lightDir = normalize(i.lightDir.xyz);
	float3 viewDir = normalize(i.viewDir);

	float4 light = BRDF(s.Normal, 0.03, a, lightDir, viewDir);
	float back = pow(saturate(dot(viewDir, -lightDir)), 1.0 / s.Translucency);

	float scale = (1.0 / _LightPositionRange.w) * 0.5;
	float thickness = tex2Dproj(_Thickness, UNITY_PROJ_COORD(i.screen));

	light.rgb += back * scale * pow(thickness, s.Thickness);
	light.rgb *= _LightColor0;
	light.a *= Luminance(_LightColor0);
	light *= tex2D(_LightTexture0, i.lightDir.ww).UNITY_ATTEN_CHANNEL * 2.0;

	float3 res = s.Albedo;

	res *= light.rgb;
	res += light.a;

#ifdef LIGHTMAP_OFF
	res += s.Albedo * i.multi.rgb;
#endif

	return float4(res, 1.0);
}

#endif