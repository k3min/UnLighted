Shader "UnLighted/Über"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1)
		_Albedo("Albedo", 2D) = "white" {}
		_Alt("Alternative", Color) = (1, 0.75, 0)
		_Normal("Normal", 2D) = "bump" {}
		_Metallic("Metallic", 2D) = "white" {}
		_Roughness("Roughness", 2D) = "white" {}
		_AO("AO", 2D) = "white" {}
		_Height("Height", 2D) = "white" {}
		_Cut("Cut", 2D) = "white" {}
		_Params("Metallic, Roughness, Bumpiness, Cut", Vector) = (1, 1, 1, 1)
		[HideInInspector]
		_Hack("", 2D) = "bump" {}
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" }

		CGINCLUDE

		#include "UnityCG.cginc"

		#include "./../Includes/Vars.cginc"
		#include "./../Includes/Util.cginc"
		#include "./../Includes/Uber.cginc"

		#pragma glsl
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma only_renderers d3d11 opengl

		#pragma multi_compile ALBEDO_OFF ALBEDO_ON
		#pragma multi_compile METALLIC_OFF METALLIC_ON
		#pragma multi_compile ROUGHNESS_OFF ROUGHNESS_ON
		#pragma multi_compile HEIGHT_OFF HEIGHT_ON
		#pragma multi_compile CUT_OFF CUT_ON

		float3 _Color;
		float3 _Alt;
		sampler2D _Albedo;
		sampler2D _Normal;
		sampler2D _Metallic;
		sampler2D _Roughness;
		sampler2D _AO;
		sampler2D _Height;
		sampler2D _Hack;
		sampler2D _Cut;

		ENDCG

		Pass
		{
			Tags { "LightMode" = "ForwardBase" }

			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag

			float4 frag(v2f_img i) : COLOR
			{
				return float4(tex2D(_Albedo, i.uv).rgb * _Color, 1.0);
			}

			ENDCG
		}

		Pass
		{
			Tags { "LightMode" = "PrePassBase" }

			CGPROGRAM

			#pragma vertex vert_light
			#pragma fragment frag

			float4 frag(v2f_light i) : COLOR
			{
				float4 params = saturate(_Params);

			#ifdef CUT_ON
				clip(tex2D(_Cut, i.uv).r - params.w);
			#endif

			#ifdef HEIGHT_ON
				i.uv += ParallaxOffset(tex2D(_Height, i.uv), 0.05, i.viewDir);
			#endif

				float4 n0 = tex2D(_Hack, i.uv);
				float4 n1 = tex2D(_Normal, i.uv);

				float3 normal = UnpackNormal(lerp(n0, n1, params.z));

				float3 worldNorm;

				worldNorm.x = dot(i.TtoW0, normal);
				worldNorm.y = dot(i.TtoW1, normal);
				worldNorm.z = dot(i.TtoW2, normal);

			#ifdef ALBEDO_ON
				float3 albedo = tex2D(_Albedo, i.uv).rgb * _Color;
			#else
				float3 albedo = _Color;
			#endif

			#ifdef METALLIC_ON
				float metallic = tex2D(_Metallic, i.uv).r * params.x;
			#else
				float metallic = params.x;
			#endif

			#ifdef ROUGHNESS_ON
				float a = tex2D(_Roughness, i.uv).r * params.y;
			#else
				float a = params.y;
			#endif

				float4 res;

				res.rg = EncodeNormal(worldNorm);
				res.b = max(a, EPSILON);
				res.a = lerp(0.03, Luminance(albedo), metallic);

				return res;
			}

			ENDCG
		}

		Pass
		{
			Tags { "LightMode" = "PrePassFinal" }

			CGPROGRAM

			#pragma vertex vert_uber
			#pragma fragment frag

			#pragma multi_compile_prepassfinal
			#pragma multi_compile AO_OFF AO_ON
			#pragma multi_compile REFLECTIONS_OFF REFLECTIONS_ON

			float4 frag(v2f_uber i) : COLOR
			{
				float4 params = saturate(_Params);

			#ifdef CUT_ON
				clip(tex2D(_Cut, i.uv.xy).r - params.w);
			#endif

				float fade = Fade(i);
				float4 light = Lighting(i, fade);

			#ifdef HEIGHT_ON
				i.uv.xy += ParallaxOffset(tex2D(_Height, i.uv.xy).x, 0.05, i.viewDir);
			#endif

			#ifdef ALBEDO_ON
				float3 albedo = tex2D(_Albedo, i.uv.xy).rgb * _Color;
			#else
				float3 albedo = _Color;
			#endif

				if (i.color.r > 0 && i.color.g == 0 && i.color.b == 0)
				{
					albedo *= _Alt;
				}

			#ifdef METALLIC_ON
				float metallic = tex2D(_Metallic, i.uv.xy).r * params.x;
			#else
				float metallic = params.x;
			#endif

				float3 res = albedo * (1.0 - metallic);

			#ifdef REFLECTIONS_ON
			#ifdef LIGHTMAP_ON
				if (fade < 1.0) {
			#endif

				float4 n0 = tex2D(_Hack, i.uv.xy);
				float4 n1 = tex2D(_Normal, i.uv.xy);

				float3 normal = UnpackNormal(lerp(n0, n1, params.z));

			#ifdef ROUGHNESS_ON
				float a = tex2D(_Roughness, i.uv.xy).r * params.y;
			#else
				float a = params.y;
			#endif

				a = max(a, EPSILON);

				float4 cube = float4(BPCEM(i, normal), a * 8.0);
				float3 env = HDRDecode(texCUBElod(_Box, cube));
				float3 spec = lerp(0.03.xxx, albedo, metallic);

				env *= F3(spec, a, dot(normalize(i.viewDir), normal));

			#ifdef AO_ON
				env *= (tex2D(_AO, i.uv.xy).r * params.z) + (1.0 - params.z);
			#endif

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

				return float4(res, 1.0);
			}

			ENDCG
		}

		Pass
		{
			Name "SHADOWCASTER"

			Tags { "LightMode" = "ShadowCaster" }

			Cull Off

			CGPROGRAM

			#pragma vertex vert_shadow
			#pragma fragment frag

			float4 frag(v2f_shadow i) : COLOR
			{
			#ifdef CUT_ON
				clip(tex2D(_Cut, i.uv).r - saturate(_Params.w));
			#endif

				return EncodeFloatRGBA(min(length(i.vec) * _LightPositionRange.w, 1.0 - EPSILON));
			}

			ENDCG
		}
	}

	CustomEditor "UberMaterialEditor"
}