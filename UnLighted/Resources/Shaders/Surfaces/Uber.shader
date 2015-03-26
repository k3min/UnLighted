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
		_Params("Metallic, Roughness, Bumpiness, Cut", Vector) = (1, 1, 1, 0)
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
				float4 p = saturate(_Params);

			#ifdef CUT_ON
				clip(tex2D(_Cut, i.uv).r - p.w);
			#endif

			#ifdef HEIGHT_ON
				i.uv += ParallaxOffset(tex2D(_Height, i.uv), 0.05, i.viewDir);
			#endif

				Surface s;

				s.Albedo = _Color;
				s.Metallic = p.x;
				s.Roughness = p.y;

			#ifdef ALBEDO_ON
				s.Albedo *= tex2D(_Albedo, i.uv).rgb;
			#endif

				if (i.color.r >  EPSILON &&
					i.color.g <= EPSILON &&
					i.color.b <= EPSILON)
				{
					s.Albedo *= _Alt;
				}

				s.Normal = lerp(FWD, UnpackNormal(tex2D(_Normal, i.uv)), p.z);

			#ifdef METALLIC_ON
				s.Metallic *= tex2D(_Metallic, i.uv).r;
			#endif

			#ifdef ROUGHNESS_ON
				s.Roughness *= tex2D(_Roughness, i.uv).r;
			#endif

				return PrePassBase(i, s);
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
				float4 p = saturate(_Params);

			#ifdef CUT_ON
				clip(tex2D(_Cut, i.uv.xy).r - p.w);
			#endif

			#ifdef HEIGHT_ON
				i.uv.xy += ParallaxOffset(tex2D(_Height, i.uv.xy).r, 0.05, i.viewDir);
			#endif

				Surface s;

				s.Albedo = _Color;
				s.Metallic = p.x;
				s.Roughness = p.y;

			#ifdef ALBEDO_ON
				s.Albedo *= tex2D(_Albedo, i.uv.xy).rgb;
			#endif

				if (i.color.r >  EPSILON &&
					i.color.g <= EPSILON &&
					i.color.b <= EPSILON)
				{
					s.Albedo *= _Alt;
				}

			#ifdef REFLECTIONS_ON
				s.Normal = lerp(FWD, UnpackNormal(tex2D(_Normal, i.uv.xy)), p.z);

			#ifdef ROUGHNESS_ON
				s.Roughness *= tex2D(_Roughness, i.uv.xy).r;
			#endif
			#endif

			#ifdef METALLIC_ON
				s.Metallic *= tex2D(_Metallic, i.uv.xy).r;
			#endif

			#ifdef AO_ON
				s.AO = (tex2D(_AO, i.uv.xy).r * p.z) + (1.0 - p.z);
			#else
				s.AO = 1.0;
			#endif

				return PrePassFinal(i, s);
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