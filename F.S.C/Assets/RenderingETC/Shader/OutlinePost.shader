Shader "Custom/OutlinePost"
{
    Properties
    {
        _MainTex ("Mask Texture", 2D) = "white" {}
        _Thickness ("Thickness", Float) = 1
        _Alpha ("Alpha", Range(0,1)) = 1
        _TexelSize ("Texel Size", Vector) = (0.001,0.001,0,0)
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
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float _Thickness;
            float _Alpha;
            float4 _TexelSize;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                fixed4 color : COLOR;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 center = tex2D(_MainTex, i.uv);

                float2 step = _TexelSize.xy * _Thickness;

                fixed4 best = fixed4(0, 0, 0, 0);

                fixed4 s;

                s = tex2D(_MainTex, i.uv + float2( step.x, 0)); best = s.a > best.a ? s : best;
                s = tex2D(_MainTex, i.uv + float2(-step.x, 0)); best = s.a > best.a ? s : best;
                s = tex2D(_MainTex, i.uv + float2(0,  step.y)); best = s.a > best.a ? s : best;
                s = tex2D(_MainTex, i.uv + float2(0, -step.y)); best = s.a > best.a ? s : best;

                s = tex2D(_MainTex, i.uv + float2( step.x,  step.y)); best = s.a > best.a ? s : best;
                s = tex2D(_MainTex, i.uv + float2(-step.x,  step.y)); best = s.a > best.a ? s : best;
                s = tex2D(_MainTex, i.uv + float2( step.x, -step.y)); best = s.a > best.a ? s : best;
                s = tex2D(_MainTex, i.uv + float2(-step.x, -step.y)); best = s.a > best.a ? s : best;

                float outline = saturate(best.a - center.a);

                fixed3 color = best.rgb / max(best.a, 0.0001);

                fixed4 col;
                col.rgb = color;
                col.a = outline * _Alpha * i.color.a;

                return col;
            }
            ENDCG
        }
    }
}