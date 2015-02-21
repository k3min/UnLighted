Shader "Hidden/UnLighted/HBAO"
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

		#pragma target 3.0
		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma only_renderers d3d11 opengl

		ENDCG

		Pass
		{
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag

			float4x4 _Proj;
			sampler2D _Random;
			float4 _ProjInfo;
			float _Radius;
			float _Intensity;

			static const float3 kernel[8] = {
				float3(0.01305719,0.5872321,-0.119337),
				float3(0.3230782,0.02207272,-0.4188725),
				float3(-0.310725,-0.191367,0.05613686),
				float3(-0.4796457,0.09398766,-0.5802653),
				float3(0.1399992,-0.3357702,0.5596789),
				float3(-0.2484578,0.2555322,0.3489439),
				float3(0.1871898,-0.702764,-0.2317479),
				float3(0.8849149,0.2842076,0.368524),
			};

			float4 frag(v2f_img i) : COLOR
			{
				float z = LinearEyeDepth(tex2D(_CameraDepthTexture, i.uv).x);
				float3 origin = float3(((i.uv * _MainTex_TexelSize.zw) * _ProjInfo.xy + _ProjInfo.zw) * z, z);

				float4 buffer = tex2D(_CameraNormalsTexture, i.uv);
				float3 normal = DecodeNormal(buffer.xy);

				float3 rvec = (tex2D(_Random, i.uv * 64.0).xyz * 2.0) - 1.0;
				float3 tangent = normalize(rvec - normal * dot(rvec, normal));
				float3 bitangent = cross(normal, tangent);
				float3x3 tbn = float3x3(tangent, bitangent, normal);

				float ao = 0.0;

				for (int j = 0; j < 8; ++j)
				{
					float3 sample = (mul(tbn, kernel[j]) * _Radius) + origin;
					float4 offset = float4(sample, 1.0);

					offset = mul(_Proj, offset);
					offset.xy /= offset.w;
					offset.xy = (offset.xy * 0.5) + 0.5;

					float sampleDepth = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, offset.xy));
					float diff = origin.z - sampleDepth;

					if (abs(diff) < _Radius)
					{
						ao += diff;
					}
				}

				return saturate(pow(1.0 - (ao / 8.0), _Intensity));
			}

			ENDCG
		}

		Pass
		{
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _AO;

			float4 frag(v2f_img i) : COLOR
			{
				return tex2D(_MainTex, i.uv) * tex2D(_AO, i.uv);
			}

			ENDCG
		}
	}
}