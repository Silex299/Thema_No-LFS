// Made with Amplify Shader Editor
// Converted to URP by assistant
Shader "QFX/SFX/Beam/Energy Beam URP"
{
    Properties
    {
        [HDR]_TintColor("Tint Color", Color) = (0,0,0,0)
        _MainTex("Main Tex", 2D) = "white" {}
        _MainSpeed("Main Speed", Vector) = (0,0,0,0)
        _MaskTex("Mask Tex", 2D) = "white" {}
        _NoiseMap("Noise Map", 2D) = "white" {}
        _DistortionMap("Distortion Map", 2D) = "white" {}
        _NoiseDistortionPower("Noise Distortion Power", Range(0, 2)) = 0.2179676
        _NoiseDistortionTilling("Noise Distortion Tilling", Range(0, 10)) = 1.142912
        _NoiseDistortionSpeed("Noise Distortion Speed", Vector) = (0,0.2,0,0)
        _NoisePower("Noise Power", Range(0, 2)) = 1.098096
        _NoiseOffset("Noise Offset", Range(0, 1)) = 0
        _NoiseTilling("Noise Tilling", Range(0, 10)) = 0.894883
        _NoiseSpeed("Noise Speed", Vector) = (0.2,0,0,0)
        _Float0("Float 0", Float) = 0
        [HideInInspector] _texcoord("", 2D) = "white" {}
        [HideInInspector] __dirty("", Int) = 1
    }

    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 3.0
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            
            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);
            TEXTURE2D(_NoiseMap);
            SAMPLER(sampler_NoiseMap);
            TEXTURE2D(_DistortionMap);
            SAMPLER(sampler_DistortionMap);
            TEXTURE2D(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);

            float4 _TintColor;
            float2 _MainSpeed;
            float _NoiseDistortionPower;
            float _NoiseDistortionTilling;
            float2 _NoiseDistortionSpeed;
            float2 _NoiseSpeed;
            float _NoiseTilling;
            float _NoisePower;
            float _NoiseOffset;
            float _Float0;

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS);
                output.uv = input.uv;
                output.screenPos = TransformObjectToHClip(input.positionOS);
                return output;
            }

            half4 ASE_ComputeGrabScreenPos(Varyings i)
            {
                half4 o = i.screenPos;
                #if UNITY_UV_STARTS_AT_TOP
                o.y = (_ScreenParams.y - o.y);
                #endif
                return o;
            }

            half4 frag(Varyings i) : SV_TARGET
            {
                float2 uv_MainTex = i.uv + _MainSpeed * _Time.y;
                float2 uv_Distortion = i.uv * _NoiseDistortionTilling + _NoiseDistortionSpeed * _Time.y;
                float distortion = SAMPLE_TEXTURE2D(_DistortionMap, sampler_DistortionMap, uv_Distortion).r * _NoiseDistortionPower;
                float2 uv_Noise = i.uv * _NoiseTilling + _NoiseSpeed * _Time.y;
                float noise = SAMPLE_TEXTURE2D(_NoiseMap, sampler_NoiseMap, uv_Noise).r * _NoisePower - _NoiseOffset;
                float4 mainColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv_MainTex + distortion + noise);

                half4 screenPos = ASE_ComputeGrabScreenPos(i);
                float4 screenColor = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, screenPos.xy / screenPos.w);

                float4 finalColor = (_TintColor * mainColor) + screenColor;
                float alpha = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, i.uv).r;
                finalColor.a *= alpha;

                return finalColor;
            }
            ENDHLSL
        }
    }

    CustomEditor "ASEMaterialInspector"
} 