Shader "Particle"
{
	Properties
	{
		_Metallic("Metallic", Range(0,1)) = 1.0
		_Smoothness("Smoothness", Range(0,1)) = 0.5
		[HideInInspector] _texcoord("", 2D) = "white" {}
	}
		SubShader
	{
		Tags { "RenderType" = "Opaque" }
		CGPROGRAM
		#pragma surface SurfaceShader Standard fullforwardshadows addshadow
		#pragma instancing_options procedural:setup

		float _Smoothness, _Metallic;

		struct Input
		{
			float2 uv_texcoord;
		};

		struct MParticle
		{
			float3 position;
			float3 velocity;
			float mass;
			float lifetime;
			float3 direction;
			float radius;

		};

		#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
			StructuredBuffer<MParticle> ParticleBuffer;
			StructuredBuffer<float4> ParticlePropsBuffer;
		#endif

		void setup()
		{
			#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
				MParticle particle = ParticleBuffer[unity_InstanceID];
				float s = particle.radius * 2.0;
				unity_ObjectToWorld._11_21_31_41 = float4(s, 0, 0, 0);
				unity_ObjectToWorld._12_22_32_42 = float4(0, s, 0, 0);
				unity_ObjectToWorld._13_23_33_43 = float4(0, 0, s, 0);
				unity_ObjectToWorld._14_24_34_44 = float4(particle.position, 1);
				unity_WorldToObject = unity_ObjectToWorld;
				unity_WorldToObject._14_24_34 *= -1;
				unity_WorldToObject._11_22_33 = 1.0f / unity_WorldToObject._11_22_33;
			#endif
		}

		void SurfaceShader(Input IN, inout SurfaceOutputStandard o)
		{
			o.Albedo = 1;
			o.Metallic = _Metallic;
			o.Smoothness = _Smoothness;
			o.Alpha = 1.0;
			#ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
				o.Albedo *= ParticlePropsBuffer[unity_InstanceID].xyz;
				o.Smoothness *= ParticlePropsBuffer[unity_InstanceID].w;
			#endif
		}
		ENDCG
	}
		FallBack "Diffuse"
}
