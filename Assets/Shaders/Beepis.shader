Shader "Unlit/Beepis"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _TintColor("Tint Color", Color) = (255,1,1,1)
        _Transparency("Transparency", Range(0.0, 0.5)) = 0.25
        _CutoutThresh("CutoutThreshold", Range(0.1, 1.0)) = 0.2
        _Dist("Distance", float) = 1
        _Amp("Amplitude", float) = 1
        _Spd("Speed", float) = 1
        _Amount("Amount", float) = 1
    }
    SubShader
    {
        Tags {"Queue" = "Transparent" "RenderType"="Transparent" }
        LOD 100

        ZWrite off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            //#pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                //UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _TintColor;
            float _Transparency;
            float _CutoutThresh;
            float _Dist;
            float _Spd;
            float _Amount;
            float _Amp;

            v2f vert (appdata v)
            {
                v2f o;
                v.vertex.x += sin(_Time.y * _Spd * v.vertex.y * _Amp) * _Dist * _Amount;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                //UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv) + _TintColor;
                col.a = _Transparency;
                clip(col.r - _CutoutThresh); //discard is a keyword
                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
