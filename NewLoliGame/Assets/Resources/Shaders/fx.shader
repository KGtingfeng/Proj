Shader "Custom/fx"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color",Color) = (1,1,1,1)
        _SpeedX ("SpeedX",float) = 0
        _SpeedY ("SpeedY",float) = 0
    }
    SubShader
    {

        Tags {"Queue" = "Transparent"}
        Blend One One

        
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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _SpeedX,_SpeedY;
            fixed4 _Color;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex) + float2(_SpeedX,_SpeedY)*_Time.y;
                o.color = v.color;

                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 vcol = i.color;
                fixed4 col = tex2D(_MainTex, i.uv)*vcol.a*_Color.a;

                return col;
            }
            ENDCG
        }
    }
}
