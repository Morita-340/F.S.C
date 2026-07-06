Shader "Custom/OutlineMask"
{
    Properties
    {
        _MaskColor ("Mask Color", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest Always
        Blend One OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing

            #include "UnityCG.cginc"
            #include "UnitySprites.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct OutlineV2F
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            fixed4 _MaskColor;

            OutlineV2F vert(appdata v)
            {
                OutlineV2F o;

                UNITY_SETUP_INSTANCE_ID(v);

                float3 flipped = UnityFlipSprite(v.vertex.xyz, _Flip);

                o.pos = UnityObjectToClipPos(float4(flipped, 1.0));
                o.uv = v.uv;
                o.color = v.color;

                return o;
            }

            fixed4 frag(OutlineV2F i) : SV_Target
            {
                fixed spriteAlpha = tex2D(_MainTex, i.uv).a * i.color.a;
                fixed alpha = spriteAlpha * _MaskColor.a;

                return fixed4(_MaskColor.rgb * alpha, alpha);
            }
            ENDCG
        }
    }
}