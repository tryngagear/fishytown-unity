#ifndef PSX_FORWARD_UNLIT_INCLUDED
#define PSX_FORWARD_UNLIT_INCLUDED

struct appdata{
    float4 vertex : POSITION;
    float4 tangent : TANGENT;
    float3 normal : NORMAL;
    float4 uv : TEXCOORD0;
    float4 texcoord1 : TEXCOORD1;
    float4 texcoord2 : TEXCOORD2;
    float4 texcoord3 : TEXCOORD3;
    half4 color : COLOR;
    UNITY_VERTEX_INPUT_INSTANCE_ID
};

struct v2f{
    half4 pos : SV_POSITION;
    half4 color : COLOR0;
    half4 colorFog : COLOR1;
    float2 uv : TEXCOORD0;
    float3 viewDirWS : TEXCOORD4;
    half3 normal : TEXCOORD1;
    float fogCoord : TEXCOORD2;     
                
    UNITY_VERTEX_INPUT_INSTANCE_ID
    UNITY_VERTEX_OUTPUT_STEREO
};

void InitializeInputData(v2f input, out InputData inputData){
    inputData = (InputData)0;

    #if defined(DEBUG_DISPLAY)
    inputData.positionWS = input.positionWS;
    inputData.normalWS = input.normalWS;
    inputData.viewDirectionWS = input.viewDirWS;
    #else
    inputData.positionWS = float3(0, 0, 0);
    inputData.normalWS = half3(0, 0, 1);
    inputData.viewDirectionWS = half3(0, 0, 1);
    #endif
    inputData.shadowCoord = 0;
    inputData.fogCoord = 0;
    inputData.vertexLighting = half3(0, 0, 0);
    inputData.bakedGI = half3(0, 0, 0);
    inputData.normalizedScreenSpaceUV = 0;
    inputData.shadowMask = half4(1, 1, 1, 1);
    }

v2f vert (appdata v){
    v2f o = (v2f)0;

    UNITY_SETUP_INSTANCE_ID(v);
    UNITY_TRANSFER_INSTANCE_ID(v,o);
    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

    half4 amb = unity_AmbientSky;
    float dist = length(mul(UNITY_MATRIX_MV, v.vertex));

    VertexPositionInputs vertexInput = GetVertexPositionInputs(v.vertex.xyz);
    //float4 snapToPix = vertexInput.positionCS;
    float4 vertex = vertexInput.positionCS;
    vertex.xyz = vertexInput.positionCS.xyz/vertexInput.positionCS.w;
    vertex.x = floor(160*vertex.x)/160;
    vertex.y = floor(120*vertex.y)/120;
    vertex.xyz *= vertexInput.positionCS.w;
    o.pos = vertex;

    float4 affinePos = vertex;
    o.uv = TRANSFORM_TEX(v.uv, _BaseMap);
    o.uv *= dist + (vertex.w*(amb.a))/dist/2;
    o.normal = dist + (vertex.w*(amb.a))/dist/2;

    #if defined(_FOG_FRAGMENT)
    o.fogCoord = vertexInput.positionVS.z;
    #else
    o.fogCoord = ComputeFogFactor(vertexInput.positionCS.z);
    #endif

    #if defined(DEBUG_DISPLAY)
    VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
    half3 viewDirWS = GetWorldSpaceViewDir(vertexInput.positionWS);

    // already normalized from normal transform to WS.
    o.positionWS = vertexInput.positionWS;
    o.normalWS = normalInput.normalWS;
    o.viewDirWS = viewDirWS;
    #endif

	o.normal.b = 1;

    return o;
}

half4 frag (v2f i) : COLOR
{
    UNITY_SETUP_INSTANCE_ID(i);
    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
    half2 uv = i.uv;
    half4 texColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, uv/i.normal.r);
    half3 color = texColor.rgb * _BaseColor.rgb;
    half alpha = texColor.a * _BaseColor.a;

    AlphaDiscard(alpha, _Cutoff);            
    
    #if defined(_ALPHAPREMULTIPLY_ON)
    color *= alpha;
    #endif

    InputData inputData;
    InitializeInputData(i, inputData);
    SETUP_DEBUG_TEXTURE_DATA(inputData, i.uv, _BaseMap);

    #if defined(_FOG_FRAGMENT)
        #if (defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2))
            float viewZ = -i.fogCoord;
            float nearToFarZ = max(viewZ - _ProjectionParams.y, 0);
            half fogFactor = ComputeFogFactorZ0ToFar(nearToFarZ);
        #else
            half fogFactor = 0;
        #endif
    #else
        half fogFactor = i.fogCoord;
    #endif
    half4 finalColor = UniversalFragmentUnlit(inputData, color, alpha);
    
    #if defined(_SCREEN_SPACE_OCCLUSION) && !defined(_SURFACE_TYPE_TRANSPARENT)
        float2 normalizedScreenSpaceUV = GetNormalizedScreenSpaceUV(i.pos);
        AmbientOcclusionFactor aoFactor = GetScreenSpaceAmbientOcclusion(normalizedScreenSpaceUV);
        finalColor.rgb *= aoFactor.directAmbientOcclusion;
    #endif

    finalColor.rgb = MixFog(finalColor.rgb, fogFactor);
    return finalColor;
    }
#endif