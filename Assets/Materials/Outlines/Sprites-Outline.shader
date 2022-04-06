Shader "Unlit/InnerSpriteOutline HLSL"
{
    Properties
    {
        [PerRendererData] _MainTex ("Texture", 2D) = "white" {}
        [PerRendererData] _Color ("Tint", Color) = (1,1,1,1)
        _OutlineColor ("Outline", Color) = (1,1,1,1)
    }
    SubShader
    {
		Tags
		{
			"RenderType" = "Transparent"
			"Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
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
            float4 _MainTex_ST;
            float4 _MainTex_TexelSize;
 
			fixed4 _Color;
            fixed4 _OutlineColor;
 
            v2f vert (appdata_full v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.color = v.color;
                return o;
            }
 
            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                _Color = i.color;

                col.rgb *= _Color.rgb;

				fixed leftPixel = tex2D(_MainTex, i.uv + float2(-_MainTex_TexelSize.x, 0)).a;
				fixed upPixel = tex2D(_MainTex, i.uv + float2(0, _MainTex_TexelSize.y)).a;
				fixed rightPixel = tex2D(_MainTex, i.uv + float2(_MainTex_TexelSize.x, 0)).a;
				fixed bottomPixel = tex2D(_MainTex, i.uv + float2(0, -_MainTex_TexelSize.y)).a;
 
                fixed4 endcol;
                bool isOutline = false;
 
                if (col.a < 1 && (leftPixel == 1 || upPixel == 1 || rightPixel == 1 || bottomPixel == 1))
                    isOutline = true;
                    
                if (isOutline)
                    endcol = _OutlineColor;
                else
                    endcol = col;

                if (isOutline)
                    endcol.a = _OutlineColor.a * _Color.a;
                else if (col.a != 0)
                    endcol.a *= _Color.a;

                endcol.rgb *= endcol.a;

                return endcol;
            }
            ENDCG
        }
    }
}