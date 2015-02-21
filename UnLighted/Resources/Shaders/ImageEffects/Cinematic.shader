Shader "Hidden/UnLighted/Cinematic"
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

		#pragma fragmentoption ARB_precision_hint_fastest
		#pragma only_renderers d3d11 opengl

		float _Threshhold;
		float _Intensity;
		float2 _Offsets;
		sampler2D _MainTex;
		float2 _MainTex_TexelSize;

		struct v2f_multi {
			float4 pos : POSITION;
			float2 uv[7] : TEXCOORD0;
		};

		v2f_multi vert_multi(appdata_full v)
		{
			v2f_multi o;

			o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

			o.uv[0] = v.texcoord;

			o.uv[1] = v.texcoord + 0.5 * _MainTex_TexelSize * _Offsets;
			o.uv[2] = v.texcoord - 0.5 * _MainTex_TexelSize * _Offsets;
			o.uv[3] = v.texcoord + 1.5 * _MainTex_TexelSize * _Offsets;
			o.uv[4] = v.texcoord - 1.5 * _MainTex_TexelSize * _Offsets;
			o.uv[5] = v.texcoord + 2.5 * _MainTex_TexelSize * _Offsets;
			o.uv[6] = v.texcoord - 2.5 * _MainTex_TexelSize * _Offsets;

			return o;
		}

		ENDCG

		Pass // 0
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			struct v2f {
				float4 pos : POSITION;
				float4 uv[5] : TEXCOORD0;
			};

			v2f vert(appdata_full v)
			{
				v2f o;

				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

				o.uv[0].xy = v.texcoord;

				o.uv[1]    = v.texcoord.xyxy + _Offsets.xyxy * float4(1, 1,-1,-1);
				o.uv[2]    = v.texcoord.xyxy + _Offsets.xyxy * float4(1, 1,-1,-1) * 2.0;
				o.uv[3]    = v.texcoord.xyxy + _Offsets.xyxy * float4(1, 1,-1,-1) * 3.0;
				o.uv[4]    = v.texcoord.xyxy + _Offsets.xyxy * float4(1, 1,-1,-1) * 4.0;

				return o;
			}

			float4 frag(v2f i) : COLOR
			{
				float4 res = float4(0, 0, 0, 0);

				res += 0.225 * tex2D(_MainTex, i.uv[0].xy);

				res += 0.150 * tex2D(_MainTex, i.uv[1].xy);
				res += 0.150 * tex2D(_MainTex, i.uv[1].zw);

				res += 0.110 * tex2D(_MainTex, i.uv[2].xy);
				res += 0.110 * tex2D(_MainTex, i.uv[2].zw);

				res += 0.075 * tex2D(_MainTex, i.uv[3].xy);
				res += 0.075 * tex2D(_MainTex, i.uv[3].zw);

				res += 0.0525 * tex2D(_MainTex, i.uv[4].xy);
				res += 0.0525 * tex2D(_MainTex, i.uv[4].zw);

				return res;
			}

			ENDCG
		}

		Pass // 1
		{
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag

			sampler2D _Bloom;

			float4 frag(v2f_img i) : COLOR
			{
				float4 color = tex2D(_MainTex, i.uv);
				float4 bloom = tex2D(_Bloom, i.uv);

				return color + (bloom * _Intensity);
			}

			ENDCG
		}

		Pass // 2
		{
			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag

			float4 frag(v2f_img i) : COLOR
			{
				float4 color = tex2D(_MainTex, i.uv);

				color.rgb = max(color.rgb - _Threshhold, 0.0);

				return color;
			}

			ENDCG
		}

		Pass // 3
		{
			CGPROGRAM

			#pragma vertex vert_multi
			#pragma fragment frag

			float3 _Color;

			float4 frag(v2f_multi i) : COLOR
			{
				float4 color = tex2D(_MainTex, i.uv[0]);

				color += tex2D(_MainTex, i.uv[1]);
				color += tex2D(_MainTex, i.uv[2]);
				color += tex2D(_MainTex, i.uv[3]);
				color += tex2D(_MainTex, i.uv[4]);
				color += tex2D(_MainTex, i.uv[5]);
				color += tex2D(_MainTex, i.uv[6]);

				color = max((color / 7.0) - _Threshhold, 0.0);

				color.rgb *= _Color.rgb * _Intensity;

				return color;
			}

			ENDCG
		}

		Pass // 4
		{
			CGPROGRAM

			#pragma vertex vert_multi
			#pragma fragment frag

			float4 frag(v2f_multi i) : COLOR
			{
				float4 color = tex2D(_MainTex, i.uv[0]);

				color += tex2D(_MainTex, i.uv[1]);
				color += tex2D(_MainTex, i.uv[2]);
				color += tex2D(_MainTex, i.uv[3]);
				color += tex2D(_MainTex, i.uv[4]);
				color += tex2D(_MainTex, i.uv[5]);
				color += tex2D(_MainTex, i.uv[6]);

				return color / (7.0 + Luminance(color.rgb) + 0.5);
			}

			ENDCG
		}

		Pass // 5
		{
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			float _Stretch;

			v2f_multi vert(appdata_full v)
			{
				v2f_multi o;

				o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

				o.uv[0] = v.texcoord;

				o.uv[1] = v.texcoord + _Stretch * 2.0 * _Offsets;
				o.uv[2] = v.texcoord - _Stretch * 2.0 * _Offsets;
				o.uv[3] = v.texcoord + _Stretch * 4.0 * _Offsets;
				o.uv[4] = v.texcoord - _Stretch * 4.0 * _Offsets;
				o.uv[5] = v.texcoord + _Stretch * 6.0 * _Offsets;
				o.uv[6] = v.texcoord - _Stretch * 6.0 * _Offsets;

				return o;
			}

			float4 frag(v2f_multi i) : COLOR
			{
				float4 color = tex2D(_MainTex, i.uv[0]);

				color = max(color, tex2D(_MainTex, i.uv[1]));
				color = max(color, tex2D(_MainTex, i.uv[2]));
				color = max(color, tex2D(_MainTex, i.uv[3]));
				color = max(color, tex2D(_MainTex, i.uv[4]));
				color = max(color, tex2D(_MainTex, i.uv[5]));
				color = max(color, tex2D(_MainTex, i.uv[6]));

				return color;
			}

			ENDCG
		}

		Pass // 6
		{
			Blend One One

			CGPROGRAM

			#pragma vertex vert_img
			#pragma fragment frag

			float4 frag(v2f_img i) : COLOR
			{
				return tex2D(_MainTex, i.uv);
			}

			ENDCG
		}
	}
}