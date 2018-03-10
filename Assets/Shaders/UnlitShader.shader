Shader "Custom/MyShaders/MyUnlitShader"
{
	Properties
	{
		_MyFloat ("My float", float) = 143.0
		_MainTex ("Texture", 2D) = "white" {}
		_TintColor("tint", Color) = (0, 1, 0, 1)
		_Amplitude("Amplitude", float) = 1
		_Speed("Speed", float) = 1
	}
	SubShader
	{
		Tags {"RenderType"="Transparent" "Queue" = "Transparent" }
		LOD 100
		Zwrite Off
		Blend SrcAlpha OneMinusSrcAlpha



		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			// make fog work
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
				float4 vertex : SV_POSITION;
			};

			float4 _MyFloat;
			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _TintColor;
			float4 _Amplitude;
			float4 _Speed;
			
			v2f vert (appdata v)
			{
				v2f o;

				//v.vertex.x += sin(_Time.y *_Speed + v.vertex.y * _Amplitude); 
				v.vertex.x *= sin(_Time.y) * v.vertex.y; 
				v.vertex.y *= cos(_Time.x) * v.vertex.x;

				//v.vertex.y = sin(_Time.y *_Speed + v.vertex.y * _Amplitude); 

				o.vertex = UnityObjectToClipPos(v.vertex);

				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				//UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				// sample the texture
				fixed4 col = tex2D(_MainTex, i.uv) * _TintColor;
				// apply fog
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
				//return col * float4(1, 0, 0, 0);
			}
			ENDCG
		}
	}
}
