Shader "Hidden/QuickOutline/URP_Outline"
{
    Properties
    {
        _OutlineColor ("Outline Color", Color) = (1,1,0,1)
        _OutlineWidth ("Outline Width", Range(0.0,10.0)) = 2.0
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalRenderPipeline" }
        Pass
        {
            Name "Outline"
            Cull Front
            ZWrite Off
            ZTest Less
            Blend SrcAlpha OneMinusSrcAlpha

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
            };

            CBUFFER_START(UnityPerMaterial)
                float4 _OutlineColor;
                float _OutlineWidth;
            CBUFFER_END

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                float3 normalWS = TransformObjectToWorldNormal(IN.normalOS);
                float3 posWS = TransformObjectToWorld(IN.positionOS.xyz);

                posWS += normalWS * _OutlineWidth * 0.01;
                OUT.positionHCS = TransformWorldToHClip(posWS);
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                return _OutlineColor;
            }
            ENDHLSL
        }
    }
}
