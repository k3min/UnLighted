Shader "UnLighted/Über Translucent"
{
	Properties
	{
		_Color("Color", Color) = (1, 1, 1)
		_Albedo("Albedo", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_Roughness("Roughness", 2D) = "white" {}
		_Params("Roughness, Translucency, Thickness", Vector) = (1, 0.25, 1)
	}

	SubShader
	{
		Tags { "RenderType" = "Translucent" }

		CGINCLUDE

		#include "UnityCG.cginc"

		#include "./../Includes/Vars.cginc"
		#include "./../Includes/BRDF.cginc"
		#include "./../Includes/Util.cginc"
		#include "./../Includes/Uber.cginc"

		#pragma glsl
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma only_renderers d3d11 opengl

		#pragma multi_compile REFLECTIONS_OFF REFLECTIONS_ON

		float3 _Color;
		sampler2D _Albedo;
		sampler2D _Normal;
		sampler2D _Roughness;

		float4 frag(v2f_uber i) : COLOR
		{
			Surface s;

			s.Albedo = tex2D(_Albedo, i.uv.xy).rgb * _Color.rgb;
			s.Normal = UnpackNormal(tex2D(_Normal, i.uv.xy));
			s.Roughness = tex2D(_Roughness, i.uv.xy).x * _Params.x;
			s.Thickness = max(_Params.z, EPSILON);
			s.Translucency = max(_Params.y, EPSILON);

			return Translusency(i, s);
		}

		ENDCG

		Pass
		{
			Tags { "LightMode" = "ForwardBase" }

			CGPROGRAM

			#pragma vertex vert_uber
			#pragma fragment frag

			#pragma multi_compile_fwdbase

			ENDCG
		}

		Pass
		{
			Tags { "LightMode" = "ForwardAdd" }

			Blend One One

			CGPROGRAM

			#pragma vertex vert_uber
			#pragma fragment frag

			ENDCG
		}

		UsePass "UnLighted/Über/SHADOWCASTER"
	}

	CustomEditor "UberTranslucencyMaterialEditor"
}