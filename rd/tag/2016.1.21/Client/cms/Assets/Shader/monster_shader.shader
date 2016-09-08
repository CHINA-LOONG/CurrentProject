Shader "monster/ambientOnly" {
	Properties {
		_Color ("Main Color", Color) = (.5,.5,.5,1)
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_AmbientRatio("Ambient ratio", Range (0, 1.0)) = 1.0
	}


	SubShader {
		Tags { "RenderType"="Opaque" }
		Pass {
			Name "BASE"
			Cull Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			uniform float _AmbientRatio; 

			struct appdata {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};
			
			struct v2f {
				float4 pos : SV_POSITION;
				float2 texcoord : TEXCOORD0;
				UNITY_FOG_COORDS(2)
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.pos = mul (UNITY_MATRIX_MVP, v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
				UNITY_TRANSFER_FOG(o,o.pos);
				return o;
			}

			fixed4 frag (v2f i) : SV_Target
			{
                float3 ambientLighting = UNITY_LIGHTMODEL_AMBIENT.xyz;
				fixed4 col = _Color * tex2D(_MainTex, i.texcoord);
				col.xyz = ambientLighting.xyz * _AmbientRatio + col.xyz;
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}
			ENDCG			
		}
	} 

	Fallback "VertexLit"
}
