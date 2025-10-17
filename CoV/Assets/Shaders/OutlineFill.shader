Shader "Hidden/QuickOutline/Outline Fill"
{
    Properties
    {
        _OutlineColor("Color", Color) = (1,1,0,1)
        _OutlineWidth("Width", Float) = 2.0
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalRenderPipeline" "Queue"="Overlay" }
        Pass
        {
            Name "Outline"
            Cull Front
            ZWrite Off
            ZTest Always

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

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                float3 normalWS = TransformObjectToWorldNormal(IN.normalOS);
                float3 positionWS = TransformObjectToWorld(IN.positionOS.xyz);
                positionWS += normalWS * _OutlineWidth * 0.01;
                OUT.positionHCS = TransformWorldToHClip(positionWS);
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
