Shader "Hidden/UnLighted-ImageEffects-Visualize"
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

		Pass // 0: Geomery normals
		{
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag

			float4 frag(v2f_img i) : COLOR
			{
				return float4(DecodeNormal(tex2D(_CameraNormalsTexture, i.uv).xy), 1.0);
			}

			ENDCG
		}

		Pass // 1: Geomery roughness
		{
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag

			float4 frag(v2f_img i) : COLOR
			{
				return tex2D(_CameraNormalsTexture, i.uv).z;
			}

			ENDCG
		}

		Pass // 2: Geomery specular
		{
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag

			float4 frag(v2f_img i) : COLOR
			{
				return tex2D(_CameraNormalsTexture, i.uv).w;
			}

			ENDCG
		}

		Pass // 3: Depth
		{
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag

			float4 frag(v2f_img i) : COLOR
			{
				return Linear01Depth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv));
			}

			ENDCG
		}

		Pass // 4: LightBuffer color
		{
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag

			float4 frag(v2f_img i) : COLOR
			{
				return float4(tex2D(_LightBuffer, i.uv).xyz, 1.0);
			}

			ENDCG
		}

		Pass // 5: LightBuffer specular
		{
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag

			float4 frag(v2f_img i) : COLOR
			{
				return tex2D(_LightBuffer, i.uv).w;
			}

			ENDCG
		}

		Pass // 6: Thickness
		{
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag

			float4 frag(v2f_img i) : COLOR
			{
				return tex2D(_Thickness, i.uv).x;
			}

			ENDCG
		}

		Pass // 7: Combined view space normals
		{
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag

			float4 frag(v2f_img i) : COLOR
			{
				return float4(DecodeViewNormalStereo(tex2D(_CameraDepthNormalsTexture, i.uv)), 1.0);
			}

			ENDCG
		}

		Pass // 8: Combined depth
		{
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag

			float4 frag(v2f_img i) : COLOR
			{
				return DecodeFloatRG(tex2D(_CameraDepthNormalsTexture, i.uv).zw);
			}

			ENDCG
		}
	}
}