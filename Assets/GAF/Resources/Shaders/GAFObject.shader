Shader "GAF/GAFObject"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_ColorMult("ColorMult",Color) = (1.0, 1.0, 1.0, 1.0 )
		_ColorShift("ColorShift",Color) = (0.0, 0.0, 0.0, 0.0 )
		_Alpha ("Alpha factor", Range(0.0,1.0)) = 1.0
	}
	
	SubShader 
	{
		Tags 
		{
			"Queue"="Transparent"
			"IgnoreProjector"="True"
			"RenderType"="Transparent"
		}
		
		Cull Off
		Zwrite Off
		Lighting Off
		Blend SrcAlpha OneMinusSrcAlpha
		
		CGPROGRAM

		#pragma surface surf Unlit noambient
		
		#include "UnityCG.cginc"
		
		sampler2D _MainTex;
		
		float3 _ColorMult;	
		float4 _ColorShift;
		
		float _Alpha;
		
		struct Input
		{
			float2 uv_MainTex;
		};
		
		fixed4 LightingUnlit(SurfaceOutput s, half3 lightDir, half3 viewDir, half atten)
		{
			fixed4 c;
	        c.rgb = s.Albedo; 
	        c.a = s.Alpha;
	        return c;
		}
		
		void surf (Input input, inout SurfaceOutput o)
		{			
			half4 mainColor	= tex2D(_MainTex, input.uv_MainTex );
			
			o.Albedo = ( mainColor.rgb * _ColorMult.rgb + _ColorShift.rgb );
			o.Alpha  = ( mainColor.a   * _Alpha         + _ColorShift.a   );
		}
		
		ENDCG
	}
	
	FallBack "Mobile/Particles/Aplha Blended"
}
