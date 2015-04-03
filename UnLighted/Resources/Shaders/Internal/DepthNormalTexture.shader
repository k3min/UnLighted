Shader "Hidden/Camera-DepthNormalTexture"
{
	SubShader
	{
		Tags { "RenderType" = "Opaque" }

		Pass
		{
			CGPROGRAM

			#include "UnityCG.cginc"

			#pragma vertex vert
			#pragma fragment frag

			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma only_renderers d3d11 opengl

			struct v2f {
				float4 pos : POSITION;
				float4 data : TEXCOORD0;
			};

			v2f vert(appdata_full v)
			{
				v2f o;

				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.data.xyz = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
				o.data.w = -(mul(UNITY_MATRIX_MV, v.vertex).z * _ProjectionParams.w);

				return o;
			}

			float4 frag(v2f i) : COLOR
			{
				return EncodeDepthNormal(i.data.w, i.data.xyz);
			}

			ENDCG
		}
	}

	SubShader
	{
		Tags { "RenderType" = "Translucent" }

		Pass
		{
			CGPROGRAM

			#include "UnityCG.cginc"
			#include "./../Includes/Vars.cginc"

			#pragma vertex vert
			#pragma fragment frag

			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma only_renderers d3d11 opengl

			struct v2f {
				float4 pos : POSITION;
				float4 data : TEXCOORD0;
			};

			v2f vert(appdata_full v)
			{
				v2f o;

				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.data.xyz = mul((float3x3)UNITY_MATRIX_IT_MV, v.normal);
				o.data.w = -(mul(UNITY_MATRIX_MV, v.vertex).z * _ProjectionParams.w);

				return o;
			}

			float4 frag(v2f i) : COLOR
			{
				return EncodeDepthNormal(i.data.w, i.data.xyz);
			}

			ENDCG
		}
	}
}