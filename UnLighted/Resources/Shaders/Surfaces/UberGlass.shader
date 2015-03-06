Shader "UnLighted/Über Glass"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1)
		_Normal("Normal", 2D) = "bump" {}
		_Roughness("Roughness", 2D) = "white" {}
		_Params("Scale, Roughness, Height", Vector) = (1, 1, 1, 0)
		[HideInInspector]
		_Hack("", 2D) = "bump" {}
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
			"IgnoreProjector" = "True"
		}

		Cull Off
		ZWrite Off

		CGINCLUDE

		#define UNITY_PASS_FORWARDBASE

		#include "UnityCG.cginc"

		#include "./../Includes/Vars.cginc"
		#include "./../Includes/Util.cginc"
		#include "./../Includes/Uber.cginc"

		#pragma glsl
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma only_renderers d3d11 opengl

		#pragma multi_compile REFLECTIONS_OFF REFLECTIONS_ON

		float3 _Color;
		sampler2D _Normal;
		sampler2D _Roughness;
		sampler2D _Hack;
		sampler2D _GrabTexture;
		float4 _GrabTexture_TexelSize;

		ENDCG

		GrabPass
		{
		}

		Pass
		{
			CGPROGRAM

			#pragma vertex vert_uber
			#pragma fragment frag

			float4 frag(v2f_uber i) : COLOR
			{
				float2 uv = i.uv.xy * _Params.x;

				float4 params = saturate(_Params);

				float4 n0 = tex2D(_Hack, uv);
				float4 n1 = tex2D(_Normal, uv);

				float3 normal = UnpackNormal(lerp(n0, n1, params.z));
				float a = max(tex2D(_Roughness, uv).r * params.y, EPSILON);

				i.screen.xy += normal.xy * 0.5;

				float3 res = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.screen)).rgb * _Color;

				i.viewDir = normalize(i.viewDir);

				float3 h = normalize(i.multi.xyz + i.viewDir);
				float NdL = saturate(dot(normal, i.multi.xyz));
				float NdV = abs(dot(normal, i.viewDir));
				float NdH = saturate(dot(normal, h));
				float VdH = saturate(dot(i.viewDir, h));

				res += Specular(0.03, a, NdL, NdV, NdH, VdH);

			#ifdef REFLECTIONS_ON
				float4 cube;

				cube.xyz = BPCEM(i, normal);
				cube.w = a * 8.0;

				res += HDRDecode(texCUBElod(_Box, cube)) * F3(0.03, a, NdV);
			#endif

				return float4(res, 1.0);
			}

			ENDCG
		}
	}
}