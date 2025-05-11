Shader "Custom/TokenShader" {

    Properties{
        _MainTex("Texture", 2D) = "white" {}
        _Color("Tint Color", Color) = (1,1,1,1)
        _Scale("Scale", Float) = 1.0
    }

    SubShader{
        Tags { "RenderType" = "Opaque" }

        Stencil
        {
            Ref 1         // value to write
            Comp Always   // always pass
            Pass Replace  // write Ref into stencil
        }

        LOD 200

        CGPROGRAM
        // Use the Standard lighting model with shadows
        #pragma surface surf Standard fullforwardshadows

        sampler2D _MainTex;
        fixed4 _Color;
        float _Scale;

        struct Input {
            float3 worldPos;
            float3 worldNormal;
        };

        // Compute blending weights from the absolute world-space normal components.
        inline float3 ComputeTriplanarWeights(float3 normal) {
            // Take the absolute value of each component.
            float3 weights = abs(normal);
            // Optionally bias the weights for smoother blending.
            weights = saturate(weights - 0.2);
            // Normalize the weights so that they sum to 1.
            weights /= (weights.x + weights.y + weights.z + 0.0001);
            return weights;
        }

        void surf(Input IN, inout SurfaceOutputStandard o) {
            // Multiply the world-space position by the scale factor.
            float3 scaledPos = IN.worldPos * _Scale;

            // Compute blend weights based on the object's surface normal.
            float3 weights = ComputeTriplanarWeights(normalize(IN.worldNormal));

            // Sample the same texture along three different projections.
            fixed4 texSampleX = tex2D(_MainTex, scaledPos.zy); // Projection from the X-axis
            fixed4 texSampleY = tex2D(_MainTex, scaledPos.xz); // Projection from the Y-axis
            fixed4 texSampleZ = tex2D(_MainTex, scaledPos.xy); // Projection from the Z-axis

            // Blend the three samples using the computed weights.
            fixed4 blendedTex = texSampleX * weights.x +
                                texSampleY * weights.y +
                                texSampleZ * weights.z;

            // Apply tint color and assign the result to the surface output.
            o.Albedo = blendedTex.rgb * _Color.rgb;
            o.Alpha = blendedTex.a * _Color.a;
        }
        ENDCG
    }

    FallBack "Diffuse"
}