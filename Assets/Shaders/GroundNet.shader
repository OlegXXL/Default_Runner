Shader "Custom/GroundNet"
{
    Properties
    {
        _ColorA ("Color A", Color) = (0.6,0.6,0.6,1)    
        _ColorB ("Color B", Color) = (1,1,1,1)    
        _TilesPerUnit ("Tiles per unit", Float) = 2.0   
        _EdgeSoftness ("Edge softness", Range(0,0.5)) = 0.03
        _Gloss ("Smoothness", Range(0,1)) = 0.2
        _Metallic ("Metallic", Range(0,1)) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert
        #pragma target 3.0

        sampler2D _MainTex; 

        struct Input
        {
            float3 worldPos;
        };

        float4 _ColorA;
        float4 _ColorB;
        float _TilesPerUnit;
        float _EdgeSoftness;
        float _Gloss;
        float _Metallic;

        void vert(inout appdata_full v)
        {
            
        }

        void surf(Input IN, inout SurfaceOutputStandard o)
        {
            float2 uv = IN.worldPos.xz * _TilesPerUnit;

            float2 cell = floor(uv)+50;
            float parity = fmod(cell.x + cell.y, 2.0);

            float4 colCurrent = lerp(_ColorA, _ColorB, parity);
            float4 colOther   = lerp(_ColorA, _ColorB, 1.0 - parity);

            float2 uvFrac = frac(uv);
            float d = min(min(uvFrac.x, 1.0 - uvFrac.x), min(uvFrac.y, 1.0 - uvFrac.y));

            float edgeSmooth = smoothstep(0.0, _EdgeSoftness, d);

            float4 finalCol = lerp(colOther, colCurrent, edgeSmooth);

            o.Albedo = finalCol.rgb;
            o.Metallic = _Metallic;
            o.Smoothness = _Gloss;
            o.Alpha = finalCol.a;
        }
        ENDCG
    }

    FallBack "Diffuse"
}
