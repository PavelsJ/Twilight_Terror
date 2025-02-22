Shader "Custom/FOD_Mapping"
{
    Properties
    {
        _FogTexture ("Fog Texture", 2D) = "white" {}
        _FogColor ("Fog Color", Color) = (0.1, 0.1, 0.1, 0.7)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _FogTexture;
            fixed4 _FogColor;
            float2 _TextureSize;
            
            StructuredBuffer<float3> _Agents;
            StructuredBuffer<float> _Transparency;
            
            int _AgentCount;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float metaballFunction(float2 uv)
            {
                float intensity = 0.0;

                for (int j = 0; j < _AgentCount; j++)
                {
                    float2 agentPos = _Agents[j].xy;
                    float range = _Agents[j].z / _TextureSize.x;

                    float2 diff = uv - agentPos;
                    float sqrDist = dot(diff, diff);
                    float influence = range * range / (sqrDist + 0.0001); // Формула метаболов

                    intensity += influence;
                }

                return intensity;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 pixelCoord = floor(i.uv * _TextureSize) + 0.5;
                float2 snappedUV = pixelCoord / _TextureSize;

                fixed4 fogColor = tex2D(_FogTexture, snappedUV);
                float intensity = metaballFunction(snappedUV);

                float threshold = 1.0; // Чем меньше, тем больше соединение
                if (intensity > threshold)
                {
                    return fixed4(0, 0, 0, 0); // Полностью прозрачный
                }
                else if (intensity > threshold * 0.7)
                {
                    return fixed4(0, 0, 0, 0.8f); // Полупрозрачный
                }
                
                return lerp(fogColor, _FogColor, _FogColor.a);
            }
            ENDCG
        }
    }
}
