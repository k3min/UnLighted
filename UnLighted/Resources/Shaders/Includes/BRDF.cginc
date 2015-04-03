#ifndef BRDF_INCLUDED
#define BRDF_INCLUDED

inline float D(float a, float NdH) // GGX Distribution
{
	float a2 = a * a;
	float NdH2 = NdH * NdH;

	float denominator = (NdH2 * (a2 - 1.0)) + 1.0;

	denominator *= denominator;
	denominator *= PI;

	return a2 / denominator;
}

inline float G(float a, float NdV, float NdL) // Smith-Schlick GGX Geometry
{
	float k = a * 0.5;
	float GV = NdV / ((NdV * (1.0 - k)) + k);
	float GL = NdL / ((NdL * (1.0 - k)) + k);

	return GV * GL;
}

inline float F(float spec, float a, float VdH) // Schlick Fresnel
{
	return spec + ((1.0 - spec) * pow(1.0 - VdH, _Fresnel));
}

inline float Specular(float spec, float a, float NdL, float NdV, float NdH, float VdH) // Cook-Torrance Specular
{
	return (F(spec, a, VdH) * G(a, NdV, NdL) * D(a, NdH)) / max(4.0 * NdL * NdV, EPSILON);
}

// http://graphicrants.blogspot.com/2013/08/specular-brdf-reference.html
inline float4 BRDF(float3 normal, float spec, float a, float3 lightDir, float3 viewDir)
{
	float3 h = normalize(viewDir + lightDir);
	float NdL = saturate(dot(normal, lightDir));
	float NdV = saturate(dot(normal, viewDir));
	float NdH = saturate(dot(normal, h));
	float VdH = saturate(dot(viewDir, h));

	float4 res;

	res.rgb = NdL;
	res.a = Specular(spec, a, NdL, NdV, NdH, VdH);

	return res;
}

#endif