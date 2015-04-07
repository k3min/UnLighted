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

			static const float2 kernel[8] = {
				float2(-0.926212,-0.40581),
				float2(-0.695914, 0.457137),
				float2(-0.203345, 0.820716),
				float2( 0.96234, -0.194983),
				float2( 0.473434,-0.480026),
				float2( 0.519456, 0.767022),
				float2( 0.185461,-0.893124),
				float2( 0.89642,  0.412458)
			};

			float4 frag(v2f_img i) : COLOR
			{
				float2 uv = i.uv - 0.5;
				float off = length(0.5 - i.uv) * L;

				uv *= (off * _Params.x) + 1.0;
				uv *= _Params.y;

				float4 res = 0.0;

				for (int j = 0; j < 8; j++)
				{
					float2 blur = kernel[j];

					blur *= _Params.z;
					blur *= off;
					blur *= _MainTex_TexelSize.xy;
					blur += uv;

					res.rb += tex2D(_MainTex, blur + 0.5).rb;
				}

				res.rb /= 8.0;
				res.ga = tex2D(_MainTex, uv + 0.5).ga;

				return res * pow(1.0 - off, _Params.w);
			}

			ENDCG
		}
	}
}