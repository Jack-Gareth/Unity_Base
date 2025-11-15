Shader "Custom/SpriteWorldClip"
{
    Properties
    {
        _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        
        _ClipMinX ("Clip Min X", Float) = -10000
        _ClipMaxX ("Clip Max X", Float) = 10000
        _ClipMinY ("Clip Min Y", Float) = -10000
        _ClipMaxY ("Clip Max Y", Float) = 10000
        _UseClipping ("Use Clipping", Float) = 0
        _ClipAlpha ("Clip Alpha", Range(0,1)) = 0.3
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

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
                float3 worldPos : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            
            float _ClipMinX;
            float _ClipMaxX;
            float _ClipMinY;
            float _ClipMaxY;
            float _UseClipping;
            float _ClipAlpha;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color * _Color;
                
                if (_UseClipping > 0.5)
                {
                    if (i.worldPos.x >= _ClipMinX && i.worldPos.x <= _ClipMaxX && 
                        i.worldPos.y >= _ClipMinY && i.worldPos.y <= _ClipMaxY)
                    {
                        col.a *= _ClipAlpha;
                    }
                }
                
                return col;
            }
            ENDCG
        }
    }
    Fallback "Sprites/Default"
}
