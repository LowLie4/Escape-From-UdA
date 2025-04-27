Shader "Custom/StencilMask"
{
    SubShader
    {
        // Que pinte *antes* que la ventana
        Tags { "Queue"="Geometry-1" }
        // No escribimos color
        ColorMask 0
        // No escribimos en Depth
        ZWrite Off
        // Ignoramos el Z‐buffer de la escena
        ZTest Always

        Stencil
        {
            Ref 1
            Comp Always
            Pass Replace
        }

        // Pase vacío: sólo interesa el stencil
        Pass { }
    }
}
