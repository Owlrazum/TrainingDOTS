// Was inspired by code from: https://twitter.com/AlexStrook/status/1294735487513645058

Shader "Skybox Gradient"
{
	Properties
	{
		top("Top", Color) = (1,1,1,0)
		bottom("Bottom", Color) = (0,0,0,0)
		factor("Factor", Float) = 1
		power("Power", Float) = 1
	}
	
	SubShader
	{
		Tags { "RenderType"="Opaque" }
		LOD 100

		CGINCLUDE
		#pragma target 3.0
		ENDCG
		Blend Off
		Cull Back
		ColorMask RGBA
		ZWrite On
		ZTest LEqual
		Offset 0 , 0
		
		Pass
		{
			Name "Unlit"
			Tags { "LightMode"="ForwardBase" }
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_instancing
			#include "UnityCG.cginc"

			struct mesh
			{
				float4 pos : POSITION;
				float2 uv : TEXCOORD;
			};
			
			struct surface
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD;
			};

			uniform float4 bottom;
			uniform float4 top;
			uniform float factor;
			uniform float power;
			
			surface vert ( mesh m )
			{
				surface s;
				s.pos = UnityObjectToClipPos(m.pos);
			    s.uv = m.uv;
				return s;
			}
			
			fixed4 frag (surface surface ) : SV_Target
			{
				fixed4 finalColor = lerp(bottom, top, pow(saturate((surface.uv.y * factor)), power ));
				return finalColor;
			}
			ENDCG
		}
	}
}