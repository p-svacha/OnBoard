Shader "Custom/TriplanarShader" {

    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _Color("Tint Color", Color) = (1,1,1,1)
        _Scale("Scale", Float) = 1.0
    }
        SubShader{
            Tags { "RenderType" = "Opaque" }
            Stencil {
                Ref 1
                Comp Always
                Pass Replace
            }
            LOD 200

            CGPROGRAM
            #pragma surface surf Standard fullforwardshadows

            sampler2D _MainTex;
            fixed4 _Color;
            float _Scale;

            struct Input {
                float3 worldPos;
                float3 worldNormal;
            };

            inline float3 ComputeTriplanarWeights(float3 normal) {
                float3 w = abs(normal);
                w = saturate(w - 0.2);
                w /= (w.x + w.y + w.z + 1e-4);
                return w;
            }

            void surf(Input IN, inout SurfaceOutputStandard o) {
                // 1) bring worldPos back into object space
                float3 objPos = mul(unity_WorldToObject, float4(IN.worldPos, 1)).xyz;

                // 2) likewise object-space normal so it rotates with the mesh
                float3 objNormal = normalize(mul((float3x3)unity_WorldToObject, IN.worldNormal));

                // 3) scale and triplanar‐blend in object space
                float3 scaledPos = objPos * _Scale;
                float3 w = ComputeTriplanarWeights(objNormal);

                fixed4 sx = tex2D(_MainTex, scaledPos.zy);
                fixed4 sy = tex2D(_MainTex, scaledPos.xz);
                fixed4 sz = tex2D(_MainTex, scaledPos.xy);

                fixed4 col = sx * w.x + sy * w.y + sz * w.z;

                o.Albedo = col.rgb * _Color.rgb;
                o.Alpha = col.a * _Color.a;
            }
            ENDCG
        }
    FallBack "Diffuse"
}