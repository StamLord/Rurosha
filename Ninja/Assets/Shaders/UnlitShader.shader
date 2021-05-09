// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Unlit alpha-blended shader.
// - no lighting
// - no lightmap support
// - no per-material color

Shader "Unlit/CoolUI" {
Properties {
    _MainTex ("Main Texture", 2D) = "white" {}
    _Color1 ("Color1", Color) = (1,1,1,1) 
    _Color2 ("Color2", Color) = (1,1,1,1) 
    _LerpSpeed ("Lerp Speed", Float) = 1
    _Scroll ("Scroll", Vector) = (0,0,0,0)

    _NoiseTex ("Noise Texture", 2D) = "white" {}
    _NoiseSpread ("Noise Spread", Float) = 1
    
    _MaskTex ("Mask Texture", 2D) = "white" {}

    _FadeTex ("Fade Texture", 2D) = "white" {}
    _Fill ("Fill", Range(0,1)) = 1
}

SubShader {
    Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
    LOD 100

    ZWrite Off
    Blend SrcAlpha OneMinusSrcAlpha

    Pass {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                float2 fillTexcoord: TEXCOORD3;
                float2 texcoord : TEXCOORD0;
                float2 maskTexcoord : TEXCOORD1;
                float2 noiseTexcoord : TEXCOORD2;
                UNITY_FOG_COORDS(1)
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            sampler2D _NoiseTex;
            sampler2D _FadeTex;
            sampler2D _MaskTex;

            float4 _MainTex_ST;
            float4 _MaskTex_ST;
            float4 _NoiseTex_ST;

            float4 _Color1;
            float4 _Color2;
            float _LerpSpeed;
            float2 _Scroll;
            float _NoiseSpread;
            float _Fill;

            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.maskTexcoord = TRANSFORM_TEX(v.texcoord, _MaskTex);
                o.noiseTexcoord = TRANSFORM_TEX(v.texcoord, _MaskTex);
                o.vertex = UnityObjectToClipPos(v.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float offset = tex2D(_NoiseTex, i.noiseTexcoord);

                fixed4 col = tex2D(_MainTex, float2(
                    i.vertex.x / 500 + _Time[1] * _Scroll.x + offset / _NoiseSpread, 
                    i.vertex.y / 500 + _Time[1] * _Scroll.y) + offset / _NoiseSpread);

                col *= lerp(_Color1, _Color2, saturate(sin(_Time[1]) * _LerpSpeed));
                
                if(i.maskTexcoord.x > _Fill) discard;

                fixed4 maskCol = tex2D(_MaskTex, i.maskTexcoord);
                col.a = 1 - maskCol.r;

                
                // col.a -= tex2D(_FillMaskTex, i.fillTexcoord - fillOffset);

                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
        ENDCG
    }
}

}

