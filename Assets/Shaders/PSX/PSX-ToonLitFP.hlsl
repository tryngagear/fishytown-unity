#ifndef PSX_VERTEX_LIT_FORWARD_PASS_INCLUDED
#define PSX_VERTEX_LIT_FORWARD_PASS_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

// GLES2 has limited amount of interpolators
#if defined(_PARALLAXMAP) && !defined(SHADER_API_GLES)
#define REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR
#endif

#if (defined(_NORMALMAP) || (defined(_PARALLAXMAP) && !defined(REQUIRES_TANGENT_SPACE_VIEW_DIR_INTERPOLATOR))) || defined(_DETAIL)
#define REQUIRES_WORLD_SPACE_TANGENT_INTERPOLATOR
#endif

struct appdata{
    float4 vertex : POSITION;
    float4 tangent : TANGENT;
    float3 normal : NORMAL;
    float4 uv : TEXCOORD0;
    float4 staticLightmapUV : TEXCOORD1;
    float4 dynamicLightmapUV : TEXCOORD2;
    float4 uvLM : TEXCOORD3;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f{
    half4 posCS : SV_POSITION;
    float2 uv : TEXCOORD0;
    float2 uvLM : TEXCOORD1;
    float3 normalWS : TEXCOORD2;
    float3 normalOS : TEXCOORD8;
#if _NORMALMAP
    half3 tangentWS                 : TEXCOORD3;
    half3 bitangentWS               : TEXCOORD4;
#endif
    float3 viewDirWS                : TEXCOORD5;
    half4 posAndFogFactor   : TEXCOORD6;
#ifdef _MAIN_LIGHT_SHADOWS
    float4 shadowCoord              : TEXCOORD7;
#endif
    half3 vertCol : COLOR;
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

///////////////////////////////////////////////////////////////////////////////
//                  Vertex and Fragment functions                            //
///////////////////////////////////////////////////////////////////////////////

v2f vert (appdata v){
    v2f o = (v2f)0;

    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_TRANSFER_INSTANCE_ID(v,o);
    //UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

    half4 amb = unity_AmbientSky;
    float dist = length(mul(UNITY_MATRIX_MV, v.vertex));

    VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);
    VertexNormalInputs   normalInput = GetVertexNormalInputs(v.normal, v.tangent);

    float fogFactor = ComputeFogFactor(vertexInput.positionCS.z);

    float4 vertex = vertexInput.positionCS;
    vertex.xyz = vertexInput.positionCS.xyz/vertexInput.positionCS.w;
    vertex.x = floor(120*vertex.x)/120;
    vertex.y = floor(90*vertex.y)/90;
    vertex.xyz *= vertexInput.positionCS.w;
    o.posCS = vertex;

    float4 affinePos = vertex;
    o.uv = TRANSFORM_TEX(v.uv, _BaseMap);
    o.uv *= dist + (vertex.w)/dist/2;
    o.normalOS = dist + (vertex.w)/dist/2;
    o.normalWS = normalInput.normalWS;

    o.uvLM = v.uvLM.xy*unity_LightmapST.xy + unity_LightmapST.zw;

    o.posAndFogFactor = float4(vertexInput.positionWS, fogFactor);

#ifdef _MAIN_LIGHT_SHADOWS
    o.shadowCoord = GetShadowCoord(vertexInput);
#endif

    half3 viewDirWS = GetWorldSpaceViewDir(v.vertex);
    o.viewDirWS = viewDirWS;

    return o;
}

half4 frag (v2f i) : SV_Target 
{
    UNITY_SETUP_INSTANCE_ID(i);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
    
    SurfaceData surfaceData;
    InitializeStandardLitSurfaceData(i.uv/i.normalOS.r, surfaceData);   

    #if _NORMALMAP
        half3 normalWS = TransformTangentToWorld(surfaceData.normalTS,
        half3x3(i.tangentWS, i.bitangentWS, i.normalWS));
    #else
        half3 normalWS = i.normalWS;
    #endif
        normalWS = normalize(normalWS);

    #ifdef LIGHTMAP_ON
        // Normal is required in case Directional lightmaps are baked
        half3 bakedGI = SampleLightmap(i.uvLM, normalWS);
    #else
        // Samples SH fully per-pixel. SampleSHVertex and SampleSHPixel functions
        // are also defined in case you want to sample some terms per-vertex.
        half3 bakedGI = SampleSH(normalWS);
    #endif

    float3 positionWS = i.posAndFogFactor.xyz;
    half3 viewDirectionWS = SafeNormalize(GetCameraPositionWS() - positionWS);

    BRDFData brdfData;
    InitializeBRDFData(surfaceData.albedo, surfaceData.metallic, surfaceData.specular, surfaceData.smoothness, surfaceData.alpha, brdfData);

    #ifdef _MAIN_LIGHT_SHADOWS
        // Main light is the brightest directional light.
        // It is shaded outside the light loop and it has a specific set of variables and shading path
        // so we can be as fast as possible in the case when there's only a single directional light
        // You can pass optionally a shadowCoord (computed per-vertex). If so, shadowAttenuation will be
        // computed.
        Light mainLight = GetMainLight(i.shadowCoord);
    #else
        Light mainLight = GetMainLight();
    #endif

    half3 ambientCol = half3(unity_SHAr.w, unity_SHAg.w, unity_SHAb.w);

    //toon shading
    float _Glossiness = 5;
    float4 _SpecularColor = float4(0.9,0.9,0.9,1);
    float3 toonViewDir = i.viewDirWS;
    float3 halfVec = normalize(mainLight.direction + toonViewDir);

    float3 toonNormal = i.normalWS;//normalize(i.normalWS);
    float NdotH = dot(toonNormal, halfVec);
    float NdotL = dot(mainLight.direction, toonNormal);
    float li = smoothstep(0, 0.01, NdotL);//NdotL > 0 ? 1:0;
    float specIntesity = pow(NdotH*li, _Glossiness*_Glossiness);
    float specIntensSmooth = smoothstep(0, 0.01, specIntesity);
    float4 spec = specIntensSmooth * _SpecularColor;
    float3 lic = li * mainLight.color;

    float4 _RimColor = half4(1,1,1,1);
    float _RimAmount = 0.7;
    float _RimThresh = 0.1;

    float4 rimDot = 1 - dot(toonNormal, toonViewDir);
    float rimIntensity = rimDot * pow(NdotL, _RimThresh);
    rimIntensity = smoothstep(_RimAmount - 0.01, _RimAmount + 0.01, rimIntensity);
    float4 rim = rimDot* rimIntensity;

    half3 color = surfaceData.albedo * (lic + spec + rim + ambientCol);

    color += surfaceData.emission;

    float fogFactor = i.posAndFogFactor.w;

    // Mix the pixel color with fogColor. You can optionaly use MixFogColor to override the fogColor
    // with a custom one.
    color = MixFog(color, fogFactor);
    return half4(color, surfaceData.alpha);
}

#endif