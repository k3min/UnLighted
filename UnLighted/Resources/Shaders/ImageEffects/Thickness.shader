Shader "Hidden/UnLighted-ImageEffects-Thickness"
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

			float4 frag(v2f_img i) : COLOR
			{
				float d1 = DecodeFloatRGBA(tex2D(_MainTex, i.uv));
				float d2 = DecodeFloatRG(tex2D(_CameraDepthNormalsTexture, i.uv).zw);

				return 1.0 - saturate((d1 - d2) * _ProjectionParams.z);
			}

			ENDCG
		}
	}

	SubShader
	{
		Tags { "RenderType" = "Translucent" }

		Pass
		{
			Cull Front

			CGPROGRAM

			#include "UnityCG.cginc"
			#include "./../Includes/Vars.cginc"

			#pragma vertex vert
			#pragma fragment frag

			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma only_renderers d3d11 opengl

			struct v2f {
				float4 pos : POSITION;
				float depth : TEXCOORD0;
			};

			v2f vert(appdata_full v)
			{
				v2f o;

				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.depth = -mul(UNITY_MATRIX_MV, v.vertex).z * _ProjectionParams.w;

				return o;
			}

			float4 frag(v2f i) : COLOR
			{
				return EncodeFloatRGBA(min(i.depth, 1.0 - EPSILON));
			}

			ENDCG
		}
	}
}