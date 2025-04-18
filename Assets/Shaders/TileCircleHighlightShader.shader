Shader "Custom/Particles/AlphaFade"
{
    Properties
    {
        _MainTex("Particle Texture", 2D) = "white" {}
        _TintColor("Tint Color", Color) = (1,1,1,1)
    }
        SubShader
        {
            Tags
            {
                "Queue" = "Transparent"
                "IgnoreProjector" = "True"
                "RenderType" = "Transparent"
                "PreviewType" = "Plane"
            }
            LOD 200

            // Standard alpha blending:
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Off
            Fog { Mode Off }
            Lighting Off

            Pass
            {
                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #include "UnityCG.cginc"

                sampler2D _MainTex;
                float4   _TintColor;

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv     : TEXCOORD0;
                    float4 color  : COLOR;
                };

                struct v2f
                {
                    float4 pos : SV_POSITION;
                    float2 uv  : TEXCOORD0;
                    fixed4 col : COLOR;
                };

                v2f vert(appdata v)
                {
                    v2f o;
                    o.pos = UnityObjectToClipPos(v.vertex);
                    o.uv = v.uv;
                    // combine particle color (from system) with your tint
                    o.col = v.color * _TintColor;
                    return o;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 tex = tex2D(_MainTex, i.uv);
                // multiply sampled alpha+RGB by the vertex color
                return tex * i.col;
            }
            ENDCG
        }
        }
}