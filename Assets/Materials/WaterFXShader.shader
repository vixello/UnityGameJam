Shader "BoatAttack/WaterFXShader_Visible"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [Enum(UnityEngine.Rendering.BlendMode)] _SrcBlend ("Src Blend mode", Float) = 5 // SrcAlpha
        [Enum(UnityEngine.Rendering.BlendMode)] _DstBlend ("Dst Blend mode", Float) = 10 // OneMinusSrcAlpha
        [Toggle] _Invert ("Invert?", Float) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" "RenderPipeline"="UniversalPipeline" }
        ZWrite Off
        Blend [_SrcBlend] [_DstBlend]
        LOD 100

        Pass
        {
            Name "WaterFX"
            Tags{"LightMode" = "WaterFX"}
            HLSLPROGRAM
            #pragma vertex WaterFXVertex
            #pragma fragment WaterFXFragment
            #pragma shader_feature _INVERT_ON

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float3 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                half4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float2 uv : TEXCOORD0;
                half4 normal : TEXCOORD1;
                half4 tangent : TEXCOORD2;
                half4 bitangent : TEXCOORD3;
                half4 color : TEXCOORD4;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;

            Varyings WaterFXVertex (Attributes input)
            {
                Varyings output = (Varyings)0;

                VertexPositionInputs vertexPosition = GetVertexPositionInputs(input.positionOS.xyz);
                VertexNormalInputs vertexTBN = GetVertexNormalInputs(input.normalOS, input.tangentOS);

                output.vertex = vertexPosition.positionCS;

                output.uv = input.uv;
                output.color = input.color;

                half3 viewDir = GetCameraPositionWS() - vertexPosition.positionWS;

                output.normal = half4(vertexTBN.normalWS, viewDir.x);
                output.tangent = half4(vertexTBN.tangentWS, viewDir.y);
                output.bitangent = half4(vertexTBN.bitangentWS, viewDir.z);

                return output;
            }

            half4 WaterFXFragment (Varyings input) : SV_Target
            {
                half4 col = tex2D(_MainTex, input.uv);

                // Foam mask from grayscale texture
                half foamMask = col.r * input.color.r;

                // Use foamMask as alpha for particles
                half alpha = saturate(foamMask);

                // Optional: make color fully white or blue-ish
                half3 color = half3(1,1,1); // white foam, or adjust for tint

                half4 outColor = half4(color, alpha);

                #ifdef _INVERT_ON
                outColor.rgb = 1 - outColor.rgb; // invert color if needed
                #endif

                // ensure alpha is not zero
                outColor.a = max(outColor.a, 0.01);

                return outColor;
            }

            ENDHLSL
        }
    }
}
