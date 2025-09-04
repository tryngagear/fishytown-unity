Shader "Unlit/VolumetricFog"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Cull Off Zwrite Off ZTest Always
        Pass
        {
            HLSLPROGRAM
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile _  _MAIN_LIGHT_SHADOWS_CASCADE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            //#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Shadows.hlsl"
            struct appdata
            {
                real4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = TransformWorldToHClip(v.vertex);
                o.uv = v.uv;
                return o;
            }
            
            sampler2D _MainTex;
            float4 _MainTex_ST;
            real3 _SunDirection;

            real _Scattering=1;
            real _Steps=25;
            real _MaxDistance =75;

            half ShadowAtten(real3 worldPos){
                return MainLightRealtimeShadow(TransformWorldToShadowCoord(worldPos));
            }

            real3 GetWorldPos(real2 uv){
                #if UNITY_REVERSED_Z
                    real depth = SampleSceneDepth(uv);
                #else
                    real depth = lerp(UNITY_NEAR_CLIP_VALUE, 1, SampleSceneDepth(uv));
                #endif
                return ComputeWorldSpacePosition(uv, depth, UNITY_MATRIX_I_VP);
            }

            real ComputeScattering(real lightDotView){
                real result = 1.0f - _Scattering * _Scattering;
                result /= (4.0f * PI * pow(1.0f + _Scattering*_Scattering - (2.0f * _Scattering)* lightDotView, 1.5f));
                return result;
            }

            real random( real2 p ){
                return frac(sin(dot(p,real2(41,289)))*45758.5453)-0.5;
            }

            real random01( real2 p ){
                return frac(sin(dot(p,real2(41,289)))*45758.5453);
            }

            real invLerp(real from, real to, real val){
                return (val-from)/(to-from);
            }

            real remap(real origFrom, real origTo, real targetFrom, real targetTo, real val){
                real rel = invLerp(origFrom, origTo, val);
                return lerp(targetFrom, targetTo, rel);
            }

            real frag (v2f i) : SV_Target
            {
                real3 worldPos = GetWorldPos(i.uv);

                real3 startPos = _WorldSpaceCameraPos;
                real3 rayVector = worldPos - startPos;
                real3 rayDir = normalize(rayVector);
                real rayLen = length(rayVector);

                if(rayLen >_MaxDistance){
                    rayLen=_MaxDistance;
                    worldPos = startPos+rayDir*rayLen;
                }

                real stepLen = rayLen /_Steps;
                real3 step = rayDir * stepLen;
                real3 currPos = startPos;
                real accumFog = 0;

                for(real j = 0; j < _Steps-1; j++){
                    real shadowMapVal = ShadowAtten(currPos);

                    if(shadowMapVal>0){
                        real kernCol = ComputeScattering(dot(rayDir, _SunDirection));
                        accumFog += kernCol;
                    }
                    currPos += step;
                }

                accumFog /= _Steps;
                return accumFog;
            }
            ENDHLSL
        }
    }
}
