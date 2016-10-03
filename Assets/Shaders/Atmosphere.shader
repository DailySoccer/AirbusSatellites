// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Atmosphere" {
   Properties {
      _Color ("Color", Color) = (1, 1, 1, 0.5) 
      
      _AtmophereOpacity ("AtmophereOpacity", range(0, 10)) = 1
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
            
            float3 lightDirection = normalize(
               _WorldSpaceLightPos0.xyz);
               
 
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
 
            output.levelOfLighting = 
               max(0.0, dot(output.normal, lightDirection));
            output.tex = input.texcoord;
 
            output.pos = mul(UNITY_MATRIX_MVP, input.vertex);
            return output;
         }
 
         float4 frag(vertexOutput input) : COLOR
         {
            float3 normalDirection = normalize(input.normal);
            float3 viewDirection = normalize(input.viewDir);
 
            float newOpacity = min( 1.0, (_Color.a 
               / abs( dot (viewDirection, normalDirection ) ) ) * min( 1.0, input.levelOfLighting * _AtmophereOpacity ) );
               
            
	        
	            
            return float4(_Color.rgb, newOpacity);
         }
 
         ENDCG
      }
   }
}