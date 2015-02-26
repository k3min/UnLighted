struct v2f_shadow {
	float4 pos : POSITION;
	float3 vec : TEXCOORD0;
};

v2f_shadow vert_shadow(appdata_full v)
{
	v2f_shadow o;

	o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
	o.vec = mul(_Object2World, v.vertex).xyz - _LightPositionRange.xyz;

	return o;
}

float4 frag_shadow(v2f_shadow i) : COLOR
{
	return EncodeFloatRGBA(min(length(i.vec) * _LightPositionRange.w, 1.0 - EPSILON));
}