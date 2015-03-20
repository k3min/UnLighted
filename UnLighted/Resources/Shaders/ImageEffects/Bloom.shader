Shader "Hidden/UnLighted-ImageEffects-Bloom"
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

		ENDCG

		Pass
		{
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _Bloom;

			float4 frag(v2f_img i) : COLOR
			{
				float4 color = tex2D(_MainTex, i.uv);
				float3 bloom = tex2D(_Bloom, i.uv).rgb;

				color.rgb += bloom * _Params.x;

				return color;
			}

			ENDCG
		}

		Pass
		{
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag

			static const float2 weights[7] = {
				float2(0.0205, 0),
				float2(0.0855, 0),
				float2(0.232,  0),
				float2(0.324,  1),
				float2(0.232,  0),
				float2(0.0855, 0),
				float2(0.0205, 0)
			};

			float4 frag(v2f_img i) : COLOR
			{
				float2 tex =_MainTex_TexelSize * _Params.xy;
				float2 off = i.uv - (tex * 3.0);

				float4 res = 0.0;

				for (int j = 0; j < 7; j++)
				{
					res += tex2D(_MainTex, off) * weights[j].xxxy;
					off += tex;
				}

				return res;
			}

			ENDCG
		}

		Pass
		{
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag

			float4 frag(v2f_img i) : COLOR
			{
				return max(tex2D(_MainTex, i.uv) - _Params.x, 0.0);
			}

			ENDCG
		}
	}
}