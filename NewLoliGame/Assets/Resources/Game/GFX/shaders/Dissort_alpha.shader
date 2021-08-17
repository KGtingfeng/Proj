Shader "Custom/Dissort_Alpha"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color",Color) = (1,1,1,1)
        _DistortTex("DistortTex",2D) = "White" {}
        _Speed("Speed (MainXY) (DisZW) ",vector) = (0,0,0,0)
        _Distort("Distort",Range(0,1)) = 0.5
    }
    SubShader
    {
        Tags {"Queue" = "Transparent"}
        Blend One OneMinusSrcAlpha
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
                fixed4 color : COLOR;
            };

            struct v2f
            {
                float2 uv1 : TEXCOORD0;
                float2 uv2 : TEXCOORD1;
                fixed4 vertexColor : COLOR;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex,_DistortTex;
            float4 _MainTex_ST,_DistortTex_ST,_Speed;
            fixed4 _Color;
            float _Distort;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv1 = TRANSFORM_TEX(v.uv, _MainTex)+_Speed.xy*_Time.y;
                o.uv2 = TRANSFORM_TEX(v.uv,_DistortTex)+_Speed.zw*_Time.y;
                o.vertexColor = v.color;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 vcol = i.vertexColor;
                fixed4 distex = tex2D(_DistortTex,i.uv2);
                fixed2 disuv = lerp(i.uv1,distex,_Distort);
                fixed4 col = tex2D(_MainTex,disuv) * vcol.a*_Color;

                return col;
            }
            ENDCG
        }
    }
}
