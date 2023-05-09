#ifndef WB_FORWARD_LIT_INCLUDED
#define WB_FORWARD_LIT_INCLUDED
#define BEND_CONST 0.0000f
// GLES2 has limited amount of interpolators
#if defined(_PARALLAXMAP) && !defined(SHADER_API_GLES)
#define REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR
#endif

#if (defined(_NORMALMAP) || (defined(_PARALLAXMAP) && !defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR))) || defined(_DETAIL)
#define REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR
#endif

struct Attributes{
    float4 positionOS   : POSITION;
    float3 normalOS     : NORMAL;
    float4 tangentOS    : TANGENT; 
    float2 texcoord     : TEXCOORD0;
    float2 staticLightmapUV     : TEXCOORD1;
    float2 dynamicLightmapUV    : TEXCOORD2;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct Varyings{
    float2 uv           : TEXCOORD0;
#if defined(REQUIRES_WORLD_SPACE_POS_INTERPOLATOR)
    float3 positionWS   : TEXCOORD1;
#endif

    float3 normalWS     : TEXCOORD2;
#if defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
    half4 tangentWS     : TEXCOORD3;
#endif
    float3 viewDirWS    : TEXCOORD4;

#ifdef _ADDITIONAL_LIGHTS_VERTEX
    half4 fogFactorAndVertexLight   : TEXCOORD5;
#else
    half  fogFactor                 : TEXCOORD5;
#endif

#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
    float4 shadowCoord              : TEXCOORD6;
#endif

#if defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
    half3 viewDirTS                : TEXCOORD7;
#endif

    DECLARE_LIGHTMAP_OR_SH(staticLightmapUV, vertexSH, 8);
#ifdef DYNAMICLIGHTMAP_ON
    float2  dynamicLightmapUV : TEXCOORD9;
#endif

    float4 positionCS               : SV_POSITION;
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

    float _WorldBend;

void InitInputData(Varyings IN, half3 normalTS, out InputData OUT)
{
    OUT = (InputData)0;

#if defined(REQUIRES_WORLD_SPACE_POS_INTERPOLATOR)
    OUT.positionWS = IN.positionWS;
#endif
    half3 viewDirWS = GetWorldSpaceNormalizeViewDir(IN.positionWS);
#if defined(_NORMALMAP) || defined(_DETAIL)
    float sgn = IN.tangentWS.w;
    float3 bitangent = sgn * cross(IN.normalWS.xyz, IN.tangentWS.xyz);
    half3x3 tangentToWorld = half3x3(IN.tangentWS.xyz, bitangent.xyz, IN.normalWS.xyz);

    #if defined(_NORMALMAP)
        OUT.tangentToWorld = tangentToWorld;
    #endif
    OUT.normalWS = TransformTangentToWorld(normalTS, tangentToWorld);
#else
    OUT.normalWS = IN.normalWS;
#endif
    OUT.normalWS = NormalizeNormalPerPixel(OUT.normalWS);
    OUT.viewDirectionWS = viewDirWS;
#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
    OUT.shadowCoord = IN.shadowCoord;
#elif defined(MAIN_LIGHT_CALCULATE_SHADOWS)
    OUT.shadowCoord = TransformWorldToShadowCoord(OUT.positionWS);
#else
    OUT.shadowCoord = float4(0, 0, 0, 0);
#endif
#ifdef _ADDITIONAL_LIGHTS_VERTEX
    OUT.fogCoord = InitializeInputDataFog(float4(IN.positionWS, 1.0), IN.fogFactorAndVertexLight.x);
    OUT.vertexLighting = IN.fogFactorAndVertexLight.yzw;
#else
    OUT.fogCoord = InitializeInputDataFog(float4(IN.positionWS, 1.0), IN.fogFactor);
#endif

#if defined(DYNAMICLIGHTMAP_ON)
    OUT.bakedGI = SAMPLE_GI(IN.staticLightmapUV, IN.dynamicLightmapUV, IN.vertexSH, IN.normalWS);
#else
    OUT.bakedGI = SAMPLE_GI(IN.staticLightmapUV, IN.vertexSH, IN.normalWS);
#endif

    OUT.normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(IN.positionCS);
    OUT.shadowMask = SAMPLE_SHADOWMASK(IN.staticLightmapUV);

#if defined(DEBUG_DISPLAY)
    #if defined(DYNAMICLIGHTMAP_ON)
        OUT.dynamicLightmapUV = IN.dynamicLightmapUV;
    #endif
    #if defined(LIGHTMAP_ON)
        OUT.staticLightmapUV = IN.staticLightmapUV;
    #else
        OUT.vertexSH = IN.vertexSH;
    #endif
#endif
}

///////////////////////////////////////////////////////////////////////////////
//                  Vertex and Fragment functions                            //
///////////////////////////////////////////////////////////////////////////////

Varyings WorldbendLitPassVertex(Attributes IN)
{
    Varyings OUT = (Varyings)0;

    UNITY_SETUP_INSTANCE_ID(IN);
    UNITY_TRANSFER_INSTANCE_ID(IN, OUT);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

    float4 vv = mul(unity_ObjectToWorld, IN.positionOS);
    vv.xyz -= _WorldSpaceCameraPos.xyz; 
    vv = float4(0.0f, ((vv.z * vv.z) + (vv.x * vv.x)) * BEND_CONST, 0.0f, 0.0f);
    //vv *= -vv;
    //vv *= _WorldBend;
    //vv = float4(0.0f, vv.y, 0.0f, 0.0f); //option 2
    IN.positionOS += mul(unity_WorldToObject, vv);

    VertexPositionInputs    vertexInput = GetVertexPositionInputs(IN.positionOS.xyz);
    VertexNormalInputs      normalInput = GetVertexNormalInputs(IN.positionOS, IN.tangentOS);

    half3 vertexLight = VertexLighting(vertexInput.positionWS, normalInput.normalWS);

    half fogFactor = 0;

    #if !defined(_FOG_FRAGMENT)
        fogFactor = ComputeFogFactor(vertexInput.positionCS.z);
    #endif
    
    OUT.uv = TRANSFORM_TEX(IN.texcoord, _BaseMap);
    OUT.normalWS = normalInput.normalWS;

#if defined(REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR) || defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
    real sign = IN.tangentOS.w * GetOddNegativeScale();
    half4 tangentWS = half4(normalInput.tangentWS.xyz, sign);
#endif
#if defined(REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR)
    OUT.tangentWS = tangentWS;
#endif
#if defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
    half3 viewDirWS = GetWorldSpaceNormalizeViewDir(vertexInput.positionWS);
    half3 viewDirTS = GetViewDirectionTangentSpace(tangentWS, OUT.normalWS, viewDirWS);
    OUT.viewDirTS = viewDirTS;
#endif

    OUTPUT_LIGHTMAP_UV(IN.staticLightmapUV, unity_LightmapST, OUT.staticLightmapUV);

#ifdef DYNAMICLIGHTMAP_ON
    OUT.dynamicLightmapUV = IN.dynamicLightmapUV.xy * unity_DynamicLightmapST.xy + unity_DynamicLightmapST.zw;
#endif
    OUTPUT_SH(OUT.normalWS.xyz, OUT.vertexSH);
#ifdef _ADDITIONAL_LIGHTS_VERTEX
    OUT.fogFactorAndVertexLight = half4(fogFactor, vertexLight);
#else
    OUT.fogFactor = fogFactor;
#endif

#if defined(REQUIRES_WORLD_SPACE_POS_INTERPOLATOR)
    OUT.positionWS = vertexInput.positionWS;
#endif

#if defined(REQUIRES_VERTEX_SHADOW_COORD_INTERPOLATOR)
    OUT.shadowCoord = GetShadowCoord(vertexInput);
#endif
    OUT.positionCS = vertexInput.positionCS;
    return OUT;
}

half4 WorldbendLitPassFragment(Varyings IN) :SV_Target
{
    UNITY_SETUP_INSTANCE_ID(IN);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(IN);

#if defined(_PARALLAXMAP)
#if defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR)
    half3 viewDirTS = IN.viewDirTS;
#else
    half3 viewDirWS = GetWorldSpaceNormalizeViewDir(IN.positionWS);
    half3 viewDirTS = GetViewDirectionTangentSpace(IN.tangentWS, IN.normalWS, viewDirWS);
#endif
    ApplyPerPixelDisplacement(viewDirTS, IN.uv);
#endif
    SurfaceData surfaceData;
    InitializeStandardLitSurfaceData(IN.uv, surfaceData);

    InputData inputData;
    InitInputData(IN, surfaceData.normalTS, inputData);
    SETUP_DEBUG_TEXTURE_DATA(inputData, IN.uv, _BaseMap);

    #ifdef _DBUFFER
        ApplyDecalToSurfaceData(IN.posistionCS, surfaceData, inputData);
    #endif

    half4 color = UniversalFragmentPBR(inputData, surfaceData);

    color.rgb = MixFog(color.rgb, inputData.fogCoord);
    color.a = OutputAlpha(color.a, _Surface);

    return color;
}

#endif