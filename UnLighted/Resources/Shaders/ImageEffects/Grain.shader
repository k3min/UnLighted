// http://devlog-martinsh.blogspot.com/2013/05/image-imperfections-and-film-grain-post.html
Shader "Hidden/UnLighted-ImageEffects-Grain"
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
		#include "./../Includes/Util.cginc"

		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma only_renderers d3d11 opengl

		ENDCG

		Pass
		{
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag

			#pragma glsl

			inline float Fade(float t)
			{
				return t * t * t * ((t * ((t * 6.0) - 15.0)) + 10.0);
			}

			inline float Grain(float2 uv)
			{
				float2 pf = frac(uv);

				float4 n;

				n.x = dot(Random(Random(uv).x).xy, pf);
				n.y = dot(Random(Random(uv).x + float2(0,1)).xy, pf - float2(0,1));
				n.z = dot(Random(Random(uv).x + float2(1,0)).xy, pf - float2(1,0));
				n.w = dot(Random(Random(uv).x + 1.0).xy, pf - 1.0);

				n.xy = lerp(n.xy, n.zw, Fade(pf.x));

				return lerp(n.x, n.y, Fade(pf.y));
			}

			float4 frag(v2f_img i) : COLOR
			{
				float4 res = tex2D(_MainTex, i.uv);

				float lum = Luminance(res.rgb);
				float grain = Grain(i.uv);

				lum += smoothstep(_Params.y, 0.0, lum);
				lum *= grain;

				res.rgb += (grain - lum) * _Params.x;

				return res;
			}

			ENDCG
		}

		Pass
		{
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag

			float4 frag(v2f_img i) : COLOR
			{
				float lum = Luminance(tex2D(_MainTex, i.uv).rgb);

				return lum + smoothstep(_Params.y, 0.0, lum);
			}

			ENDCG
		}
	}
}