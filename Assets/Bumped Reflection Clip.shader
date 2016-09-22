Shader "Example/Bumped Reflection Clip" 
  {
    Properties 
	{
     _MainTex ("Texture", 2D) = "white" {}
     _BumpMap ("Bumpmap", 2D) = "bump" {}
     _Cube ("Cubemap", CUBE) = "" {}
     _Value ("Reflection Power", Range(0,1)) = 0.5
	}
    
	SubShader 
	{
     Tags { "RenderType" = "Opaque" }
     Cull Off
     CGPROGRAM
      
     #pragma surface surf Lambert
     struct Input 
      {
       float2 uv_MainTex;
       float2 uv_BumpMap;
       float3 worldRefl;
       INTERNAL_DATA
      };
		  
     sampler2D _MainTex;
     sampler2D _BumpMap;
     samplerCUBE _Cube;
     float _Value;

     void surf (Input IN, inout SurfaceOutput o) 
      {
       float4 tex = tex2D (_MainTex, IN.uv_MainTex);
       clip (tex.a - 0.5);
       o.Albedo = tex.rgb;
       o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
       float4 refl = texCUBE (_Cube, WorldReflectionVector (IN, o.Normal));
       o.Emission = refl.rgb * _Value * refl.a;
      }
     ENDCG
    } 
  Fallback "Diffuse"
} 