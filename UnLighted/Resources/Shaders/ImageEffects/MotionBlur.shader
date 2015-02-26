Shader "Hidden/UnLighted/MotionBlur"
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
		#include "./../Includes/Util.cginc"

		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma only_renderers d3d11 opengl

		float4x4 _Proj;

		ENDCG

		Pass
		{
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag

			float4 frag(v2f_img i) : COLOR
			{
				float d = tex2D(_CameraDepthTexture, i.uv).x;

				float4 pos = float4((i.uv * 2.0) - 1.0, d, 1.0);
				float4 prv = mul(_Proj, pos);

				return float4(MotionVector(pos, prv), 0.0, 0.0);
			}

			ENDCG
		}

		Pass
		{
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag

			#pragma glsl

			sampler2D _MotionTex;
			float4 _MotionTex_TexelSize;
			float _MotionScale;
			int _MaxSamples;

			float4 frag(v2f_img i) : COLOR
			{
				float2 vel = (tex2D(_MotionTex, i.uv).xy * 2.0) - 1.0;

				vel *= _MotionScale;

				int magnitude = round(length(vel * _MotionTex_TexelSize.zw));
				int samples = clamp(magnitude, 1, _MaxSamples);

				float4 res = tex2D(_MainTex, i.uv);

				for (int j = 1; j < samples; j++)
				{
					float idx = j;
					float smp = samples - 1;

					float2 uv = vel * ((idx / smp) - 0.5);

					res += tex2D(_MainTex, i.uv + uv);
				}

				res /= float(samples);

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
				float3 uv = i.screen.xyz / i.screen.w;

				float d = tex2D(_CameraDepthTexture, uv.xy).x;
				float z = (uv.z * 0.5) + 0.5;

				if (d < (z - EPSILON))
				{
					discard;
				}

				return float4(MotionVector(i.cur, i.prv), 0.0, 0.0);
			}

			ENDCG
		}
	}
}