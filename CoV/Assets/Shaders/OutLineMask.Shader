Shader "Hidden/QuickOutline/Outline Mask"
{
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        Pass
        {
            ZWrite On
            ColorMask 0
        }
    }
}
