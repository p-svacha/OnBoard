Shader "Custom/TokenShader"
{
    Properties
    {
        _MainTex("Base Texture",        2D) = "white" {}
        _Color("Tint Color",            Color) = (1,1,1,1)
        _Scale("Triplanar Scale",       Float) = 1.0
        _BlendSharpness("Blend Sharpness", Range(1,8)) = 2.0
        _Rotation("Texture Rotation",   Range(0,360)) = 0.0
    }

        SubShader
        {
            Tags { "RenderType" = "Opaque" }
            Stencil
            {
                Ref 1
                Comp Always
                Pass Replace
            }
            LOD 200

            CGPROGRAM
            #pragma surface surf Standard fullforwardshadows

            sampler2D _MainTex;
            fixed4   _Color;
            float    _Scale;
            float    _BlendSharpness;
            float    _Rotation;

            struct Input {
                float3 worldPos;
                float3 worldNormal;
            };

            // carve off edges, sharpen, then normalize
            inline float3 ComputeTriplanarWeights(float3 n)
            {
                float3 w = abs(n);
                w = saturate(w - 0.2);               // bias
                w = pow(w, _BlendSharpness);        // sharpen
                return w / (w.x + w.y + w.z + 1e-4);
            }

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                // bring into object‐space
                float3 objPos = mul(unity_WorldToObject, float4(IN.worldPos,1)).xyz;
                float3 objNormal = normalize(mul((float3x3)unity_WorldToObject, IN.worldNormal));

                // weights for X/Y/Z projections
                float3 w = ComputeTriplanarWeights(objNormal);

                // build rotation matrix once
                float ang = _Rotation * UNITY_PI / 180.0;
                float ca = cos(ang);
                float sa = sin(ang);
                float2x2 R = float2x2(ca, -sa,
                                       sa,  ca);

                // basePos = objPos * scale
                float3 basePos = objPos * _Scale;

                // project + rotate each plane's UV
                float2 uvX = mul(R, basePos.zy); // X‐axis projection uses (Z,Y)
                float2 uvY = mul(R, basePos.xz); // Y‐axis projection uses (X,Z)
                float2 uvZ = mul(R, basePos.xy); // Z‐axis projection uses (X,Y)

                // sample
                fixed4 sx = tex2D(_MainTex, uvX);
                fixed4 sy = tex2D(_MainTex, uvY);
                fixed4 sz = tex2D(_MainTex, uvZ);

                // blend by weights
                fixed4 col = sx * w.x + sy * w.y + sz * w.z;

                // tint out
                o.Albedo = col.rgb * _Color.rgb;
                o.Alpha = col.a * _Color.a;
            }
            ENDCG
        }

            FallBack "Diffuse"
}
