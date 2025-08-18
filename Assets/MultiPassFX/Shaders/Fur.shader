Shader "MultiPassFX/Fur"
{
	Properties
	{
		[NoScaleOffset]_BaseMap("Base Map", 2D) = "white" {}
		_BaseColor("Base Color", Color) = (1, 1, 1, 1)
		// Use for ParticleSystem pass color
		[HideInInspector]_Color("Color", Color) = (1, 1, 1, 1)
		[NoScaleOffset]_NormalMap("Normal Map", 2D) = "bump" { }
		_NormalScale("Normal Scale", Float) = 1
		[NoScaleOffset]_FurMap("Fur Map", 2D) = "white" {}
		_FurMapTiling("Fur Map Tiling", Float) = 1
		[NoScaleOffset]_FurMask("Fur Mask", 2D) = "white" {}
		_FurMaskClip("Fur Mask Clip", Range(0, 1)) = 0.5
		_FurSmoothness("Fur Tip Softness", Range(1, 5)) = 1
		_FurLength("Fur Length", Float) = 0.01
		_FurShrink("Fur Shrink", Float) = 0.0

		_FurLayerAttenuation("Fur Layer Attenuation", Range(0, 1)) = 0.5
		[HDR]_RimColor("Rim Color", Color) = (0, 0, 0, 1)
		_RimPower("Rim Power", Range(0.01, 32.0)) = 8.0
		_RimPlus("Rim Plus", Float) = 0.38

		[HDR]_SpecularColor("Specular Color", Color) = (0, 0, 0, 1)
		[Toggle]_Anisotropy("Anisotropy Direction X/Y", Float) = 0
		_AnisotropyBias("Anisotropic Bias", Range(-1, 1)) = 0
		_AnisotropyPower("Anisotropic Power", Range(1, 256)) = 32
		_AnisotropyStrength("Anisotropic Strength", Range(0, 1)) = 0.5

		[Toggle(_DEPTH_FADE_ON)]_DepthFadeOn("Depth Fade On", Float) = 0
		_DepthFadeDistance("Depth Fade Distance", Range(0.01, 100)) = 1
		_FurOutsideFade("Fur Outside Fade", Range(0.2, 5)) = 1.0

		[HideInInspector]_Surface("Surface", Float) = 0.0
		[HideInInspector][Enum(UnityEngine.Rendering.BlendMode)]_SrcBlend("Src Blend", Float) = 1.0
		[HideInInspector][Enum(UnityEngine.Rendering.BlendMode)]_DstBlend("Dst Blend", Float) = 0.0
		[HideInInspector][Enum(UnityEngine.Rendering.BlendMode)]_SrcBlendAlpha("Src Blend Alpha", Float) = 1.0
		[HideInInspector][Enum(UnityEngine.Rendering.BlendMode)]_DstBlendAlpha("Dst Blend Alpha", Float) = 0.0

		[HideInInspector][Toggle]_ZWrite("ZWrite", Float) = 1.0
		[HideInInspector][Enum(UnityEngine.Rendering.CullMode)]_Cull("Cull", Float) = 2.0
	}

	SubShader
	{
		Tags 
		{ 
			"RenderType" = "TransparentCutout"
			"Queue" = "AlphaTest" 
			"IsMultiPass" = "True"
		}

		Pass
		{
			Name "Fur"

			Cull [_Cull]
            Blend [_SrcBlend] [_DstBlend], [_SrcBlendAlpha] [_DstBlendAlpha]
			ZWrite [_ZWrite]

			HLSLPROGRAM

			#pragma multi_compile_instancing

			#pragma multi_compile _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE _MAIN_LIGHT_SHADOWS_SCREEN

			#pragma multi_compile_local _ _ENABLE_SKINNED_MESH_BUFFER

			#pragma shader_feature_local _NORMALMAP
			#pragma shader_feature_local _DEPTH_FADE_ON
			#pragma shader_feature_local _FUR_MASK

			#pragma vertex	 FurVert
			#pragma fragment FurFrag

			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
			#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
			#include "SkinnedMeshBuffer.hlsl"

			TEXTURE2D(_BaseMap);
			SAMPLER(sampler_BaseMap);

			TEXTURE2D(_NormalMap);
			SAMPLER(sampler_NormalMap);

			TEXTURE2D(_FurMap);
			SAMPLER(sampler_FurMap);

			TEXTURE2D(_FurMask);
			SAMPLER(sampler_FurMask);

			half _PassCount;
			half4 _BaseColor;
			half4 _Color;
			float _FurMapTiling;
			half _FurMaskClip;
			float _FurSmoothness;
			float _FurLength;
			float _FurShrink;
			half _FurLayerAttenuation;
			half4 _RimColor;
			half _RimPower;
			half _RimPlus;

			half4 _SpecularColor;
			half _Anisotropy;
			half _AnisotropyBias;
			half _AnisotropyPower;
			half _AnisotropyStrength;

			float _FurOutsideFade;
			float _DepthFadeDistance;

			struct Attributes
			{
				SKINNED_MESH_VERTEX_INPUT
				float2 uv		  : TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct Varyings
			{
				float4 positionCS : SV_POSITION;
				float4 uv         : TEXCOORD0;
				half3 normalWS    : TEXCOORD1;
				half3 tangentWS   : TEXCOORD2;
				half3 bitangentWS : TEXCOORD3;
				float3 positionWS : TEXCOORD4;
			#if _DEPTH_FADE_ON
				float4 positionSS : TEXCOORD5;
			#endif
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			Varyings FurVert(Attributes input)
			{
				Varyings output = (Varyings)0;

				UNITY_SETUP_INSTANCE_ID(input);
				UNITY_TRANSFER_INSTANCE_ID(input, output);
				SETUP_SKINNED_MESH_INPUT(input);

				VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
				output.tangentWS = normalInput.tangentWS;
				output.bitangentWS = normalInput.bitangentWS;
				output.normalWS = normalInput.normalWS;

				output.positionWS = TransformObjectToWorld(input.positionOS.xyz);

			#if UNITY_ANY_INSTANCING_ENABLED
				float currentPass = UNITY_GET_INSTANCE_ID(input);
			#else
				float currentPass = 0;
			#endif

			#if _FUR_MASK
				half furMask = SAMPLE_TEXTURE2D_LOD(_FurMask, sampler_FurMask, input.uv, 0).r;
			#else
				half furMask = 1.0;
			#endif

				float passWeight = (currentPass + 1) / _PassCount;
				output.positionWS += output.normalWS * ((_FurLength * passWeight - _FurShrink) * furMask);

				output.positionCS = TransformWorldToHClip(output.positionWS);
				output.uv.xy = input.uv;
				output.uv.zw = input.uv * _FurMapTiling;

			#if _DEPTH_FADE_ON
				output.positionSS = ComputeScreenPos(output.positionCS);
			#endif

				return output;
			}

			half3 TShift(half3 tangent, half3 normal, half bias)
			{   
				return normalize(tangent + bias * normal);
			}

			half HairSpecular(half3 T, half3 V, half3 L, half power)
			{
				// T:顺着发丝方向; V:实现方向; L:光线方向; exponent:各向异性锐度（越大高光越细）
				half3 H = normalize(L + V);                // 半角向量
				half TdotH = dot(T, H);                    // 发丝方向和半角的点积
				half sinTH = sqrt(1 - TdotH * TdotH);      // 垂直分量   s^2+c^2 = 1
				half dirAtten = smoothstep(-1, 0, TdotH);  // 方向衰减:光线和视线在发丝两侧
				return dirAtten * pow(sinTH, power);     // 最终高光计算
			}

			half4 FurFrag(Varyings input, bool isFrontFace : SV_IsFrontFace) : SV_TARGET
			{
				UNITY_SETUP_INSTANCE_ID(input);

				half4 baseColor = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv.xy) * _BaseColor * _Color;

				half3 albedo = baseColor.rgb;
				half alpha = baseColor.a;

			#if _NORMALMAP
				half3 normalTS = UnpackNormalScale(SAMPLE_TEXTURE2D(_NormalMap, sampler_NormalMap, input.uv.xy), _NormalScale);
				half3 normalWS = TransformTangentToWorld(normalTS, half3x3(input.tangentWS, input.bitangentWS, input.normalWS));
			#else
				half3 normalWS = input.normalWS;
			#endif

				normalWS = normalize(normalWS);

				half3 viewDirWS = SafeNormalize(GetWorldSpaceViewDir(input.positionWS));
				half NoV = saturate(dot(viewDirWS, normalWS));

			#if UNITY_ANY_INSTANCING_ENABLED
				float currentPass = UNITY_GET_INSTANCE_ID(input);
			#else
				float currentPass = 0;
			#endif

			#if _FUR_MASK
				half furMask = SAMPLE_TEXTURE2D(_FurMask, sampler_FurMask, input.uv.xy).r;
				clip(furMask - _FurMaskClip);
			#endif

				half furNoise = SAMPLE_TEXTURE2D(_FurMap, sampler_FurMap, input.uv.zw).r;
				float layerWeight = currentPass / (_PassCount + 1);
				float clipRate = pow(layerWeight, _FurSmoothness);
				float edgeFactor = pow(NoV, _FurOutsideFade);
				alpha *= edgeFactor;

				half layerAttenuation = lerp(_FurLayerAttenuation, 1, (currentPass + 1) / _PassCount);

				clip(furNoise - clipRate);

				float4 shadowCoord = TransformWorldToShadowCoord(input.positionWS);
				Light mainLight = GetMainLight(shadowCoord);

				half NoL = dot(normalWS, mainLight.direction);

				half3 H = normalize(viewDirWS + mainLight.direction);
				half NoH = saturate(dot(normalWS, H));

				half3 col = (NoL * 0.5 + 0.5) * layerAttenuation * mainLight.shadowAttenuation * albedo * mainLight.color;

				half rim = pow(1 - NoV + _RimPlus, _RimPower);
				col += _RimColor.rgb * rim;

				half3 shiftDir = TShift(lerp(input.bitangentWS, input.tangentWS, _Anisotropy), normalWS, _AnisotropyBias);
				float specular = HairSpecular(shiftDir, viewDirWS, mainLight.direction, _AnisotropyPower) * _AnisotropyStrength;
				col += mainLight.color * _SpecularColor.rgb * specular;

			#if _DEPTH_FADE_ON
				float2 screenUV = input.positionSS.xy / input.positionSS.w;
				float sceneDepth = LinearEyeDepth(SampleSceneDepth(screenUV), _ZBufferParams);
				float depth = LinearEyeDepth(input.positionCS.z, _ZBufferParams);
				alpha *= saturate((sceneDepth - depth) / _DepthFadeDistance);
			#endif

				return half4(col, alpha);
			}

			ENDHLSL
		}
	}

	CustomEditor "MultiPassFX.FurGUI"
}