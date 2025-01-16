Shader "Custom/GameBoyDarkRedShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 texColor = tex2D(_MainTex, i.uv);

                // Retro palette colors
                fixed4 color1 = fixed4(0, 0, 0, 1);       // Black
                fixed4 color2 = fixed4(1, 0, 0, 1);       // Red
                fixed4 color3 = fixed4(0.4, 0, 0, 1);     // Dark Red

                float luminance = dot(texColor.rgb, fixed3(0.3, 0.59, 0.11));

                // Apply the palette based on luminance levels
                if (luminance < 0.33)
                    return color1;
                else if (luminance < 0.66)
                    return color2;
                else
                    return color3;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}

