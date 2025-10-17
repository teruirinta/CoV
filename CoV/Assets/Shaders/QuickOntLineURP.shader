Shader "Universal Render Pipeline/QuickOutlineURP"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (1,1,0,1)
        _OutlineWidth ("Outline Width", Range(0.0, 0.05)) = 0.02
        _MainTex ("Main Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        LOD 100

        Pass
        {
            Name "Outline"
            Tags { "LightMode" = "UniversalForward" }

            Cull Front
            ZWrite Off
            ZTest LEqual

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            float _OutlineWidth;
            float4 _OutlineColor;

            Varyings vert (Attributes input)
            {
                Varyings output;
                float3 normalDir = normalize(input.normalOS);
                float3 offset = normalDir * _OutlineWidth;
                float4 positionWS = TransformObjectToHClip(input.positionOS + offset);
                output.positionHCS = positionWS;
                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }

        Pass
        {
            Name "Base"
            Tags { "LightMode" = "UniversalForward" }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            Varyings vert (Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS);
                output.uv = input.uv;
                return output;
            }

            half4 frag (Varyings input) : SV_Target
            {
                return SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv);
            }
            ENDHLSL
        }
    }

    FallBack Off
}
