
//*************************************
// 2D simple clouds shader.
//*************************************

Shader "AC/Time Of Day System Free/WIP/Simple Clouds"
{

	Properties
	{
		
	}


	SubShader
	{

		Tags { "Queue"="Geometry+2000" "RenderType"="Transparent" "IgnoreProjector"="True" }

		Cull Front

		ZWrite On 
		ZTest LEqual
		Blend SrcAlpha OneMinusSrcAlpha

 		Offset 2,2000

 		Fog{Mode Off}

		Pass
		{

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"


			uniform sampler2D _CloudsTex;
		
			uniform float2    _CloudsSize;
			uniform float2    _CloudsOffset;
			uniform float2    _CloudsSpeed;
			uniform half4     _CloudsColor;
			uniform float     _Sharpness;
			uniform half      _Density;
			uniform half      _CloudsShadow;
		
			//--------------------------------
		
			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;

			};

			struct v2f
			{

				float4 pos : SV_POSITION;
				float2 uv  : TEXCOORD0;
				//float3 worldPos : TEXCOORD1;


			};


			
			v2f vert (appdata v)
			{
				v2f o;

				o.pos = UnityObjectToClipPos(v.vertex);
				//o.worldPos = mul((float3x3)unity_ObjectToWorld,v.vertex).xyz;

				o.uv = (v.uv + _CloudsOffset) *_CloudsSize;

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{

				// Clouds.
				half4 clouds = tex2D( _CloudsTex, i.uv + _Time.xx * _CloudsSpeed); 

				half4 alpha = _Density *  pow(clouds, _Sharpness);

				half shadow = 1 / exp(alpha.r * _CloudsShadow);

				fixed4 color =  half4((_CloudsColor.rgb * shadow), saturate(alpha.r));



				// Gamma.
				//color = pow(color,1.0/2.2);
				//color = sqrt(color);


				return color;

			}
			ENDCG
		}
	}
}
