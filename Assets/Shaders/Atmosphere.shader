// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Atmosphere" {
   Properties {
      //_Color ("Color", Color) = (1, 1, 1, 0.5) 

      _AtmophereOpacity ("AtmophereOpacity", range(0, 10)) = 1

      _EarthSurfaceCorrection ("EarthSurfaceCorrection", range(0, 1)) = 0.73
         // user-specified RGBA color including opacity
   }
   SubShader {
      Tags { "Queue" = "Transparent" } 
         // draw after all opaque geometry has been drawn
      Pass { 
         ZWrite Off // don't occlude other objects
         Blend SrcAlpha OneMinusSrcAlpha // standard alpha blending
 
         CGPROGRAM 
 
         #pragma vertex vert  
         #pragma fragment frag 
 
         #include "UnityCG.cginc"
 
         uniform float4 _Color; // define shader property for shaders
         uniform float _AtmophereOpacity;
         uniform float _EarthSurfaceCorrection;
 
         struct vertexInput {
            float4 vertex : POSITION;
            float3 normal : NORMAL;
            float4 tangent : TANGENT;
            float4 texcoord : TEXCOORD0;
         };
         struct vertexOutput {
            float4 pos : SV_POSITION;
            float3 normal : TEXCOORD0;
            float3 viewDir : TEXCOORD1;
            float4 tex : TEXCOORD2;
            
            // level of diffuse lighting computed in vertex shader
            float3 tangentWorld : TEXCOORD3;  
		    float3 binormalWorld : TEXCOORD4;
		    float3 normalWorld : TEXCOORD5;
		    float4 posWorld : TEXCOORD6;
			float levelOfLighting : TEXCOORD7;
         };
         
 
         vertexOutput vert(vertexInput input)  
         {
            vertexOutput output;
 
            float4x4 modelMatrix = unity_ObjectToWorld;
            float4x4 modelMatrixInverse = unity_WorldToObject; 
            
            float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
               
 
            output.normal = normalize(
               mul(float4(input.normal, 0.0), modelMatrixInverse).xyz);
            output.viewDir = normalize(_WorldSpaceCameraPos 
               - mul(modelMatrix, input.vertex).xyz);
            output.normalWorld = output.normal;   
               
            output.tangentWorld = normalize(
            	mul(modelMatrix, float4(input.tangent.xyz, 0.0)).xyz);
         	output.binormalWorld = normalize(
            	cross(output.normalWorld, output.tangentWorld) 
            	* input.tangent.w); // tangent.w is specific to Unity

         	output.posWorld = mul(modelMatrix, input.vertex);
 
            output.levelOfLighting = max(0.0, dot(output.normal, lightDirection));
            output.tex = input.texcoord;
 
            output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
            float3 normalDirection = normalize(input.normal);
            float3 viewDirection = normalize(input.viewDir);

            //float dotProd = dot(viewDirection, normalDirection);
            //float jellyfishEffect = abs(dotProd-0.27);
            //jellyfishEffect = pow(jellyfishEffect,0.3);

            //jellyfishEffect = abs( dot (viewDirection, normalDirection )  );

            //jellyfishEffect = _Color.a / abs( dot (viewDirection, normalDirection )  );
            float dotProd = dot(viewDirection, normalDirection);
            float inverseCorrection = (1.0/_EarthSurfaceCorrection);
            float atmosMultiplier = 1 / (inverseCorrection - 1);
            // Rayleigh wavelength nm 5.8, 13.5, 33.1
            //float rayleigh = float3(5.8, 13.5, 33.1);

            float dotCorrected = (1 - dotProd) * (1.0/_EarthSurfaceCorrection);
            int isAtmos = max(0.0, sign(dotCorrected - 1));


            float jellyfishEffect = min(1.0, dotCorrected) - (isAtmos * (dotCorrected - 1) * atmosMultiplier);

            //jellyfishEffect = pow(jellyfishEffect, 2.5);// + (dotCorrected * isAtmos * 3);

            float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
            float levelOfLighting = max(0.0, dot(normalDirection, lightDirection));
            //levelOfLighting = input.levelOfLighting;

            float newOpacity = min(1.0, jellyfishEffect  * min(1.0, levelOfLighting * _AtmophereOpacity) );

            
	        //if (dotCorrected >= 1) return float4(1,0,0, 1);

            //return float4(_Color.rgb, newOpacity);
            float3 rayleigh = float3((1-levelOfLighting), 0.2, newOpacity);
            rayleigh.g = (rayleigh.r + rayleigh.b)/2;

            //return float4(levelOfLighting*levelOfLighting*levelOfLighting*newOpacity, newOpacity*levelOfLighting, newOpacity/levelOfLighting , newOpacity);
            return float4(rayleigh, newOpacity);
         }
 
         ENDCG
      }
   }
}