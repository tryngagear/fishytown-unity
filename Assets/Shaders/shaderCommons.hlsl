#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
TEXTURE2D(_NormalMap); SAMPLER(sampler_NormalMap);

float4 _BaseMap_ST;
float4 _BaseColor;
float _Cutoff;
float _Smoothness;
float _NormalStrength;

void TestAlphaClip(float4 colorSample) {
#ifdef _ALPHA_CUTOUT
	clip(colorSample.a * _BaseColor.a - _Cutoff);
#endif
}