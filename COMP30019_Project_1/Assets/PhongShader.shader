// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "PhongShader"
{
	SubShader
	{

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			uniform float4 _LightColor;
			uniform float4 _LightPosition;

			struct vertIn
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
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

				o.worldNormal = UnityObjectToWorldNormal(i.normal);

				o.color = i.color;
				
				return o;
			}
			
			float4 frag (vertOut o) : SV_Target
			{
				float3 interpNormal = normalize(o.worldNormal);

				// Calculate ambient RGB intensities
				float Ka = 3;
				float3 amb = o.color.rgb * UNITY_LIGHTMODEL_AMBIENT.rgb * Ka;

				// Calculate diffuse RBG reflections, we save the results of L.N because we will use it again
				// (when calculating the reflected ray in our specular component)
				float fAtt = 0.0007;
				float Kd = 1;
				float3 L = normalize(_LightPosition.xyz - o.worldVertex.xyz);
				float LdotN = dot(L, interpNormal);
				float3 dif = fAtt * _LightColor.rgb * Kd * o.color.rgb * saturate(LdotN);

				// Calculate specular reflections
				float Ks = 1;
				float specN = 1; // Values>>1 give tighter highlights
				float3 V = normalize(_WorldSpaceCameraPos.xyz - o.worldVertex.xyz);

				// Using classic reflection calculation
				float3 R = normalize((2.0 * LdotN * interpNormal) - L);
				float3 spe = fAtt * _LightColor.rgb * Ks * pow(saturate(dot(V, R)), specN);

				fixed4 returnColor = 0;
				returnColor.rgb = amb.rgb + dif.rgb + spe.rgb;
				returnColor.a = o.color.a;

                return returnColor;
			}
			ENDCG
		}
	}
}
