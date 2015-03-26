Shader "UnLighted/Über Glass"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1)
		_Normal("Normal", 2D) = "bump" {}
		_Roughness("Roughness", 2D) = "white" {}
		_Params("Scale, Roughness, Bumpiness", Vector) = (1, 1, 1)
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"RenderType" = "Transparent"
			"IgnoreProjector" = "True"
		}

		GrabPass
		{
		}

		Pass
		{
			ZWrite On

			CGPROGRAM

			#include "UnityCG.cginc"

			#include "./../Includes/Vars.cginc"
			#include "./../Includes/Util.cginc"
			#include "./../Includes/Uber.cginc"

			#pragma vertex vert_uber
			#pragma fragment frag

			#pragma glsl
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma only_renderers d3d11 opengl

			#pragma multi_compile REFLECTIONS_OFF REFLECTIONS_ON

			float3 _Color;
			sampler2D _Normal;
			sampler2D _Roughness;
			sampler2D _GrabTexture;

			float4 frag(v2f_uber i) : COLOR
			{
				float4 p = saturate(_Params);

				i.uv.xy *= p.x;

				float3 normal = lerp(FWD, UnpackNormal(tex2D(_Normal, i.uv.xy)), p.z);

				i.screen.xyz += normal;

				float3 res = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(i.screen)).rgb * _Color;

			#ifdef REFLECTIONS_ON
				res += Refl(i, normal, 0.03, tex2D(_Roughness, i.uv.xy).r * p.y);
			#endif

				return float4(res, 0.0);
			}

			ENDCG
		}
	}
}