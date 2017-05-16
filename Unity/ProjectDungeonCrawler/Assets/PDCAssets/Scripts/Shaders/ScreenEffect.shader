Shader "Mapje/ScreenEffect"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}

		_BumpMap("Noise text", 2D) = "white" {}
		_Magnitude("Magnitude", Range(0,1)) = 0.05
	}
		SubShader
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Opaque" }
		ZWrite On Lighting Off Cull Off Fog{ Mode Off } Blend One Zero

		Pass
	{
		CGPROGRAM
		#pragma vertex vert
		#pragma fragment frag

		#include "UnityCG.cginc"

		sampler2D _MainTex;

		sampler2D _BumpMap;
		float  _Magnitude;

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


		v2f vert(appdata v)
		{
			v2f o;
			o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);

			o.uv = v.uv;
			return o;
		}

		fixed4 frag(v2f i) : SV_Target
		{
			float2 disp = tex2D(_BumpMap, i.uv).xy;
			disp = ((disp * 2) - 1) * _Magnitude * sin(_Time);

			float4 col = tex2D(_MainTex, i.uv + disp);
			return col;
		}
			ENDCG
		}
	}
}