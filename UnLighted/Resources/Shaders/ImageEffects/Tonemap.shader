Shader "Hidden/UnLighted-ImageEffects-Tonemap"
{
	Properties
	{
		_MainTex("", 2D) = "" {}
	}

	SubShader
	{
		ZWrite Off

		CGINCLUDE

		#include "UnityCG.cginc"
		#include "./../Includes/Vars.cginc"

		#pragma glsl
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma only_renderers d3d11 opengl

		float _AdaptionRate;
		int _Level;

		float4 fragTest(float4 v : POSITION) : COLOR
		{
			float4 s;

			s.xy = 0.5;
			s.w = _Level;

			float4 r;

			r.rgb = Luminance(tex2Dlod(_MainTex, s).rgb);
			r.a = 1.0 - exp2(unity_DeltaTime * -_AdaptionRate);

			return r;
		}

		ENDCG

		Pass
		{
			Blend SrcAlpha OneMinusSrcAlpha

			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment fragTest

			ENDCG
		}

		Pass
		{
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment fragTest

			ENDCG
		}

		Pass
		{
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _Adapted;
			float _Exposure;

			inline float log10(float x)
			{
				return log(x) / 2.30258509299404568401;
			}

			inline float Autokey(float lum)
			{
				return 1.03 - (2.0 / (2.0 + log10(lum + 1.0)));
			}

			inline float4 Hejl(float4 color)
			{
				float4 x = max(color - 0.004, 0.0);

				return (x * ((6.2 * x) + 0.5)) / ((x * ((6.2 * x) + 1.7)) + 0.06);
			}

			float4 frag(v2f_img i) : COLOR
			{
				float adapted = tex2D(_Adapted, 0.5.xx).r;
				float exposure = _Exposure * Autokey(adapted) / adapted;

				return Hejl(tex2D(_MainTex, i.uv) * exposure);
			}

			ENDCG
		}
	}
}