Shader "Hidden/UnLighted/Pointer"
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

			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			#include "UnityCG.cginc"
			#include "./../Includes/Vars.cginc"

			#pragma vertex vert_img
			#pragma fragment frag

			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma only_renderers d3d11 opengl

			float4 _Color;

			float4 frag(v2f_img i) : COLOR
			{
				float4 res = _Color;

				res.a *= pow(sin(i.uv.y * PI), 2.0);

				return res;
			}

			ENDCG
		}
	}
}	