Shader "Custom/FOVShader"
{
    Properties
    {
        _Color("Color", Color) = (0, 0, 0, 0) // ����� ���� ��� �����
    }

        SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Overlay-1" }
        Pass
        {
            Cull Off
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            Stencil
            {
                Ref 1          // ������������� �������� 1
                Comp Always    // ������ ���������� Stencil
                Pass Replace   // �������� �������� � Stencil ������ �� 1
            }

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
            };

            float4 _Color;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = TransformObjectToHClip(v.vertex);
                return o;
            }

            float4 frag(v2f i) : SV_Target
            {
                return float4(0, 0, 0, 0); // ��������� ���������� ����
            }
            ENDHLSL
        }
    }
}