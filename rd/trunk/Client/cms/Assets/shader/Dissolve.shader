Shader "Custom/Dissolve" {
	Properties{
		_Color("主颜色", Color) = (1,1,1,1)
		_MainTex("模型贴图", 2D) = "white" {}
		_DissolveText("溶解贴图", 2D) = "white" {}
		_Tile("溶解贴图的平铺大小", Range(0, 1)) = 1
		_Amount("溶解值", Range(0, 1)) = 0.5
		_DissSize("溶解大小", Range(0, 1)) = 0.1
		_DissColor("溶解主色", Color) = (1,1,1,1)
		_AddColor("叠加色，与主色叠加为开始色[R|G|B>0表示启用]", Color) = (1,1,1,1)
	}

	Category{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "DisableBatching" = "True" }
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask RGBA
		Cull Off
		Lighting Off
		ZWrite Off
		Fog{ Mode Off }
		
		SubShader{
		Pass{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"
		sampler2D _MainTex;
		sampler2D _DissolveText;
		fixed4 _Color; // 主色 
		half _Tile; // 平铺值 
		half _Amount; // 溶解度 
		half _DissSize; // 溶解范围 
		half4 _DissColor; // 溶解颜色 
		half4 _AddColor; // 叠加色 
		static half3 finalColor = float3(1,1,1); // 最终色 

		struct appdata_t {
			float4 vertex : POSITION;
			fixed4 color : COLOR;
			float2 texcoord : TEXCOORD0;
		};

		struct v2f {
			float4 vertex : SV_POSITION;
			fixed4 color : COLOR;
			float2 texcoord : TEXCOORD0;
			//UNITY_FOG_COORDS(1)
		};

		float4 _MainTex_ST;
		v2f vert(appdata_t v)
		{
			v2f o;
			o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
			o.color = v.color;
			o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
			//UNITY_TRANSFER_FOG(o,o.vertex);
			return o;
		}

		fixed4 frag(v2f i) : SV_Target
		{
			fixed4 col = 1.0;
			fixed4 tex = tex2D(_MainTex, i.texcoord);
			col.rgb = tex.rgb * _Color.rgb;
			float ClipTex = tex2D(_DissolveText, i.texcoord / _Tile).r;
			float ClipAmount = ClipTex - _Amount;
			col.a = _Color.a;
			if (_Amount > 0)
			{
				if (ClipAmount <= 0)
				{
					col.rgb = col.rgb * _AddColor;
					col.a = _AddColor.a;
				}
				else
				{
					if (ClipAmount < _DissSize)
					{
						if (_AddColor.x == 0)
							finalColor.x = _DissColor.x;
						else
							finalColor.x = ClipAmount / _DissSize;

						if (_AddColor.y == 0)
							finalColor.y = _DissColor.y;
						else
							finalColor.y = ClipAmount / _DissSize;

						if (_AddColor.z == 0)
							finalColor.z = _DissColor.z;
						else
							finalColor.z = ClipAmount / _DissSize;
						col.rgb = col.rgb * finalColor * 2;
					}
				}
			}
			return col;
		}
		ENDCG
		}
		}
	}
}
