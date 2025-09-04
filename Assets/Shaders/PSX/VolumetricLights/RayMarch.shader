Shader "Unlit/RayMarch"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            HLSLPROGRAM
            #pragma shader_feature_local_fragment _
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #define MAX_STEPS 100
            #define MAX_DIST 100
            #define SURF_DIST 1e-3
            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 ro : TEXCOORD1;
                float3 hitPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.ro = float4(_WorldSpaceCameraPos,1);
                o.hitPos = mul(unity_ObjectToWorld,v.vertex);
                return o;
            }
            float GetDist(float3 p){
                float d = length(p)-.5;
                d = length(float2(length(p.xy)-.5, p.z))-.1;
                //float3 offset = abs(p)-.5;
                //float unsignDst = length(max(offset,0));
                //float dstIB = min(offset,0);
                //d = unsignDst + dstIB;
                return d;
            }
            float3 GetNormal(float3 p){
                float2 e = float2(1e-2,0);
                float3 n = GetDist(p)-float3(
                    GetDist(p-e.xyy),
                    GetDist(p-e.yxy),
                    GetDist(p-e.yyx)
                );

                return normalize(n);
            }
            float VoxelMarch(){
                return 0;
            }
            float beersLaw(float dist){
                return exp(-dist*300);
            }
            float RayMarch(float3 ro, float3 rd){
                float distO = 0;
                float distS;
                for(int i=0; i<MAX_STEPS; i++){
                    float3 p = ro + distO*rd;
                    distS = GetDist(p);
                    distO += distS;
                    if(distS<SURF_DIST|| distO>MAX_DIST) break;
                }
                return distO;
            }
            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv-.5;
                float3 ro = i.ro;
                float3 rd = normalize(i.hitPos - ro);
                float d = RayMarch(ro, rd);
                fixed4 col = 0;
                //if(d<MAX_DIST){
                float3 p = ro+rd*d;
                float3 n = GetNormal(p);
                col.rgb = n;
                //}else{
                    //discard;
                //}
                float4 b = float4(1,1,1,0.5)*beersLaw(d);
                return col+b;
            }
            ENDHLSL
        }
    }
}
