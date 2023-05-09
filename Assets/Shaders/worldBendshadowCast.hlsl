struct Attributes{
    float3 normalOS : NORMAL;
    float3 posOS: POSITION;
#ifdef _ALPHA_CUTOUT
    float2 uv : TEXCOORD0;
#endif
    };

struct Varyings{
    float4 posHCS: SV_POSITION;
#ifdef _ALPHA_CUTOUT
    float2 uv : TEXCOORD0;
#endif

    };

float3 _LightDirection;

float3 FlipNormalBasedOnViewDir(float3 normalWS, float3 positionWS) {
	float3 viewDirWS = GetWorldSpaceNormalizeViewDir(positionWS);
	return normalWS * (dot(normalWS, viewDirWS) < 0 ? -1 : 1);
}

float4 GetShadowCasterPositionCS(float3 positionWS, float3 normalWS) {
	float3 lightDirectionWS = _LightDirection;
#ifdef _DOUBLE_SIDED_NORMALS 
    normalWS = FlipNormalBasedOnViewDir(normalWS, positionWS);
#endif

	float4 positionCS = TransformWorldToHClip(ApplyShadowBias(positionWS, normalWS, lightDirectionWS));

#if UNITY_REVERSED_Z
	positionCS.z = min(positionCS.z, UNITY_NEAR_CLIP_VALUE);
#else
	positionCS.z = max(positionCS.z, UNITY_NEAR_CLIP_VALUE);
#endif
	return positionCS;
}

Varyings vert(Attributes IN){
    Varyings OUT;

    VertexPositionInputs posnInputs = GetVertexPositionInputs(IN.posOS);
    VertexNormalInputs normInputs = GetVertexNormalInputs(IN.normalOS);

    OUT.posHCS = GetShadowCasterPositionCS(posnInputs.positionWS, normInputs.normalWS);
#ifdef _ALPHA_CUTOUT
	OUT.uv = TRANSFORM_TEX(IN.uv, _BaseMap);
#endif
    return OUT;
}

float4 frag(Varyings IN): SV_TARGET{
#ifdef _ALPHA_CUTOUT
    float2 uv = IN.uv;
    float4 colorSample = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv);
    TestAlphaClip(colorSample);
#endif

    return 0;
}