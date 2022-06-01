Shader "MiniMapMask"
{
   Properties
   {
      _MainTex ("Base (RGB)", RECT) = "white" {}
      _Mask ("Culling Mask", 2D) = "white" {}
   }
   SubShader
   {
      Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
					
			CGPROGRAM
			#pragma vertex vert_img
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest 
			#include "UnityCG.cginc"
			
			uniform sampler2D _MainTex;
			uniform sampler2D _Mask;
			
			float4 frag (v2f_img i) : COLOR
			{
				float4 org = tex2D(_MainTex, i.uv);
				float4 msk = tex2D(_Mask, i.uv);
				if(org.a > msk.a || org.a == 0) org.a = msk.a;
				return org;
			}
			ENDCG
		}
	}
	Fallback off
}