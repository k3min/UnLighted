Shader "UnLighted/Über Glass"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1)
		_Normal("Normal", 2D) = "bump" {}
		_Roughness("Roughness", 2D) = "white" {}
		_Params("Scale, Roughness, Height", Vector) = (1, 1, 1)
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
				i.uv.xy *= _Params.x;

				float4 params = saturate(_Params);

				float4 n0 = tex2D(_Hack, i.uv.xy);
				float4 n1 = tex2D(_Normal, i.uv.xy);

				float3 normal = UnpackNormal(lerp(n0, n1, params.z));

				i.screen.xyz += normal;

				float3 res = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.screen)).rgb * _Color;

			#ifdef REFLECTIONS_ON
				float a = tex2D(_Roughness, i.uv.xy).r * params.y;

				a = max(a, EPSILON);

				float4 cube = float4(BPCEM(i, normal), a * 8.0);
				float3 env = HDRDecode(texCUBElod(_Box, cube));

				res += env * F3(0.03, a, dot(normalize(i.viewDir), normal));
			#endif

				return float4(res, 0.0);
			}

			ENDCG
		}
	}
}