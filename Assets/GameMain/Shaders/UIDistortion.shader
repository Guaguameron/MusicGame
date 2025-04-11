Shader "Unlit/UIDistortion"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DistortionAmount ("Distortion Amount", Range(0, 100)) = 0
        _DistortionSpeed ("Distortion Speed", Range(0, 10)) = 1
        _NoiseAmount ("Noise Amount", Range(0, 1)) = 0
        _NoiseScale ("Noise Scale", Range(1, 500)) = 100
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        
        ZWrite Off
        ZTest Always
        Blend SrcAlpha OneMinusSrcAlpha

        GrabPass { "_GrabTexture" }

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
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 grabPos : TEXCOORD1;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            sampler2D _GrabTexture;
            float4 _MainTex_ST;
            float _DistortionAmount;
            float _DistortionSpeed;
            float _NoiseAmount;
            float _NoiseScale;

            // 随机函数
            float rand(float2 co)
            {
                return frac(sin(dot(co.xy, float2(12.9898, 78.233))) * 43758.5453);
            }

            // 噪声函数
            float noise(float2 uv)
            {
                float2 i = floor(uv);
                float2 f = frac(uv);
                
                float a = rand(i);
                float b = rand(i + float2(1.0, 0.0));
                float c = rand(i + float2(0.0, 1.0));
                float d = rand(i + float2(1.0, 1.0));

                float2 u = f * f * (3.0 - 2.0 * f);
                return lerp(a, b, u.x) + (c - a)* u.y * (1.0 - u.x) + (d - b) * u.x * u.y;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.grabPos = ComputeGrabScreenPos(o.vertex);
                o.color = v.color;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 grabUV = i.grabPos.xy / i.grabPos.w;
                
                // 获取原始图像
                fixed4 originalGrab = tex2D(_GrabTexture, grabUV);
                
                // 计算扭曲偏移
                float time = _Time.y * _DistortionSpeed;
                float2 distortion = float2(
                    sin(i.uv.y * 3 + time) * (_DistortionAmount * 0.001),
                    cos(i.uv.x * 3 + time) * (_DistortionAmount * 0.001)
                );
                
                // 应用扭曲
                float2 distortedUV = grabUV + distortion;
                fixed4 distortedGrab = tex2D(_GrabTexture, distortedUV);
                
                // 生成电视花屏噪声
                float2 noiseUV = i.uv * _NoiseScale + _Time.y * 50;
                float staticNoise = noise(noiseUV);
                fixed4 noiseColor = fixed4(staticNoise, staticNoise, staticNoise, 1);
                
                // 混合扭曲和花屏效果
                fixed4 finalColor = lerp(distortedGrab, noiseColor, _NoiseAmount);
                return finalColor;
            }
            ENDCG
        }
    }
}