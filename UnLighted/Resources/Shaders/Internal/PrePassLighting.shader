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
			#include "./../Includes/Util.cginc"
			#include "./../Includes/Uber.cginc"

			#pragma vertex vert
			#pragma fragment frag

			#pragma glsl
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma only_renderers d3d11 opengl

			#pragma multi_compile_lightpass

			float4 _LightPos;
			float3 _LightDir;
			float3 _LightColor;
			sampler2D _LightTextureB0;

			CBUFFER_START(UnityPerCamera2)
			float4x4 _CameraToWorld;
			CBUFFER_END

			struct v2f {
				float4 pos : POSITION;
				float4 uv : TEXCOORD0;
				float3 ray : TEXCOORD1;
			};

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

				float depth = Linear01Depth(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.uv)).x);

				float4 viewPos = float4(i.ray * depth, 1.0);
				float3 worldPos = mul(_CameraToWorld, viewPos).xyz;

				float fade = distance(worldPos, unity_ShadowFadeCenterAndType.xyz);

				fade = lerp(viewPos.z, fade, unity_ShadowFadeCenterAndType.w);

				float lightMap = (fade * unity_LightmapFade.z) + unity_LightmapFade.w;

				if (lightMap >= 1.0)
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

				float4 res = CalculateLight(normal, metallic, a, _LightColor.rgb, lightDir, viewDir);

				res *= saturate(atten);
				res *= saturate(1.0 - lightMap);

				return res;
			}

			ENDCG
		}

		Pass
		{
		}
	}
}