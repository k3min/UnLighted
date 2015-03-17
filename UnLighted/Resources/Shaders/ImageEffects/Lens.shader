Shader "Hidden/UnLighted-ImageEffects-Lens"
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

			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma only_renderers d3d11 opengl

			float3 _ETA;

			float4 frag(v2f_img i) : COLOR
			{
				float2 uv = i.uv - 0.5;
				float3 eta = _ETA + 1.0;
				float f = (((uv.x * uv.x) + (uv.y * uv.y)) * _Params.x) + 1.0;

				float2 R = (uv * f * eta.x * _Params.y) + 0.5;
				float2 G = (uv * f * eta.y * _Params.y) + 0.5;
				float2 B = (uv * f * eta.z * _Params.y) + 0.5;

				float3 res;

				res.r = tex2D(_MainTex, R).r;
				res.g = tex2D(_MainTex, G).g;
				res.b = tex2D(_MainTex, B).b;

				return float4(res, 1.0);
			}

			ENDCG
		}
	}
}