Shader "Custom/GameBoyColorDither"
{
    Properties
    {
        _MainColor1 ("Color 1", Color) = (0.25, 0.39, 0.31, 1)
        _MainColor2 ("Color 2", Color) = (0.44, 0.53, 0.50, 1)
        _MainColor3 ("Color 3", Color) = (0.72, 0.75, 0.53, 1)
        _DitherTexture ("Dither Pattern", 2D) = "white" {}
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

            // Properties
            fixed4 _MainColor1;
            fixed4 _MainColor2;
            fixed4 _MainColor3;
            sampler2D _DitherTexture;

            // Structs
            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 worldNormal : TEXCOORD1;
                float4 screenPos : TEXCOORD2;
            };

            // Vertex Shader
            v2f vert (appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.screenPos = ComputeScreenPos(o.pos);
                return o;
            }

            // Dithering Function
            float Dither(float2 uv, float brightness)
            {
                // Dithering pattern lookup
                float ditherValue = tex2D(_DitherTexture, uv * 1).r;  // Scale down for subtle dithering
                return brightness + ditherValue - 0.5;
            }

            // Fragment Shader
fixed4 frag(v2f i) : SV_Target
{
    // Basic lighting
    float3 normal = normalize(i.worldNormal);
    float brightness = saturate(dot(normal, float3(0, 1, 0)));

    // Apply dithering
    float ditheredValue = Dither(i.screenPos.xy * _ScreenParams.xy / 4, brightness);

    // Color selection based on brightness
    fixed4 color;
    if (ditheredValue < 0.40)  // Dark
        color = _MainColor1;
    else if (ditheredValue < 0.80)  // Midtone
        color = _MainColor2;
    else  // Highlight
        color = _MainColor3;

    return color;
}
            ENDCG
        }
    }

    FallBack "Diffuse"
}


