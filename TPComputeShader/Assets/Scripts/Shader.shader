Shader "Custom/Particle" {
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Radius("Sphere Radius", float) = 0.01
	}
		
	SubShader {
		Pass {
		Tags{ "RenderType" = "Opaque" }
		LOD 200
		Blend SrcAlpha one

		CGPROGRAM
		// Physically based Standard lighting model, and enable shadows on all light types
		#pragma vertex vert
		#pragma fragment frag
		#pragma geometry geom 

		#include "UnityCG.cginc"

		// Use shader model 3.0 target, to get nicer looking lighting
		#pragma target 5.0

		struct Particle{
			float3 position;
			float3 velocity;
			float life;
		};
		
		struct PS_INPUT{
			float4 position : SV_POSITION;
			float4 color : COLOR;
			float3 normal : NORMAL;
		};
		struct v2g
		{
			float4 position : SV_POSITION;
			float4 color : COLOR;
		};
		struct g2f
		{
			float4 position : SV_POSITION;
			float4 color : COLOR;
		};
		// particles' data
		StructuredBuffer<Particle> particleBuffer;
		

		PS_INPUT vert(uint vertex_id : SV_VertexID, uint instance_id : SV_InstanceID)
		{
			PS_INPUT o = (PS_INPUT)0;

			// Color
			float life = particleBuffer[instance_id].life;
			float lerpVal = life * 0.25f;
			o.color = fixed4(1.0f, 1.0f - lerpVal + 0.1, 0.0f , lerpVal);

			// Position
			o.position = UnityObjectToClipPos(float4(particleBuffer[instance_id].position, 1.0f));

			return o;
		}
		[maxvertexcount(4)]
		void geom(point v2g i[1], inout TriangleStream<g2f> triStream)
		{
			g2f p;
			float s = 0.01;

			p.color = i[0].color;
			p.position = i[0].position + float4(-s, -s * 2, 0, 0); // En haut a gauche
			triStream.Append(p);
			p.position = i[0].position + float4(s, -s *2, 0, 0); // En haut a droite
			triStream.Append(p);
			p.position = i[0].position + float4(-s, s * 2, 0, 0); // En bas a gauche
			triStream.Append(p);
			p.position = i[0].position + float4(s, s * 2, 0, 0); // En bas a droite
			triStream.Append(p);
			triStream.RestartStrip();
		}
		float4 frag(PS_INPUT i) : COLOR
		{
			return i.color;
		}

		

		ENDCG
		}
	}
	FallBack Off
}