Shader "Hidden/Internal-PrePassLighting"
{
	SubShader
	{
		Pass
		{
		}

		Pass
		{
			ZWrite Off

			Blend One One

			CGPROGRAM

			#include "UnityCG.cginc"

			#include "./../Includes/Vars.cginc"
			#include "./../Includes/BRDF.cginc"
			#include "./../Includes/Util.cginc"

			#pragma vertex vert
			#pragma fragment frag

			#pragma glsl
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma only_renderers d3d11 opengl

			#pragma multi_compile_lightpass

			float4 _LightPos;
			float3 _LightColor;
			sampler2D _LightTextureB0;
			samplerCUBE _ShadowMapTexture;

			CBUFFER_START(UnityPerCamera2)
			float4x4 _CameraToWorld;
			CBUFFER_END

			struct v2f {
				float4 pos : POSITION;
				float4 uv : TEXCOORD0;
				float3 ray : TEXCOORD1;
			};

			inline float SampleShadowCube(float3 vec)
			{
				float fade = length(vec) * _LightPositionRange.w * (1.0 - _Shadows.z);

			#ifdef SHADOWS_SOFT
				float4 shadow = 0;

				float idx;
				float2 z;
				float4 sample;

				for (int i = 0; i < 4; i++)
				{
					idx = i;

					z = (idx + 1.0) * float2(1.0, -1.0) * _Shadows.y;

					if ((i % 2) == 1)
					{
						z = -z;
					}

					sample.x = DecodeFloatRGBA(texCUBE(_ShadowMapTexture, vec + z.xxx));
					sample.y = DecodeFloatRGBA(texCUBE(_ShadowMapTexture, vec + z.yyx));
					sample.z = DecodeFloatRGBA(texCUBE(_ShadowMapTexture, vec + z.yxy));
					sample.w = DecodeFloatRGBA(texCUBE(_ShadowMapTexture, vec + z.xyy));

					shadow += (sample < (fade - (idx * _Shadows.w))) ? _LightShadowData.r : 1.0;
				}

				return dot(shadow, 1.0 / (4.0 * 4.0));
			#else
				return DecodeFloatRGBA(texCUBE(_ShadowMapTexture, vec)) < fade ? _LightShadowData.r : 1.0;
			#endif
			}

			v2f vert(appdata_full v)
			{
				v2f o;

				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = ComputeScreenPos(o.pos);
				o.ray = mul(UNITY_MATRIX_MV, v.vertex).xyz * float3(-1,-1, 1);

				return o;
			}

			float4 frag(v2f i) : COLOR
			{
				i.ray *= _ProjectionParams.z / i.ray.z;

				float depth = Linear01Depth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.uv)));

				float3 viewPos = i.ray * depth;
				float3 worldPos = mul(_CameraToWorld, float4(viewPos, 1.0));

				float fade = distance(worldPos, unity_ShadowFadeCenterAndType.xyz);

				fade = lerp(viewPos.z, fade, unity_ShadowFadeCenterAndType.w);
				fade = LightmapFade(fade);

				if (fade >= (1.0 - EPSILON))
				{
					discard;
				}

				float4 buffer = tex2Dproj(_CameraNormalsTexture, UNITY_PROJ_COORD(i.uv));
				float3 normal = DecodeNormal(buffer.xy);
				float a = max(buffer.z, EPSILON);
				float metallic = buffer.w;

				float3 light = _LightPos.xyz - worldPos;
				float3 lightDir = normalize(light);

				float2 uv = dot(light, light) * _LightPos.w;

				float atten = tex2D(_LightTextureB0, uv).UNITY_ATTEN_CHANNEL;

				atten *= SampleShadowCube(-light);

				float3 viewDir = normalize(_WorldSpaceCameraPos - worldPos);

				float4 res = BRDF(normal, metallic, a, lightDir, viewDir);

				res.rgb *= _LightColor;
				res.a *= Luminance(_LightColor);

				res *= saturate(atten);
				res *= saturate(1.0 - fade);

				return res;
			}

			ENDCG
		}

		Pass
		{
		}
	}
}