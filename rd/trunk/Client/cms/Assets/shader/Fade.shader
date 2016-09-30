Shader "Custom/Fade"
{
	Properties
	{
		_MainTex("Base (RGB)", 2D) = "white" {}
		_Float1("Float1",Float) = 1.0
		_FadeColor("FadeColor", Color) = (1.0, 1.0, 1.0, 1.0)
	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

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
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float _Float1;
			float4 _FadeColor;

			fixed4 frag (v2f i) : SV_Target
			{
				float4 outColor;
				outColor = tex2D(_MainTex, i.uv) * _Float1 + (1.0 - _Float1) * _FadeColor;
				return outColor;
			}
			ENDCG
		}
	}
}
