Shader "Hidden/UnLighted-ImageEffects-BoxProbe"
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
			#include "./../Includes/Util.cginc"

			#pragma vertex vert_img
			#pragma fragment frag

			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma only_renderers d3d11 opengl

			float4 frag(v2f_img i) : COLOR
			{
				return HDREncode(tex2D(_MainTex, i.uv).rgb);
			}

			ENDCG
		}
	}
}