Shader "Custom/DarknessStencilShader"
{
    Properties
    {
        _Color("Color", Color) = (0, 0, 0, 1)
        _MainTex("Texture", 2D) = "black" {}
    }

        SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Overlay" }

        Pass
        {
            Stencil
            {
                Ref 1
                Comp Always
                Pass Replace
            }

        // Базовые настройки для прозрачности
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off

        // Простой шейдер для отображения цвета
        HLSLPROGRAM
        #pragma vertex vert
        #pragma fragment frag

        sampler2D _MainTex;
        float4 _Color;

        struct VertexInput
        {
            float4 vertex : POSITION;
            float2 uv : TEXCOORD0;
        };

        struct VertexOutput
        {
            float4 pos : SV_POSITION;
            float2 uv : TEXCOORD0;
        };

        VertexOutput vert(VertexInput v)
        {
            VertexOutput o;
            o.pos = TransformObjectToHClip(v.vertex);
            o.uv = v.uv;
            return o;
        }

        float4 frag(VertexOutput i) : SV_Target
        {
            float4 texColor = tex2D(_MainTex, i.uv);
            return texColor * _Color;
        }
        ENDHLSL
    }
    }
}