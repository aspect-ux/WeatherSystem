Shader "AspectWeather/Wendy/Stars" {
Properties {
	_Color ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
	_StarTex1 ("Star Texture 1", 2D) = "white" {}
	_StarTex2 ("Star Texture 2", 2D) = "white" {}
	_StarSpeed ("Rotation Speed", Float) = 2.0
	_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
	_LoY ("Opaque Y", Float) = 0
    _HiY ("Transparent Y", Float) = 10
}

Category {
	Tags { "Queue"="Transparent-1000" "IgnoreProjector"="True" "RenderType"="Transparent" }
	Blend SrcAlpha One
	ColorMask RGB
	Cull Front 
	Lighting Off 
	ZWrite Off

	SubShader 
	{
	
		Pass 
		{
			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_particles
			//#pragma multi_compile_fog

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

			#ifndef UNITY_PI
			#define UNITY_PI 3.14159
			#endif

			CBUFFER_START(UnityPerMaterial)
			half4 _Color;
			half _LoY;
      		half _HiY;

			float4 _StarTex1_ST;
			float4 _StarTex2_ST;
			float _StarSpeed;
			float _Rotation;

			float _InvFade;
			CBUFFER_END

			TEXTURE2D_X(_CameraDepthTexture);
			

			TEXTURE2D (_MainTex);
        	SAMPLER(sampler_MainTex);

			TEXTURE2D (_StarTex1);
        	SAMPLER(sampler_StarTex1);

			TEXTURE2D (_StarTex2);
        	SAMPLER(sampler_StarTex2);

			
			struct appdata_t {
				float4 vertex : POSITION;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float4 texcoord1 : TEXCOORD1;
				#ifdef SOFTPARTICLES_ON
				float4 projPos : TEXCOORD2;
				#endif
				UNITY_VERTEX_INPUT_INSTANCE_ID
            	UNITY_VERTEX_OUTPUT_STEREO
			};

			float2 rotateUV(float2 uv, float degrees)
            {
               const float Rads = (UNITY_PI * 2.0) / 360.0;
 
               float ConvertedRadians = degrees * Rads;
               float _sin = sin(ConvertedRadians);
               float _cos = cos(ConvertedRadians);
 
                float2x2 R_Matrix = float2x2( _cos, -_sin, _sin, _cos);
 
                uv -= 0.5;
                uv = mul(R_Matrix, uv);
                uv += 0.5;
 
                return uv;
            }

			v2f vert (appdata_t v)
			{
				v2f o = (v2f)0;

				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.vertex = TransformObjectToHClip(v.vertex);
				#ifdef SOFTPARTICLES_ON
				o.projPos = ComputeScreenPos (o.vertex);
				COMPUTE_EYEDEPTH(o.projPos.z);
				#endif
				o.color = v.color;

				_Rotation = _Time.x*_StarSpeed*10;

				o.texcoord1.xy = TRANSFORM_TEX(rotateUV(v.texcoord, _Rotation), _StarTex1);
				o.texcoord1.zw = TRANSFORM_TEX(rotateUV(v.texcoord, _Rotation), _StarTex2);

				float4 worldV = mul (unity_ObjectToWorld, v.vertex);
		        o.color.a = 1 - saturate((worldV.y - _LoY) / (_HiY - _LoY)); 

				return o;
			}
			
			half4 frag (v2f i) : SV_Target
			{				
				UNITY_SETUP_INSTANCE_ID(i);
            	UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
				half4 col = 1.0f * i.color * _Color * (SAMPLE_TEXTURE2D(_StarTex1,sampler_StarTex1, i.texcoord1.xy) + SAMPLE_TEXTURE2D(_StarTex2,sampler_StarTex2, i.texcoord1.zw));
				return col;
			}
			ENDHLSL
			}
		}	
	}
}
