Shader "Customs/Effects"
{
	Properties
	{
		_MainColor("Main Color", Color) = (1, 1, 1, 1)
		_MainTex ("Main Texture", 2D) = "white" {}
		_MainIntentiy("Main Intensity", Range(0, 5)) = 1
		_SecondColor("Second Color", Color) = (1, 1, 1, 1)
		_SecondTex("Second Texture", 2D) = "white" {}
		_SecondIntentiy("Second Intensity", Range(0, 5)) = 1
		_SecTexUVAnimParas("SecTexUVAnimParas", Vector) = (0, 0, 0, 0)
		_RimColor("Rim Color", Color) = (1, 1, 1, 1)
		_RimIntensity("Rim Intersity", Range(0, 3)) = 1
		_UVAnimationParas("uv animation paras", Vector) = (0, 0, 1, 1)
		_MaskDissolveTexture("Mask(r) and Dissolve(g) Texture", 2D) = "white"{}
		_MaskPower("Mask Power", Range(0, 1)) = 1
		_MaskUVAnimation("Mask UV Animation", Vector) = (0, 0, 0, 0)
		_MaskRampTex("Dissolve Ramp Texture", 2D) = "white" {}
		_MaskRampColor("Dissolve Rmap Color", Color) = (1, 1, 1, 1)
		_DissolveColor("Dissolve Color", Color) = (1, 1, 1, 0.1)
		_DissolveMap("Dissolve Map", 2D) = "white" {}
		_DissolveThreshold("Dissolve Threshold", Range(0, 1)) = 1
		//_DissolveRange("Dissovle Range", Range(0, 1)) = 0.1
		_AirDistortionBump("Air Distortion Bump Map", 2D) = "bump"{}
		_AirDistortionBumpScale("Air Distortion Bump Scale", Range(-1, 1)) = 1
		_AirDistortionTint("Air Distortion Tint", Range(0, 1)) = 0

		_SecBlendMode("Second Blend Mode", float) = 0

		[HideInInspector] _Cull("__cull", int) = 0
		[HideInInspector] _ZWrite("__zwrite", int) = 0
		[HideInInspector] _Mode ("__mode", Float) = 0.0
		[HideInInspector] _SrcBlend ("__src", Float) = 1.0
        [HideInInspector] _DstBlend ("__dst", Float) = 0.0
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue" = "Transparent"}
		LOD 100

		Pass
		{
			
			Blend [_SrcBlend] [_DstBlend]
			ZWrite [_ZWrite]
			Cull [_Cull]
			//ZTest Off

			CGPROGRAM
			//#pragma target 3.0

			#pragma multi_compile __ _SECONDMAP_ENALBE
			#pragma multi_compile __ _RIM_ENABLE
			#pragma multi_compile __ _UV_ANIMATION
			#pragma multi_compile __ _MASK_ENABLE
			#pragma multi_compile __ _DISSOLVE_ENABLE
			#pragma multi_compile __ _MASK_RAMP_ENABLE
			#pragma multi_compile __ _AIR_DISTORTION_ENABLE

			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				half4 uv : TEXCOORD0;
				fixed4 color : COLOR;
				half3 normal : NORMAL;
			};

			struct v2f
			{
				half4 uv : TEXCOORD0;
				//half2 uv1 : TEXCOORD1;
				float4 vertex : SV_POSITION;
				fixed4 color : COLOR;
				#ifdef _RIM_ENABLE
					fixed rim : TEXCOORD2;
				#endif
				#ifdef _AIR_DISTORTION_ENABLE
					float4 uvgrab : TEXCOORD3;
				#endif
				#ifdef _MASK_ENABLE
					half4 maskUV : TEXCOORD4;
				#endif 
				#ifdef _SECONDMAP_ENALBE
					half4 secendMapUV : TEXCOORD5;
				#endif
			};

			fixed4 _MainColor;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			half _MainIntentiy;

			#ifdef _SECONDMAP_ENALBE
				fixed4 _SecondColor;
				sampler2D _SecondTex;
				half4 _SecondTex_ST;
				half4 _SecTexUVAnimParas;
				half _SecBlendMode;
				half _SecondIntentiy;
			#endif

			#ifdef _RIM_ENABLE
				fixed4 _RimColor;
				half _RimIntensity;
			#endif

			#ifdef _UV_ANIMATION
				half4 _UVAnimationParas;
			#endif

			#ifdef _MASK_ENABLE
				sampler2D _MaskDissolveTexture;
				half4 _MaskDissolveTexture_ST;
				half _MaskPower;
				half4 _MaskUVAnimation;
				#ifdef _MASK_RAMP_ENABLE
						sampler2D _MaskRampTex;
						half4 _MaskRampColor;
				#endif
			#endif

			#ifdef _DISSOLVE_ENABLE
				sampler2D _DissolveMap;
				half4 _DissolveMap_ST;
				fixed4 _DissolveColor;
				half _DissolveThreshold;
			#endif

			#ifdef _AIR_DISTORTION_ENABLE
				sampler2D _CustomGrabTexture;
				sampler2D _AirDistortionBump;
				half4 _AirDistortionBump_ST;
				half _AirDistortionBumpScale;
				fixed _AirDistortionTint;
			#endif
			
			
			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				half2 uv0 = v.uv;
				o.uv.xy = TRANSFORM_TEX(uv0, _MainTex);

				#ifdef _SECONDMAP_ENALBE
					o.secendMapUV.xy = TRANSFORM_TEX(uv0, _SecondTex);
					o.secendMapUV.zw = 0;
					o.secendMapUV.xy += _SecTexUVAnimParas.xy *  _Time.x;
				#endif

				#ifdef _DISSOLVE_ENABLE
					o.uv.zw = TRANSFORM_TEX(v.uv, _DissolveMap);
				#else
					o.uv.zw = o.uv.xy;
				#endif
				#ifdef _MASK_ENABLE
					o.maskUV.xy = TRANSFORM_TEX(uv0, _MaskDissolveTexture);
				#endif
				o.color = v.color;
				#ifdef _RIM_ENABLE
					fixed3 worldNormal = normalize(mul(half4(v.normal, 0), unity_WorldToObject).xyz);
					half4 worldPos = mul( unity_ObjectToWorld, v.vertex );
					fixed3 viewDir = normalize( _WorldSpaceCameraPos.xyz - worldPos.xyz);
					o.rim   = 1.0 - saturate( dot(viewDir,worldNormal) );
				#endif
				#ifdef _AIR_DISTORTION_ENABLE
					#if UNITY_UV_STARTS_AT_TOP
						float scale = -1.0;
					#else
						float scale = 1.0;
					#endif
					o.uvgrab.xy = (float2(o.vertex.x, o.vertex.y * scale) + o.vertex.w) * 0.5;
					o.uvgrab.zw = o.vertex.zw;
				#endif
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				#ifdef _UV_ANIMATION
					i.uv.xy  += _UVAnimationParas.xy * _Time.x;
					i.uv.zw  += _UVAnimationParas.zw * _Time.x;
				#endif
				half4 col = tex2D(_MainTex, i.uv.xy) * _MainColor;
				col.rgb *= _MainIntentiy;

				#ifdef _SECONDMAP_ENALBE
					fixed4 secCol = tex2D(_SecondTex, i.secendMapUV.xy) * _SecondColor;
					secCol.rgb *= _SecondIntentiy;
					//if(_SecBlendMode == 0) //blend
					//{
						col.rgb = secCol.rgb * secCol.a + col.rgb * (1 - secCol.a * (1 - _SecBlendMode));//col.rgb * col.a + secCol.rgb * (1 - col.a);
					//	//col.a = secCol.a;
					//}
					//if(_SecBlendMode == 1) //add
					//{
					//	col.rgb = secCol.rgb * secCol.a + col.rgb;
						
					//}
					//col.rgb = col.rgb * col.a + secCol.rgb * secCol.a;
					//col.a += secCol.a;
				#endif

				col *= i.color;
				#ifdef _DISSOLVE_ENABLE
					fixed4 dissCol = tex2D(_DissolveMap, i.uv.zw);
					half disC0 = dissCol.r - _DissolveThreshold;
					fixed disRange = saturate(disC0);
					fixed disColorIntensity = saturate(1.0 - disRange / _DissolveColor.a);
					col.rgb += disColorIntensity * _DissolveColor.rgb * 5;
					if(abs(_DissolveThreshold - dissCol.r))
					clip(disC0);
				#endif
				
				#ifdef _MASK_ENABLE
					i.maskUV.xy += _MaskUVAnimation.xy * _Time.x;
					half3 maskDiss = tex2D(_MaskDissolveTexture, i.maskUV.xy).rgb;
					#ifdef _MASK_RAMP_ENABLE
						col *= tex2D(_MaskRampTex, maskDiss.rg) * _MaskRampColor;
					#else
						col *= maskDiss.r * _MaskPower;
					#endif
				#endif

				

				#ifdef _RIM_ENABLE
					col.rgb += i.rim * i.rim * _RimColor.rgb * _RimIntensity;
				#endif

				#ifdef _AIR_DISTORTION_ENABLE
					half4 airDistortionBump = tex2D(_AirDistortionBump, i.uv.zw);
					airDistortionBump.x *= airDistortionBump.w;
					//airDistortionBump.xy = airDistortionBump.xy *2 - 1;
					//#ifdef _MASK_ENABLE
					//	col.a *= maskDiss.r * _MaskPower;
					//#endif
					float2 bumpOffset = airDistortionBump.xy * _AirDistortionBumpScale;// * sin(_Time.y * 4.0);
					i.uvgrab.xy = bumpOffset * i.uvgrab.z + i.uvgrab.xy;
					half4 backgroudCol = tex2Dproj (_CustomGrabTexture, UNITY_PROJ_COORD(i.uvgrab));
					//col.rgb = lerp(col.rgb, backgroudCol.rgb, _AirDistortionTint);
					col.rgb = col.rgb * col.a + backgroudCol.rgb * (1 - col.a);
					col.a = 1;
				#endif

				return col;
			}
			ENDCG
		}
	}

	CustomEditor "EffectsGUI"
}
