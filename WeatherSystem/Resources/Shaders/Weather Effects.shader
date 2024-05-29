
Shader "AspectWeather/Weather Effects"
{
	Properties 
	{
		[Title(Particle Shader Settings)]
		[Main(GroupBasic, _KEYWORD, on)] _group1 ("Main", float) = 1
		[Sub(GroupBasic)]_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		[Sub(GroupBasic)]_AmbientColor ("Ambient Color", Color) = (1,1,1,1)
		[Sub(GroupBasic)]_MainTex ("Particle Texture", 2D) = "white" {}
		[Sub(GroupBasic)]_InvFade ("Soft Particles Factor", Range(0.01,3.0)) = 1.0
		[Sub(GroupBasic)]_AmbientBlendStrength ("Ambient Blend Strength", Range(0,1)) = 0.7

		[Main(Preset, _, on, off)] _PresetGroup ("Preset", float) = 0
		[Preset(Preset, LWGUI_BlendModePreset)] _BlendMode ("Blend Mode Preset", float) = 0
		[SubEnum(Preset, UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 2
		[SubEnum(Preset, UnityEngine.Rendering.BlendMode)] _SrcBlend ("SrcBlend", Float) = 1
		[SubEnum(Preset, UnityEngine.Rendering.BlendMode)] _DstBlend ("DstBlend", Float) = 0
		[SubToggle(Preset)] _ZWrite ("ZWrite ", Float) = 1
		[SubEnum(Preset, UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", Float) = 4 // 4 is LEqual
		[SubEnum(Preset, RGBA, 15, RGB, 14)] _ColorMask ("ColorMask", Float) = 15 // 15 is RGBA (binary 1111)
	}

	SubShader {
		Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}
		//Blend SrcAlpha One
		//ColorMask RGB
		//Cull Off Lighting Off ZWrite Off
		Pass {
			Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward"}

			LOD 150
            Cull [_Cull]
            ZWrite [_ZWrite]
            Blend [_SrcBlend] [_DstBlend]
            ColorMask [_ColorMask]

			HLSLPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_particles
			#pragma multi_compile_fog

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

			CBUFFER_START(UnityPerMaterial)
			float4 _MainTex_ST;
			half4 _TintColor;
			float _AmbientBlendStrength;
			float _InvFade;
			half4 _AmbientColor;
			CBUFFER_END

			TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

			struct appdata_t {
				float4 vertex : POSITION;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f {
				float4 vertex : SV_POSITION;
				half4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				//UNITY_FOG_COORDS(1)
				#ifdef SOFTPARTICLES_ON
				float4 projPos : TEXCOORD2;
				#endif
				UNITY_VERTEX_OUTPUT_STEREO
			};
			

			v2f vert (appdata_t v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

				o.vertex = TransformObjectToHClip(v.vertex);
				#ifdef SOFTPARTICLES_ON
				o.projPos = ComputeScreenPos(o.vertex);
				COMPUTE_EYEDEPTH(o.projPos.z);
				#endif
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				//UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			half4 frag (v2f i) : SV_Target
			{
				#ifdef SOFTPARTICLES_ON
				float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
				float partZ = i.projPos.z;
				float fade = saturate (_InvFade * (sceneZ-partZ));
				i.color.a *= fade;
				#endif
				
				half4 col = (_AmbientColor * _AmbientBlendStrength) * i.color * _TintColor * SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex ,i.texcoord);
				//UNITY_APPLY_FOG_COLOR(i.fogCoord, col, half4(0,0,0,0));
				
				return col;
			}
			ENDHLSL
		}
	}	
	CustomEditor "LWGUI.LWGUI"
}
