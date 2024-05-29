Shader "AspectWeather/Wendy/Procedural Clouds" {
    Properties 
    {
        [Title(Main Texs)]
        [Main(GroupBasic, _KEYWORD, on)] _group1 ("Main", float) = 1
        [Sub(GroupBasic)]_MainTex ("Main Tex", 2D) = "white" { }
        [Sub(GroupBasic)]_MainTex2 ("Base (RGB)", 2D) = "white" {}
        [Sub(GroupBasic)]_MainTex3 ("Base (RGB)", 2D) = "white" {}
        [Sub(GroupBasic)][HDR]_BaseColor ("Ambient Color", Color) = (1, 1, 1, 1)

        [Title(Cloud Part)]
        [Main(GroupCloud),_KEYWORD,on] _group2 ("Cloud", float) = 1
        [Sub(GroupCloud)]_LightColor ("Light Color", Color) = (1,1,1,1)
        [Sub(GroupCloud)]_CloudCover ("Cloud Cover", Range(0,1.25)) = 0.5
        [Sub(GroupCloud)]_CloudSharpness ("Cloud Sharpness", Range(0.2,0.99)) = 0.8
        [Sub(GroupCloud)]_CloudDensity ("Density", Range(0,1)) = 1
        [Sub(GroupCloud)]_FogAmount ("Fog Amount", Range(0,1)) = 1
        
        
        [Space(5)]
        [Sub(GroupCloud)]_CloudSpeed ("Cloud Formation Speed", Vector) = (0.0003, 0, 0, 0)
        [Space(30)]
        [Sub(GroupCloud)]_ShadowStrength ("Shadow Strength", Range(0,64)) = 50

        [Sub(GroupCloud)] _Seed ("Seed(Noise)", float) = 1

        [Sub(GroupCloud)]_Attenuation ("Attenuation", Range(0,1)) = 1.0
		[Sub(GroupCloud)]_NoiseScale ("Noise Scale", Range(1, 10)) = 10

        [Main(GroupHeight),_KEYWORD,on] _group3 ("Height", float) = 1
        [Sub(GroupHeight)]_LoY ("Opaque Y", Float) = 0
        [Sub(GroupHeight)]_HiY ("Transparent Y", Float) = 10
        [Sub(GroupHeight)]_WorldPosLow ("Opaque Y2", Float) = 0
        [Sub(GroupHeight)]_WorldPosHigh ("Transparent Y2", Float) = 10
		[Sub(GroupHeight)]_WorldPosGlobal ("Transparent Global", Range(0,2500)) = 0

        

        [Title(Preset Part)]
        [Main(Preset, _, on, off)] _PresetGroup ("Preset", float) = 0
		[Preset(Preset, LWGUI_BlendModePreset)] _BlendMode ("Blend Mode Preset", float) = 0
		[SubEnum(Preset, UnityEngine.Rendering.CullMode)] _Cull ("Cull", Float) = 2
		[SubEnum(Preset, UnityEngine.Rendering.BlendMode)] _SrcBlend ("SrcBlend", Float) = 1
		[SubEnum(Preset, UnityEngine.Rendering.BlendMode)] _DstBlend ("DstBlend", Float) = 0
		[SubToggle(Preset)] _ZWrite ("ZWrite ", Float) = 1
		[SubEnum(Preset, UnityEngine.Rendering.CompareFunction)] _ZTest ("ZTest", Float) = 4 // 4 is LEqual
		[SubEnum(Preset, RGBA, 15, RGB, 14)] _ColorMask ("ColorMask", Float) = 15 // 15 is RGBA (binary 1111)
    }
    SubShader 
    {
        Tags
        {
            "Queue"="Transparent-100" 
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
            "RenderPipeline" = "UniversalPipeline"
        }

        HLSLINCLUDE
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        
        CBUFFER_START(UnityPerMaterial)
        float4 _LightColor;
        float4 _BaseColor;
        float4 _CloudSpeed;
        float4 _CloudScale;

        float4 FogColor;
        float _FogAmount;
    
        float _CloudCover;
        float _CloudDensity;
        float _CloudSharpness;
        float _ShadowStrength;

        half _LoY;
        half _HiY;
        half _WorldPosLow;
        half _WorldPosHigh;
        half _WorldPosGlobal;

        uniform float _Attenuation,_Seed;

        float4 _MainTex_ST;
        float4 _MainTex2_ST;
        float4 _MainTex3_ST;

        float3 normalDirection;
        float3 diffuseReflection;
        float4 Final;
        float3 lightDirection;

        float _NoiseScale;
        CBUFFER_END
        ENDHLSL


        Pass {
        Name "ForwardLit"
        Tags { "LightMode" = "UniversalForward"}
    
        //Blend SrcAlpha OneMinusSrcAlpha
        //Cull Front 
        //ZWrite Off

        //LOD 150
        Cull [_Cull]
        ZWrite [_ZWrite]
        Blend [_SrcBlend] [_DstBlend]
        ColorMask [_ColorMask] // write channels into current pass

        HLSLPROGRAM
    
        #pragma target 3.0
        #pragma vertex vert
        #pragma fragment frag
        #include "noiseSimplex.cginc"
        

        TEXTURE2D (_MainTex);
        SAMPLER(sampler_MainTex);

        TEXTURE2D (_MainTex2);
        SAMPLER(sampler_MainTex2);

        TEXTURE2D (_MainTex3);
        SAMPLER(sampler_MainTex3);

        TEXTURE2D (_Jitter);
        SAMPLER(sampler_Jitter);

        struct Attributes
        {
            float4 vertex: POSITION;
            float3 normal : NORMAL;
            float4 texcoord: TEXCOORD0;
            UNITY_VERTEX_INPUT_INSTANCE_ID
        };
    
        struct Varyings {
            float4 pos : SV_POSITION;
            float2 tex : TEXCOORD0;
            float2 alpha : TEXCOORD1;
            float2 HeightScale : TEXCOORD5;
            float3 normal : NORMAL;
            float4 colNEW : COLOR;
            float4 worldPos : TEXCOORD2;
            float2 viewDirection : TEXCOORD4;
            UNITY_VERTEX_INPUT_INSTANCE_ID
            UNITY_VERTEX_OUTPUT_STEREO
        };

    
    

        Varyings vert (Attributes v) 
        {
            Varyings o = (Varyings)0;

            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_TRANSFER_INSTANCE_ID(v, o);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

            normalDirection = normalize( mul( float4(v.normal, 0.0 ), unity_WorldToObject).xyz);

            _Attenuation = clamp(_Attenuation, 0, 1);
            float atten = _Attenuation-(_CloudCover-0.25); 

            lightDirection = normalize(GetMainLight().direction);
            diffuseReflection = atten * max(0.0, dot(normalDirection, lightDirection));

            o.colNEW = float4(diffuseReflection, 1.0);

            o.worldPos = mul (unity_ObjectToWorld, v.vertex);
            o.alpha = 1 - saturate((o.worldPos.y - _LoY) / (_HiY - _LoY));
            o.HeightScale = 1 - saturate((o.worldPos.y - _WorldPosLow) / ((_WorldPosHigh-_WorldPosGlobal) - _WorldPosLow)-_CloudCover);

            o.pos = TransformObjectToHClip(v.vertex);
            o.tex = v.texcoord.xy;

            return o;
        }


        float4 frag (Varyings i) : COLOR
        {
            UNITY_SETUP_INSTANCE_ID(i);
            UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);

            float2 offset = _Time.xy * _CloudSpeed.xy;
            float4 tex = SAMPLE_TEXTURE2D( _MainTex,sampler_MainTex, (i.tex.xy) + offset );
            float Density = 0;

            float ns = snoise(i.tex.xy*_NoiseScale+ offset*210+_Seed);
            float4 NoiseCol = float4(ns,ns,ns,ns+_CloudCover);

            float ns2 = snoise(NoiseCol*0.001);
            float4 NoiseCol2 = float4(ns,ns,ns,ns2+_CloudCover);

            half4 col = SAMPLE_TEXTURE2D(_MainTex,sampler_MainTex,i.tex.xy * _MainTex_ST.xy + offset * 2);
            half4 col2 = SAMPLE_TEXTURE2D(_MainTex2,sampler_MainTex2,i.tex.xy * _MainTex2_ST.xy + offset * 150)*3;
            half4 col3 = SAMPLE_TEXTURE2D(_MainTex3,sampler_MainTex3,i.tex.xy * _MainTex3_ST.xy + offset * 15)*0.5;

            half4 cloudColor = SAMPLE_TEXTURE2D( _MainTex,sampler_MainTex, (i.tex.xy) + offset * 50);
            tex.a = (col2.a*NoiseCol2.a*1.25) * col3.a * col.a * (NoiseCol.a) * NoiseCol2.a;
            tex.a = i.alpha.y * tex.a;

            tex = max( tex - ( 1 - _CloudCover), 0 );

            float3 EndTracePos = Final;
            float3 TraceDir = i.colNEW;
            float3 CurTracePos =  TraceDir*50;
            TraceDir *= 4.0;

            tex.a = tex.a * i.HeightScale.xy;
            
            tex.a = 1.0 - pow( _CloudSharpness, tex.a * 128);

            

            for( int i = 0; i < _ShadowStrength; i++)
            {
                CurTracePos += TraceDir; 
                float4 tex2 = ((col2.a*NoiseCol2.a*1.25) * col3.a * col.a * (NoiseCol.a) * NoiseCol2.a) * 256 * _CloudDensity;
                Density += 0.05 * step( CurTracePos.z, tex2.a);
            }

            float Light = 1 / exp( Density * 2);
            float4 FinalColor = float4 (Light * _LightColor.r, Light * _LightColor.g, Light * _LightColor.b, tex.a);
            FinalColor.xyz += (_BaseColor.xyz*1.25)*cloudColor;

            return FinalColor;
        }
        ENDHLSL
    }
  }
  CustomEditor "LWGUI.LWGUI"
}
 