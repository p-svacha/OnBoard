Shader "Unlit/RadialGlow"
{
    Properties
    {
        _Color ("Glow Color", Color) = (1,1,1,1)
        _Intensity ("Intensity", Range(0,1)) = 0.5
        _MainTex ("Gradient Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 100

        // Only draw where stencil != 1
        Stencil
        {
            Ref 1
            Comp NotEqual
            Pass Keep
        }

        Cull Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            float _Intensity;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv  : TEXCOORD0;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the radial gradient texture (white-to-transparent circle)
                fixed4 grad = tex2D(_MainTex, i.uv);
                // Use the alpha channel of the gradient, modulated by intensity
                float a = grad.a * _Intensity;
                // Apply affinity color and computed alpha
                fixed4 col = _Color;
                col.a *= a;
                return col;
            }
            ENDCG
        }
    }
}
