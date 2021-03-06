﻿Shader "Hidden/UnLighted-ImageEffects-MotionBlur"
{
	Properties
	{
		_MainTex("", 2D) = "" {}
	}

	SubShader
	{
		ZWrite Off

		CGINCLUDE

		#include "UnityCG.cginc"
		#include "./../Includes/Vars.cginc"

		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma only_renderers d3d11 opengl

		#define MAX_SAMPLES 30

		float4x4 _Proj;

		inline float2 EncodeMotion(float4 cur, float4 prv)
		{
			float2 a = cur.xy / cur.w;
			float2 b = prv.xy / prv.w;

			return ((a - b) * 0.5) + 0.5;
		}

		inline float2 DecodeMotion(float2 motion)
		{
			return (motion * 2.0) - 1.0;
		}

		ENDCG

		Pass
		{
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag

			float4 frag(v2f_img i) : COLOR
			{
				float d = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv);

				float4 pos = float4((i.uv * 2.0) - 1.0, d, 1.0);
				float4 prv = mul(_Proj, pos);

				return float4(EncodeMotion(pos, prv), 0.0, 0.0);
			}

			ENDCG
		}

		Pass
		{
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag

			#pragma glsl

			sampler2D _Motion;
			float _MotionScale;

			float4 frag(v2f_img i) : COLOR
			{
				float2 vel = DecodeMotion(tex2D(_Motion, i.uv).xy) * _MotionScale;
				float samples = clamp(round(length(vel * _MainTex_TexelSize.zw)), 1, MAX_SAMPLES);

				float4 res = tex2D(_MainTex, i.uv);

				float idx;
				float2 off;

				for (int j = 1; j < (int)samples; j++)
				{
					idx = (float)j / samples;
					off = vel * (idx - 0.5);

					res.rgb += tex2D(_MainTex, i.uv + off);
				}

				res.rgb /= samples;

				return res;
			}

			ENDCG
		}

		Pass
		{
			ZWrite On

			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			struct v2f {
				float4 pos : POSITION;
				float4 cur : TEXCOORD0;
				float4 prv : TEXCOORD1;
				float4 screen : TEXCOORD2;
			};

			v2f vert(appdata_full v)
			{
				v2f o;

				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

				o.cur = o.pos;
				o.prv = mul(_Proj, v.vertex);

				o.screen = ComputeScreenPos(o.pos);

				return o;
			}

			float4 frag(v2f i) : COLOR
			{
				float a = Linear01Depth(((i.screen.z / i.screen.w) * 0.5) + 0.5);
				float b = DecodeFloatRG(tex2Dproj(_CameraDepthNormalsTexture, UNITY_PROJ_COORD(i.screen)).zw);

				if ((a - b) > EPSILON)
				{
					discard;
				}

				return float4(EncodeMotion(i.cur, i.prv), 0.0, 0.0);
			}

			ENDCG
		}
	}
}