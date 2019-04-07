Shader "TecWolf/Saturation" {
	Properties {
		_MainTex ("Main (RGB)", 2D) = "" {}
		_RgbTex ("RGB (RGB)", 2D) = "" {}
		_Saturation("Saturation", Range(0, 1.5)) = 1
	}
	
	CGINCLUDE

	#include "UnityCG.cginc"
	
	struct v2f {
		float4 pos : SV_POSITION;
		half2 uv : TEXCOORD0;
	};
	
	sampler2D _MainTex;
	sampler2D _RgbTex;
	fixed _Saturation;

	half4 _MainTex_ST;
	
	v2f vert( appdata_img v ) 
	{
		v2f o;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv = UnityStereoScreenSpaceUVAdjust(v.texcoord.xy, _MainTex_ST);
		return o;
	} 
	
	fixed4 frag(v2f i) : SV_Target 
	{
		fixed4 color = tex2D(_MainTex, i.uv); 

		fixed lum = Luminance(color.rgb);
		color.rgb = lerp(fixed3(lum,lum,lum), color.rgb, _Saturation);
		return color;		
	}

	ENDCG 
	
Subshader {
 Pass {
	  ZTest Always Cull Off ZWrite Off

      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      ENDCG
  }
}

Fallback off
	
}
