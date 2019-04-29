 Shader "TecWolf/Chroma" {
 		Properties {
                _MainTex ("Base (RGB)", 2D) = "white" {}
                _MaskColor ("Mask Color", Color)  = (0.0, 1.0, 0.0, 1.0)
                _Sensitivity ("Threshold Sensitivity", Range(0, 1)) = 0.5
                _Smooth ("Smoothing", Range(0, 1)) = 0.5
        }
        SubShader {
                Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
                
				LOD 100
                ZTest Always Cull Back ZWrite On Lighting Off Fog { Mode off }
                
				CGPROGRAM
                
				#pragma surface surf Lambert alpha
 
 				struct Input {
                    float2 uv_MainTex;
                };
 
                sampler2D _MainTex;
                float4 _MaskColor;
                float _Sensitivity;
 				float _Smooth;
 
                void surf (Input IN, inout SurfaceOutput o) {
                        half4 c = tex2D (_MainTex, IN.uv_MainTex);
 
                        float maskY = 0.2989 * _MaskColor.r + 0.5866 * _MaskColor.g + 0.1145 * _MaskColor.b;
						float maskColorR = 0.7132 * (_MaskColor.r - maskY);
 						float maskColorB = 0.5647 * (_MaskColor.b - maskY);
 
						float y = 0.2989 * c.r + 0.5866 * c.g + 0.1145 * c.b;
 						float ColorR = 0.7132 * (c.r - y);
 						float ColorB = 0.5647 * (c.b - y);
 
 						float blendValue = smoothstep(_Sensitivity, _Sensitivity + _Smooth, distance(float2(ColorR, ColorB), float2(maskColorR, maskColorB)));

						o.Alpha = 1.0 * blendValue;
						o.Emission = c.rgb * blendValue;               
                }
                ENDCG
        }
        FallBack "Diffuse"	
}