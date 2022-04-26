Shader "Sprites/ColorSwap"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _SwapTex("Color Data", 2D) = "transparent" {}
        _Color("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap("Pixel snap", Float) = 0
    }
        SubShader
        {
            Tags
            {
                "RenderType" = "Transparent"
                "Queue" = "Transparent"
                "IgnoreProjector" = "True"
                "RenderType" = "Transparent"
                "PreviewType" = "Plane"
                "CanUseSpriteAtlas" = "True"
            }

            Blend One OneMinusSrcAlpha
            Cull Off
            ZTest Off
            ZWrite Off



            Pass
            {

                CGPROGRAM
                #pragma vertex vert
                #pragma fragment frag

                #include "UnityCG.cginc"

                struct appdata
                {
                    float4 vertex : POSITION;
                    float2 uv : TEXCOORD0;
                };

                struct v2f
                {
                    float2 uv : TEXCOORD0;
                    float4 vertex : SV_POSITION;
                    float4 color : COLOR;
                };

                sampler2D _MainTex;
                sampler2D _AlphaTex;
                float _AlphaSplitEnabled;

                sampler2D _SwapTex;
                float4 _MainTex_ST;
                float4 _MainTex_TexelSize;

                fixed4 _Color;

                v2f vert(appdata_full v)
                {
                    v2f o;
                    o.vertex = UnityObjectToClipPos(v.vertex);
                    o.uv = v.texcoord;
                    o.color = v.color;
                    return o;
                }

                fixed4 SampleSpriteTexture(float2 uv)
                {
                    fixed4 color = tex2D(_MainTex, uv);
                    if (_AlphaSplitEnabled)
                        color.a = tex2D(_AlphaTex, uv).r;

                    return color;
                }

                fixed4 frag(v2f i) : SV_Target
                {
                    fixed4 c = SampleSpriteTexture(i.uv);
                    fixed4 swapCol = tex2D(_SwapTex, float2(c.r, 0));
                    fixed4 final = lerp(c, swapCol, swapCol.a) * i.color;
                    final.a = c.a;
                    final.rgb *= c.a;
                    return final;
                }
                ENDCG
            }
        }
}