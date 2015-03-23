// http://john-chapman-graphics.blogspot.com/2013/02/pseudo-lens-flare.html
Shader "Hidden/UnLighted-ImageEffects-Flares"
{
	Properties
	{
		_MainTex("", 2D) = "" {}
	}

	SubShader
	{
		Pass
		{
			ZWrite Off

			CGPROGRAM

			#include "UnityCG.cginc"
			#include "./../Includes/Vars.cginc"

			#pragma vertex vert_img
			#pragma fragment frag

			#pragma glsl
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma only_renderers d3d11 opengl

			sampler2D _LensDirt;
			sampler2D _LensColor;

			inline float3 Dist(float2 uv, float2 dir)
			{
				float3 res;

				float2 off = dir * _Params.z * _MainTex_TexelSize.x;

				res.r = tex2D(_MainTex, uv - off).r;
				res.g = tex2D(_MainTex, uv).g;
				res.b = tex2D(_MainTex, uv + off).b;

				return res;
			}

			inline float3 Flare(float2 uv, float2 dir)
			{
				return Dist(uv, dir) * pow(1.0 - saturate(length(0.5 - uv) * L), 10.0);
			}

			float4 frag(v2f_img i) : COLOR
			{
				float2 uv = 1.0 - i.uv;
				float2 c = 0.5 - uv;
				float2 dir = normalize(c);

				float2 ghost = c * _Params.x;
				float2 halo = dir * _Params.y;

				float3 res = 0.0;

				for (int j = 0; j < 8; j++)
				{
					res += Flare(uv + (ghost * (float)j), dir);
				}

				res *= tex2D(_LensColor, length(c).xx * L);
				res += Flare(uv + halo, dir);

				return float4(res, 1.0);
			}

			ENDCG
		}
	}
}