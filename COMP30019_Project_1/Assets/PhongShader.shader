// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/PhongShader"
{
	SubShader
	{

		Pass
		{
			Tags { "LightMode" = "ForwardBase" }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			uniform float4 _LightColor;
			uniform float4 _LightPosition;

			struct vertIn
			{
				float4 vertex : POSITION;
				float4 normal : NORMAL;
				float4 color : COLOR;
			};

			struct vertOut
			{
				float4 vertex : SV_POSITION;
				float4 color : COLOR;
				float4 worldVertex : TEXCOORD0;
				float3 worldNormal : TEXCOORD1;
			};
			
			vertOut vert (vertIn i)
			{
				vertOut o;

				o.vertex = UnityObjectToClipPos(i.vertex);
				o.worldVertex = mul(unity_ObjectToWorld, i.vertex);

				o.worldNormal = normalize(mul(transpose((float3x3)unity_WorldToObject), i.normal.xyz));

				o.color = i.color;
				
				return o;
			}
			
			fixed4 frag (vertOut o) : SV_Target
			{
				float3 interpNormal = normalize(o.worldNormal);

				// Calculate ambient RGB intensities
				float Ka = 3;
				float3 amb = o.color.rgb * UNITY_LIGHTMODEL_AMBIENT.rgb * Ka;

				// Calculate diffuse RBG reflections, we save the results of L.N because we will use it again
				// (when calculating the reflected ray in our specular component)
				float fAtt = 0.002;
				float Kd = 1;
				float3 L = normalize(_LightPosition - o.worldVertex.xyz);
				float LdotN = dot(L, interpNormal);
				float3 dif = fAtt * _LightColor.rgb * Kd * o.color.rgb * saturate(LdotN);

				// Calculate specular reflections
				float Ks = 1;
				float specN = 80; // Values>>1 give tighter highlights
				float3 V = normalize(_WorldSpaceCameraPos.xyz - o.worldVertex.xyz);

				// Using classic reflection calculation
				// this works somewhat
				float3 R = normalize((2.0 * LdotN * interpNormal) - L);
				float3 spe = fAtt * _LightColor.rgb * Ks * pow(saturate(dot(V, R)), specN);

				// this never works.
				// Using Blinn-Phong approximation:
			 	// 	specN = 200; // We usually need a higher specular power when using Blinn-Phong
				// float3 H = normalize(V + L);
				// float3 spe = 1 * _LightColor.xyz * Ks * pow(saturate(dot(interpNormal, H)), specN);

				float4 returnColor = float4(0.0f, 0.0f, 0.0f, 0.0f);
				returnColor.rgb = amb.rgb + dif.rgb + spe.rgb;
				returnColor.a = o.color.z;

				return returnColor;
			}
			ENDCG
		}
	}
}
