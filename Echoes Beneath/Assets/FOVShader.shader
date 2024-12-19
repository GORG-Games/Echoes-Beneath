Shader "Custom/FOVShader"
{
    Properties
    {
        _Color("Color", Color) = (1, 1, 1, 0) // ����� ���� ��� �����
    }

        SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Overlay" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            Stencil
            {
                Ref 0          // ������������� �������� 1
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
                return _Color;
            }
            ENDHLSL
        }
    }
}