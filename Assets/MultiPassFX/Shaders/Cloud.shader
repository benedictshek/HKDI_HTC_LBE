Shader "MultiPassFX/Cloud"
{
	Properties
	{
		[NoScaleOffset]_CloudNoiseTex("Cloud Noise", 3D) = "" {}
		_CloudNoiseUVParams("Noise UV Params xyz:Speed w:Tiling", Vector) = (1, 1, 1, 1)

		_CloudSmoothness("Cloud Smoothness", Range(1, 10)) = 2
		_CloudLength("Cloud Length", Float) = 1
		_CloudShrink("Cloud Shrink", Float) = 0.2
		_CloudLayerAttenuation("Cloud Layer Attenuation", Range(0, 1)) = 0.4

		[NoScaleOffset]_DitherTex("Dither Texture", 2D) = "black" {}
		_DitherStrength("Dither Strength", Range(0.0, 1)) = 0.2

		_BaseMap("Base Map", 2D) = "white" {}
		_BaseColor("Base Color", Color) =(1, 1, 1, 1)
		// Use for ParticleSystem pass color
		[HideInInspector]_Color("Color", Color) = (1, 1, 1, 1)
		_ShadowColor("Shadow Color", Color) =(0.2, 0.55, 0.75, 1)

		_BackLayerEnhance("Back Light Enhance", Range(0, 1)) = 0.4
		_BackSSSRange("Back SSS Range", Range(0.1, 1)) = 0.5
		_BackSSSIntensity("Back SSS Intensity", Range(0, 5)) = 1

		[Toggle(_DEPTH_FADE_ON)]_DepthFadeOn("Depth Fade On", Float) = 0
		_DepthFadeDistance("Depth Fade Distance", Range(0.01, 100)) = 1
		_CloudOutsideFade("Cloud Outside Fade", Range(0.2, 5)) = 1.0

		[HideInInspector]_Surface("Surface", Float) = 1.0
		[HideInInspector][Enum(UnityEngine.Rendering.BlendMode)]_SrcBlend("Src Blend", Float) = 5.0
		[HideInInspector][Enum(UnityEngine.Rendering.BlendMode)]_DstBlend("Dst Blend", Float) = 10.0
		[HideInInspector][Enum(UnityEngine.Rendering.BlendMode)]_SrcBlendAlpha("Src Blend Alpha", Float) = 1.0
		[HideInInspector][Enum(UnityEngine.Rendering.BlendMode)]_DstBlendAlpha("Dst Blend Alpha", Float) = 10.0

		[HideInInspector][Toggle]_ZWrite("ZWrite", Float) = 0.0
		[HideInInspector][Enum(UnityEngine.Rendering.CullMode)]_Cull("Cull", Float) = 2.0
	}

	SubShader
	{
		Tags 
		{ 
			"RenderType" = "Transparent"
			"Queue" = "Transparent"
			"IsMultiPass" = "True"
		}

		Pass
		{
			Name "Cloud"

			Cull [_Cull]
			Blend [_SrcBlend] [_DstBlend], [_SrcBlendAlpha] [_DstBlendAlpha]
			ZWrite [_ZWrite]

			HLSLPROGRAM

			#pragma multi_compile_instancing

			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN

			#pragma multi_compile_local _ _ENABLE_SKINNED_MESH_BUFFER

			#pragma shader_feature_local _DEPTH_FADE_ON
			#pragma shader_feature_local _EDGE_DITHER

			#pragma vertex	 CloudVert
			#pragma fragment CloudFrag

			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
			#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
			#include "SkinnedMeshBuffer.hlsl"

			struct Attributes
			{
				SKINNED_MESH_VERTEX_INPUT
				float2 uv		  : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				half3 normalWS    : TEXCOORD0;
				half3 tangentWS   : TEXCOORD1;
				half3 bitangentWS : TEXCOORD2;
				float4 positionWS : TEXCOORD3;
				float4 shapeUV	  : TEXCOORD4;
				float4 positionSS : TEXCOORD5;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			TEXTURE2D(_BaseMap);
			SAMPLER(sampler_BaseMap);

			TEXTURE3D(_CloudNoiseTex);
			SAMPLER(sampler_CloudNoiseTex);

			TEXTURE2D(_DitherTex);
			SAMPLER(sampler_DitherTex);

			half _PassCount;
			float4 _DitherTex_TexelSize;

			float4 _CloudNoiseUVParams;
			#define _CloudNoiseUVSpeed _CloudNoiseUVParams.xyz
			#define _CloudNoiseUVTiling _CloudNoiseUVParams.w

			float _CloudSmoothness;
			float _CloudLength;
			float _CloudShrink;

			half4 _BaseColor;		
			half4 _ShadowColor;
			half4 _Color;

			half _CloudLayerAttenuation;
			half _BackSSSRange;
			half _BackSSSIntensity;
			half _BackLayerEnhance;

			half _DitherStrength;

			float _CloudOutsideFade;
			float _DepthFadeDistance;

			Varyings CloudVert(Attributes input)
			{
				Varyings output;

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				SETUP_SKINNED_MESH_INPUT(input);

				VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
				output.tangentWS = normalInput.tangentWS;
				output.bitangentWS = normalInput.bitangentWS;
				output.normalWS = normalInput.normalWS;

			#if UNITY_ANY_INSTANCING_ENABLED
				float currentPass = UNITY_GET_INSTANCE_ID(input);
			#else
				float currentPass = 0;
			#endif

				float passWeight = (currentPass + 1) / _PassCount;
				float3 positionOS = input.positionOS.xyz + input.normalOS * (_CloudLength * passWeight - _CloudShrink);
				output.positionWS.xyz = TransformObjectToWorld(positionOS);
				output.positionCS = TransformWorldToHClip(output.positionWS.xyz);
				output.shapeUV.xyz = positionOS * 0.5 + 0.5;
				output.positionSS = ComputeScreenPos(output.positionCS);

				output.positionWS.w = input.uv.x;
				output.shapeUV.w = input.uv.y;

				return output;
			}

			half4 CloudFrag(Varyings input) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(input);

				float2 screenUV = input.positionSS.xy / input.positionSS.w;

				float2 texUV = float2(input.positionWS.w, input.shapeUV.w);

				float4 shadowCoord = TransformWorldToShadowCoord(input.positionWS.xyz);
				Light mainLight = GetMainLight(shadowCoord);

				half3 normalWS = normalize(input.normalWS);

				half3 viewDirWS = SafeNormalize(GetWorldSpaceViewDir(input.positionWS.xyz));
				half NoV = saturate(dot(viewDirWS, normalWS));

			#if UNITY_ANY_INSTANCING_ENABLED
				float currentPass = UNITY_GET_INSTANCE_ID(input);
			#else
				float currentPass = 0;
			#endif

				float3 shapeUV = input.shapeUV.xyz / _CloudNoiseUVTiling + _Time.xxx * _CloudNoiseUVSpeed;
				half noise = SAMPLE_TEXTURE3D(_CloudNoiseTex, sampler_CloudNoiseTex, shapeUV.xyz).r;

				float passWeight = currentPass / _PassCount;
				float layerWeight = currentPass / (_PassCount + 1);
				float clipRate = pow(layerWeight, _CloudSmoothness);
				float edgeFactor = pow(NoV, _CloudOutsideFade);

				float clipValue = noise - clipRate;

			#if _EDGE_DITHER
				float2 ditherOffset = currentPass * (_ScaledScreenParams.zw - 1) * _Time.xx;
				float2 ditherUV = (_DitherTex_TexelSize.xy * _ScaledScreenParams.xy) * (screenUV + ditherOffset);
				float dither = SAMPLE_TEXTURE2D_LOD(_DitherTex, sampler_DitherTex, ditherUV, 0).r * _DitherStrength;
				dither = layerWeight > 0.3 ? dither : 0;
				clipValue -= dither;
			#endif

				clip(clipValue);

				half NoL = saturate(dot(normalWS, mainLight.direction));

				half diffuse = pow(saturate(NoL), 2 - max(clipRate, noise));

				half layerAttenuation = lerp(1, clipRate, lerp(0, _CloudLayerAttenuation, diffuse));

				half backLayerEnhance = lerp(0, _BackLayerEnhance, lerp(clipRate, 0, diffuse));

				half3 backLightDir = normalWS *(1 - _BackSSSRange) + mainLight.direction;

				half backSSS = saturate(dot(viewDirWS, -backLightDir));

				backSSS = saturate(pow(backSSS, 2 + clipRate * 2) * 1.5) * _BackSSSIntensity;

				half shadowFactor = saturate(diffuse * layerAttenuation * mainLight.shadowAttenuation + max(backLayerEnhance, backSSS));

				half4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, texUV) * lerp(_ShadowColor, _BaseColor, shadowFactor) * _Color;

				half3 col = baseColor.rgb * mainLight.color;

				half alpha = baseColor.a * edgeFactor;

			#if _DEPTH_FADE_ON
				float sceneDepth = LinearEyeDepth(SampleSceneDepth(screenUV), _ZBufferParams);
				float depth = LinearEyeDepth(input.positionCS.z, _ZBufferParams);
				alpha *= saturate((sceneDepth - depth) / _DepthFadeDistance);
			#endif

				return half4(col, alpha);
			}

			ENDHLSL
		}
	}

	CustomEditor "MultiPassFX.CloudGUI"
}