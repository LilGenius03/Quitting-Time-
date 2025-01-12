Shader "Custom/RetroShader"
{
    Properties
    {
        _MainColor ("Main Color", Color) = (1, 0, 0, 1) // Red tint
        _DitherTexture ("Dither Texture", 2D) = "white" {} // Bayer matrix
        _EdgeThreshold ("Edge Threshold", Range(0, 1)) = 0.5
        _DitherSize ("DitherSize", Range(0, 100)) = 10
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

            // Shader properties
            fixed4 _MainColor;
            sampler2D _DitherTexture;
            float _EdgeThreshold;
            float _DitherSize;

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
                o.uv = v.uv;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // Sample the dither texture
                fixed ditherValue = tex2D(_DitherTexture, i.uv * _DitherSize).r;

                // Compare dither value to threshold
                if (ditherValue < _EdgeThreshold)
                    return _MainColor;
                else
                    return fixed4(0, 0, 0, 1); // Black

            }
            ENDCG
        }
    }
}
