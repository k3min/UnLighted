Shader "Hidden/UnLighted/ColorGrading"
{
	Properties
	{
		_MainTex("", 2D) = "" {}
	}

	Subshader
	{
		Pass
		{
			ZWrite Off

			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma vertex vert_img
			#pragma fragment frag

			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma only_renderers d3d11 opengl

			sampler2D _MainTex;
			sampler3D _LUT;
			float _Scale;
			float _Offset;

			float4 frag(v2f_img i) : COLOR
			{
				float4 color = tex2D(_MainTex, i.uv);

				color.rgb = sqrt(color.rgb);
				color.rgb = tex3D(_LUT, color.rgb * _Scale + _Offset).rgb;
				color.rgb *= color.rgb;

				return color;
			}

			ENDCG
		}
	}
}