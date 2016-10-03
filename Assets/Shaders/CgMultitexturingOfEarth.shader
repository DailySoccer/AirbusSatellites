// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Cg multitexturing of Earth" {
   Properties {
   	  _BumpMap ("Normal Map", 2D) = "bump" {}
   	  _SpecText ("Specular Earth", 2D) = "white" {}
   	  _Shininess ("Shininess", range(0, 1000)) = 0
   	  _Intensity ("Intensity", range(0, 3)) = 0
      _DecalTex ("Daytime Earth", 2D) = "white" {}
      _MainTex ("Nighttime Earth", 2D) = "white" {} 
      _Color ("Nighttime Color Filter", Color) = (1,1,1,1)
      
   }
   SubShader {
      Pass {	
         Tags { "LightMode" = "ForwardBase" } 
            // pass for the first, directional light 
 
         CGPROGRAM
 
         #pragma vertex vert  
         #pragma fragment frag 
 
         #include "UnityCG.cginc"
         uniform float4 _LightColor0; 
            // color of light source (from "Lighting.cginc")
 
  		 uniform sampler2D _BumpMap; 
  		 uniform sampler2D _SpecText;  
      	 uniform float4 _BumpMap_ST;
         uniform sampler2D _MainTex;
         uniform sampler2D _DecalTex;
         uniform float4 _Color; 
         uniform float _Shininess;
         uniform float _Intensity;
 
         struct vertexInput {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float4 tangent : TANGENT;
            float4 texcoord : TEXCOORD0;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float4 tex : TEXCOORD0;
            float levelOfLighting : TEXCOORD1;
               // level of diffuse lighting computed in vertex shader
                
             float3 tangentWorld : TEXCOORD2;  
		     float3 normalWorld : TEXCOORD3;
		     float3 binormalWorld : TEXCOORD4;
		     float4 posWorld : TEXCOORD5;
         };
 
         vertexOutput vert(vertexInput input) 
         {
            vertexOutput output;
 
            float4x4 modelMatrix = unity_ObjectToWorld;
            float4x4 modelMatrixInverse = unity_WorldToObject;
 
            float3 normalDirection = normalize(
               mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
            float3 lightDirection = normalize(
               _WorldSpaceLightPos0.xyz);
               
             output.tangentWorld = normalize(
            	mul(modelMatrix, float4(input.tangent.xyz, 0.0)).xyz);
         	output.normalWorld = normalize(
            	mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
         	output.binormalWorld = normalize(
            	cross(output.normalWorld, output.tangentWorld) 
            	* input.tangent.w); // tangent.w is specific to Unity

         	output.posWorld = mul(modelMatrix, input.vertex);
 
            output.levelOfLighting = 
               max(0.0, dot(normalDirection, lightDirection));
            output.tex = input.texcoord;
            output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
            float4 nighttimeColor = 
               tex2D(_MainTex, input.tex.xy);    
            float4 daytimeColor = 
               tex2D(_DecalTex, input.tex.xy);
               
           float4 diffuseColor = lerp(nighttimeColor, daytimeColor, 
               input.levelOfLighting);
               
            float4 encodedNormal = tex2D(_BumpMap, 
           _BumpMap_ST.xy * input.tex.xy + _BumpMap_ST.zw);
        float3 localCoords = float3(2.0 * encodedNormal.a - 1.0, 
            2.0 * encodedNormal.g - 1.0, 0.0);
        localCoords.z = sqrt(1.0 - dot(localCoords, localCoords));
           // approximation without sqrt:  localCoords.z = 
           // 1.0 - 0.5 * dot(localCoords, localCoords);

        float3x3 local2WorldTranspose = float3x3(
           input.tangentWorld,
           input.binormalWorld, 
           input.normalWorld);
        float3 normalDirection = 
           normalize(mul(localCoords, local2WorldTranspose));

        float3 viewDirection = normalize(
           _WorldSpaceCameraPos - input.posWorld.xyz);
        float3 lightDirection;
        float attenuation;

        if (0.0 == _WorldSpaceLightPos0.w) // directional light?
        {
           attenuation = 1.0; // no attenuation
           lightDirection = normalize(_WorldSpaceLightPos0.xyz);
        } 
        else // point or spot light
        {
           float3 vertexToLightSource = 
              _WorldSpaceLightPos0.xyz - input.posWorld.xyz;
           float distance = length(vertexToLightSource);
           attenuation = 1.0 / distance; // linear attenuation 
           lightDirection = normalize(vertexToLightSource);
        }

        float3 diffuseReflection = 
           attenuation * _LightColor0.rgb * diffuseColor
           * abs(dot(normalDirection, lightDirection));

        float3 specularReflection;
        if (dot(normalDirection, lightDirection) < 0.0) 
           // light source on the wrong side?
        {
           specularReflection = float3(0.0, 0.0, 0.0); 
              // no specular reflection
        }
        else // light source on the right side
        {
        float4 specColor = tex2D(_SpecText, input.tex.xy);
        float4 specValue = ((specColor.x + specColor.y + specColor.z)/3)*_Intensity;
        
           specularReflection = attenuation * _LightColor0.rgb 
              * specValue 
              * pow(max(0.0, dot(
              	reflect(-lightDirection, normalDirection), 
              viewDirection)), _Shininess);
        }
        return float4(diffuseReflection + specularReflection * input.levelOfLighting, 1.0);
	}
 
     ENDCG
      }
   } 
   Fallback "Decal"
}