Shader "Unlit/UIDistortion"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DistortionAmount ("Distortion Amount", Range(0, 100)) = 10
        _DistortionSpeed ("Distortion Speed", Range(0, 10)) = 2
    }
    SubShader
    {
        Tags 
        { 
            "RenderType"="Transparent" 
            "Queue"="Transparent+100"
            "PreviewType"="Plane"
            "IgnoreProjector"="True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        ZTest Always
        Cull Off

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
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float _DistortionAmount;
            float _DistortionSpeed;
            
            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                o.color = v.color;
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv = i.uv;
                
                // 创建扭曲效果
                float2 center = float2(0.5, 0.5);
                float2 toCenter = center - uv;
                float dist = length(toCenter);
                
                float distortionStrength = _DistortionAmount * 0.01;
                float timeFactor = _Time.y * _DistortionSpeed;
                
                // 添加波动扭曲
                float2 distortion = normalize(toCenter) * sin(dist * 10 - timeFactor) * distortionStrength;
                uv += distortion;
                
                // 采样纹理
                fixed4 col = tex2D(_MainTex, uv);
                col.a = 0.5; // 设置半透明
                
                return col;
            }
            ENDCG
        }
    }
}