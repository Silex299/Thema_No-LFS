Shader "QFX/SFX/Beam/Energy_Beam_URP"
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
        _NoiseDistortionTilling("Noise Distortion Tiling", Range(0, 10)) = 1.142912
        _NoiseDistortionSpeed("Noise Distortion Speed", Vector) = (0,0.2,0,0)
        _NoisePower("Noise Power", Range(0, 2)) = 1.098096
        _NoiseOffset("Noise Offset", Range(0, 1)) = 0
        _NoiseTilling("Noise Tiling", Range(0, 10)) = 0.894883
        _NoiseSpeed("Noise Speed", Vector) = (0.2,0,0,0)
        _Float0("Float 0", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Transparent" "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"
        }
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
            #pragma target 4.5
            #pragma exclude_renderers gles
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float4 screenPos : TEXCOORD1;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);
            TEXTURE2D(_NoiseMap);
            SAMPLER(sampler_NoiseMap);
            TEXTURE2D(_DistortionMap);
            SAMPLER(sampler_DistortionMap);
            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);

            float4 _TintColor;
            float4 _MainTex_ST;
            float4 _MaskTex_ST;
            float2 _MainSpeed;
            float2 _NoiseDistortionSpeed;
            float _NoiseDistortionTilling;
            float _NoiseDistortionPower;
            float2 _NoiseSpeed;
            float _NoiseTilling;
            float _NoisePower;
            float _NoiseOffset;
            float _Float0;

            Varyings vert(Attributes input)
            {
                Varyings output;
                output.positionHCS = TransformObjectToHClip(input.positionOS);
                output.uv = input.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                output.color = input.color;
                output.screenPos = TransformObjectToHClip(input.positionOS);
                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                float2 uv_MainTex = input.uv * _MainTex_ST.xy + _MainTex_ST.zw;
                float2 panner_Main = _TimeParameters.y * _MainSpeed + uv_MainTex;

                float2 panner_Distortion = _TimeParameters.y * _NoiseDistortionSpeed + input.uv;
                float distortionValue = SAMPLE_TEXTURE2D(_DistortionMap, sampler_DistortionMap, panner_Distortion).r * _NoiseDistortionPower;

                float2 panner_Noise = _TimeParameters.y * _NoiseSpeed + input.uv;
                float4 mainTexSample = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, panner_Main);
                float4 noiseSample = SAMPLE_TEXTURE2D(_NoiseMap, sampler_NoiseMap, panner_Noise * _NoiseTilling);
                float2 screenUV = (input.screenPos.xy / input.screenPos.w) + distortionValue;
                float4 screenColor = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, screenUV);

                float4 colorOutput = (_TintColor * pow(mainTexSample, _Float0)) + screenColor;

                float2 uv_MaskTex = input.uv * _MaskTex_ST.xy + _MaskTex_ST.zw;
                float alpha = SAMPLE_TEXTURE2D(_MaskTex, sampler_MaskTex, uv_MaskTex).r;

                return half4(colorOutput.rgb, alpha);
            }
            ENDHLSL
        }
    } // Add this closing brace to terminate the SubShader block
    CustomEditor "ASEMaterialInspector"
}