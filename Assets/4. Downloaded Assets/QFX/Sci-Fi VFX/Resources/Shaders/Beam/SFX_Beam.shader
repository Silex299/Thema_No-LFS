// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "QFX/SFX/Beam/Energy Beam"
{
	Properties
	{
		[HDR]_TintColor("Tint Color", Color) = (0,0,0,0)
		_MainTex("Main Tex", 2D) = "white" {}
		_MainSpeed("Main Speed", Vector) = (0,0,0,0)
		_MaskTex("Mask Tex", 2D) = "white" {}
		_NoiseMap("Noise Map", 2D) = "white" {}
		_DistortionMap("Distortion Map", 2D) = "white" {}
		_NoiseDistortionPower("Noise Distortion Power", Range( 0 , 2)) = 0.2179676
		_NoiseDistortionTilling("Noise Distortion Tilling", Range( 0 , 10)) = 1.142912
		_NoiseDistortionSpeed("Noise Distortion Speed", Vector) = (0,0.2,0,0)
		_NoisePower("Noise Power", Range( 0 , 2)) = 1.098096
		_NoiseOffset("Noise Offset", Range( 0 , 1)) = 0
		_NoiseTilling("Noise Tilling", Range( 0 , 10)) = 0.894883
		_NoiseSpeed("Noise Speed", Vector) = (0.2,0,0,0)
		_Float0("Float 0", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		Cull Off
		GrabPass{ }
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit alpha:fade keepalpha noshadow 
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPos;
		};

		uniform float4 _TintColor;
		uniform sampler2D _MainTex;
		uniform float2 _MainSpeed;
		uniform float4 _MainTex_ST;
		uniform sampler2D _NoiseMap;
		uniform sampler2D _DistortionMap;
		uniform float2 _NoiseDistortionSpeed;
		uniform float _NoiseDistortionTilling;
		uniform float _NoiseDistortionPower;
		uniform float2 _NoiseSpeed;
		uniform float _NoiseTilling;
		uniform float _NoisePower;
		uniform float _NoiseOffset;
		uniform float _Float0;
		uniform sampler2D _GrabTexture;
		uniform sampler2D _MaskTex;
		uniform float4 _MaskTex_ST;


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			float2 panner105 = ( 1.0 * _Time.y * _MainSpeed + uv_MainTex);
			float2 panner21_g1 = ( 1.0 * _Time.y * _NoiseDistortionSpeed + i.uv_texcoord);
			float temp_output_72_0_g1 = ( tex2D( _DistortionMap, (panner21_g1*_NoiseDistortionTilling + 0.0) ).r * _NoiseDistortionPower );
			float2 panner71_g1 = ( 1.0 * _Time.y * _NoiseSpeed + i.uv_texcoord);
			float4 temp_cast_0 = (_Float0).xxxx;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			float4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float4 screenColor76 = tex2D( _GrabTexture, ( temp_output_72_0_g1 + ase_grabScreenPosNorm ).xy );
			o.Emission = ( ( _TintColor * pow( tex2D( _MainTex, ( panner105 + ( ( tex2D( _NoiseMap, ( temp_output_72_0_g1 + (panner71_g1*_NoiseTilling + 0.0) ) ).r * _NoisePower ) - _NoiseOffset ) ) ) , temp_cast_0 ) ) + screenColor76 ).rgb;
			float2 uv_MaskTex = i.uv_texcoord * _MaskTex_ST.xy + _MaskTex_ST.zw;
			o.Alpha = tex2D( _MaskTex, uv_MaskTex ).r;
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}