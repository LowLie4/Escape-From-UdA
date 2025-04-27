Shader "Custom/WindowStencilReveal"
{
    Properties
    {
        _MainTex ("Imagen Revelada", 2D) = "white" {}
    }
    SubShader
    {
        // 1) Revelado donde stencil == 1
        Pass
        {
            Tags { "Queue" = "Geometry+1" }
            ZWrite Off

            Stencil
            {
                Ref 1
                Comp Equal
                Pass Keep
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragReveal
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; float2 uv : TEXCOORD0; };
            struct v2f    { float4 pos : SV_POSITION; float2 uv  : TEXCOORD0; };

            sampler2D _MainTex;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv  = v.uv;
                return o;
            }

            fixed4 fragReveal(v2f i) : SV_Target
            {
                return tex2D(_MainTex, i.uv);
            }
            ENDCG
        }

        // 2) Negro donde stencil != 1
        Pass
        {
            Tags { "Queue" = "Geometry+1" }
            ZWrite Off

            Stencil
            {
                Ref 1
                Comp NotEqual
                Pass Keep
            }

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment fragBlack
            #include "UnityCG.cginc"

            struct appdata { float4 vertex : POSITION; };
            struct v2f    { float4 pos : SV_POSITION; };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 fragBlack() : SV_Target
            {
                return fixed4(0,0,0,1);
            }
            ENDCG
        }
    }
}
