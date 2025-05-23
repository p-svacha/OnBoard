Shader "Custom/TokenShader"
{
    Properties
    {
        _MainTex("Base Texture",      2D) = "white" {}
        _PatternTex("Pattern Texture",   2D) = "white" {}
        _PatternAlpha("Pattern Alpha",     Range(0,1)) = 0.1
        _PatternScale("Pattern Scale",     Float) = 1.0
        _Color("Tint Color",        Color) = (1,1,1,1)
        _Scale("Triplanar Scale",   Float) = 1.0
        _BlendSharpness("Blend Sharpness",   Range(1,8)) = 2.0
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
            sampler2D _PatternTex;
            float    _PatternAlpha;
            float    _PatternScale;
            fixed4   _Color;
            float    _Scale;
            float    _BlendSharpness;

            struct Input {
                float3 worldPos;
                float3 worldNormal;
            };

            // take absolute world-normal, subtract small bias, raise to _BlendSharpness, then normalize
            inline float3 ComputeTriplanarWeights(float3 n)
            {
                float3 w = abs(n);
                // optional small bias to carve out the very edges:
                w = saturate(w - 0.2);
                // sharpen each channel
                w = pow(w, _BlendSharpness);
                // renormalize
                return w / (w.x + w.y + w.z + 1e-4);
            }

            void surf(Input IN, inout SurfaceOutputStandard o)
            {
                // transform to object-space
                float3 objPos = mul(unity_WorldToObject, float4(IN.worldPos,1)).xyz;
                float3 objNormal = normalize(mul((float3x3)unity_WorldToObject, IN.worldNormal));

                float3 w = ComputeTriplanarWeights(objNormal);

                // base texture triplanar
                float3 basePos = objPos * _Scale;
                fixed4 bx = tex2D(_MainTex, basePos.zy);
                fixed4 by = tex2D(_MainTex, basePos.xz);
                fixed4 bz = tex2D(_MainTex, basePos.xy);
                fixed4 baseCol = bx * w.x + by * w.y + bz * w.z;

                // pattern triplanar (alpha-masked)
                float3 patPos = objPos * _PatternScale;
                fixed4 px = tex2D(_PatternTex, patPos.zy);
                fixed4 py = tex2D(_PatternTex, patPos.xz);
                fixed4 pz = tex2D(_PatternTex, patPos.xy);

                float mask = px.a * w.x + py.a * w.y + pz.a * w.z;
                float3 patC = (px.rgb * w.x + py.rgb * w.y + pz.rgb * w.z);
                float blend = saturate(mask * _PatternAlpha);

                // overlay only where pattern alpha > 0
                baseCol.rgb = lerp(baseCol.rgb, baseCol.rgb * patC, blend);

                // final tint
                o.Albedo = baseCol.rgb * _Color.rgb;
                o.Alpha = baseCol.a * _Color.a;
            }
            ENDCG
        }

            FallBack "Diffuse"
}
