using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.Rendering;

namespace MultiPassFX
{
    public class FurGUI : ShaderGUI
    {
		static readonly string _FUR_MASK = nameof(_FUR_MASK);

		MaterialProperty _BaseMap;
		MaterialProperty _BaseColor;
		MaterialProperty _NormalMap;
		MaterialProperty _NormalScale;
		MaterialProperty _FurMap;
		MaterialProperty _FurMapTiling;
		MaterialProperty _FurMask;
		MaterialProperty _FurMaskClip;
		MaterialProperty _FurSmoothness;
		MaterialProperty _FurLength;
		MaterialProperty _FurShrink;
		MaterialProperty _FurLayerAttenuation;
		MaterialProperty _RimColor;
		MaterialProperty _RimPower;
		MaterialProperty _RimPlus;
		MaterialProperty _SpecularColor;
		MaterialProperty _Anisotropy;
		MaterialProperty _AnisotropyBias;
		MaterialProperty _AnisotropyPower;
		MaterialProperty _AnisotropyStrength;
		MaterialProperty _DepthFadeOn;
		MaterialProperty _DepthFadeDistance;
		MaterialProperty _FurOutsideFade;
		MaterialProperty _Surface;
		MaterialProperty _SrcBlend;
		MaterialProperty _DstBlend;
		MaterialProperty _SrcBlendAlpha;
		MaterialProperty _DstBlendAlpha;
		MaterialProperty _ZWrite;
		MaterialProperty _Cull;


		void FindProperties(MaterialProperty[] properties)
		{
			_BaseMap = EditorUtils.FindMaterialProperty(nameof(_BaseMap), properties);
			_BaseColor = EditorUtils.FindMaterialProperty(nameof(_BaseColor), properties);
			_NormalMap = EditorUtils.FindMaterialProperty(nameof(_NormalMap), properties);
			_NormalScale = EditorUtils.FindMaterialProperty(nameof(_NormalScale), properties);
			_FurMap = EditorUtils.FindMaterialProperty(nameof(_FurMap), properties);
			_FurMapTiling = EditorUtils.FindMaterialProperty(nameof(_FurMapTiling), properties);
			_FurMask = EditorUtils.FindMaterialProperty(nameof(_FurMask), properties);
			_FurMaskClip = EditorUtils.FindMaterialProperty(nameof(_FurMaskClip), properties);
			_FurSmoothness = EditorUtils.FindMaterialProperty(nameof(_FurSmoothness), properties);
			_FurLength = EditorUtils.FindMaterialProperty(nameof(_FurLength), properties);
			_FurShrink = EditorUtils.FindMaterialProperty(nameof(_FurShrink), properties);
			_FurLayerAttenuation = EditorUtils.FindMaterialProperty(nameof(_FurLayerAttenuation), properties);
			_RimColor = EditorUtils.FindMaterialProperty(nameof(_RimColor), properties);
			_RimPower = EditorUtils.FindMaterialProperty(nameof(_RimPower), properties);
			_RimPlus = EditorUtils.FindMaterialProperty(nameof(_RimPlus), properties);
			_SpecularColor = EditorUtils.FindMaterialProperty(nameof(_SpecularColor), properties);
			_Anisotropy = EditorUtils.FindMaterialProperty(nameof(_Anisotropy), properties);
			_AnisotropyBias = EditorUtils.FindMaterialProperty(nameof(_AnisotropyBias), properties);
			_AnisotropyPower = EditorUtils.FindMaterialProperty(nameof(_AnisotropyPower), properties);
			_AnisotropyStrength = EditorUtils.FindMaterialProperty(nameof(_AnisotropyStrength), properties);
			_DepthFadeOn = EditorUtils.FindMaterialProperty(nameof(_DepthFadeOn), properties);
			_DepthFadeDistance = EditorUtils.FindMaterialProperty(nameof(_DepthFadeDistance), properties);
			_FurOutsideFade = EditorUtils.FindMaterialProperty(nameof(_FurOutsideFade), properties);
			_Surface = EditorUtils.FindMaterialProperty(nameof(_Surface), properties);
			_SrcBlend = EditorUtils.FindMaterialProperty(nameof(_SrcBlend), properties);
			_DstBlend = EditorUtils.FindMaterialProperty(nameof(_DstBlend), properties);
			_SrcBlendAlpha = EditorUtils.FindMaterialProperty(nameof(_SrcBlendAlpha), properties);
			_DstBlendAlpha = EditorUtils.FindMaterialProperty(nameof(_DstBlendAlpha), properties);
			_ZWrite = EditorUtils.FindMaterialProperty(nameof(_ZWrite), properties);
			_Cull = EditorUtils.FindMaterialProperty(nameof(_Cull), properties);
		}

		SavedBool blendFoldout;
		SavedBool baseFoldout;
		SavedBool furFoldout;
		SavedBool colorFoldout;
		SavedBool otherFoldout;

		bool firstTimeOpen = true;

		void FirstTimeOpen()
		{
			blendFoldout = new SavedBool("MultiPassFX.FurGUI.BlendFoldout", true);
			baseFoldout = new SavedBool("MultiPassFX.FurGUI.BaseFoldout", true);
			furFoldout = new SavedBool("MultiPassFX.FurGUI.FurFoldout", true);
			colorFoldout = new SavedBool("MultiPassFX.FurGUI.ColorFoldout", true);
			otherFoldout = new SavedBool("MultiPassFX.FurGUI.OtherFoldout", true);
		}

        public override void ValidateMaterial(Material material)
        {
			if (material.GetTexture(nameof(_FurMask)) != null)
				material.EnableKeyword(_FUR_MASK);
			else
				material.DisableKeyword(_FUR_MASK);
        }


        public override void OnGUI(MaterialEditor materialEditor, MaterialProperty[] properties)
		{
			if (firstTimeOpen)
			{
				FirstTimeOpen();
				firstTimeOpen = false;
			}

			FindProperties(properties);

			if (EditorUtils.BeginFoldoutHeaderGroup(blendFoldout, "Blend Settings"))
			{
				EditorGUILayout.BeginHorizontal();

				var isOpaque = _Surface.floatValue < 0.5f;

				var originColor = GUI.color;

				GUI.color = isOpaque ? Color.green : Color.gray;
				if (GUILayout.Button("Opaque", EditorStyles.miniButton))
				{
					_Surface.floatValue = 0.0f;
					_SrcBlend.floatValue = (float)BlendMode.One;
					_DstBlend.floatValue = (float)BlendMode.Zero;
					_SrcBlendAlpha.floatValue = (float)BlendMode.One;
					_DstBlendAlpha.floatValue = (float)BlendMode.Zero;
					_ZWrite.floatValue = 1.0f;
				}

				GUI.color = isOpaque ? Color.gray : Color.green;
				if (GUILayout.Button("Transparent", EditorStyles.miniButton))
				{
					_Surface.floatValue = 1.0f;
					_SrcBlend.floatValue = (float)BlendMode.SrcAlpha;
					_DstBlend.floatValue = (float)BlendMode.OneMinusSrcAlpha;
					_SrcBlendAlpha.floatValue = (float)BlendMode.One;
					_DstBlendAlpha.floatValue = (float)BlendMode.OneMinusSrcAlpha;
					_ZWrite.floatValue = 0.0f;
				}

				GUI.color = originColor;

				EditorGUILayout.EndHorizontal();

				EditorGUILayout.Space();
				materialEditor.ShaderProperty(_ZWrite, "ZWrite");
				materialEditor.ShaderProperty(_Cull, "Cull");
				EditorGUILayout.Space();

				GUI.enabled = !isOpaque;
				materialEditor.ShaderProperty(_DepthFadeOn, "Depth Fade");
				materialEditor.ShaderProperty(_DepthFadeDistance, "Depth Fade Distance");
				materialEditor.ShaderProperty(_FurOutsideFade, "Fur Outside Fade");
				GUI.enabled = true;

				EditorGUILayout.Space();
				EditorGUILayout.EndFoldoutHeaderGroup();
			}

			if (EditorUtils.BeginFoldoutHeaderGroup(baseFoldout, "Base Settings"))
			{
				materialEditor.TexturePropertySingleLine(new GUIContent("Base Map"), _BaseMap, _BaseColor);
				EditorGUILayout.Space();
				materialEditor.TexturePropertySingleLine(new GUIContent("Normal Map"), _NormalMap, _NormalScale);
				EditorGUILayout.Space();
				EditorGUILayout.EndFoldoutHeaderGroup();
			}


			if (EditorUtils.BeginFoldoutHeaderGroup(furFoldout, "Fur Settings"))
			{
				materialEditor.TextureProperty(_FurMap, "Fur Map");
				materialEditor.ShaderProperty(_FurMapTiling, "Fur Map Tiling");

				EditorGUILayout.Space();

				materialEditor.TextureProperty(_FurMask, "Fur Mask");
				materialEditor.ShaderProperty(_FurMaskClip, "Fur Mask Clip");

				EditorGUILayout.Space();

				materialEditor.ShaderProperty(_FurSmoothness, "Fur Tip Softness");
				materialEditor.ShaderProperty(_FurLength, "Fur Total Lenght");
				materialEditor.ShaderProperty(_FurShrink, "Fur Shrink");

				EditorGUILayout.Space();
				EditorGUILayout.EndFoldoutHeaderGroup();
			}

			if (EditorUtils.BeginFoldoutHeaderGroup(colorFoldout, "Color Settings"))
            {
				materialEditor.ShaderProperty(_FurLayerAttenuation, "Fur Color LayerAttenuation");

				EditorGUILayout.Space();
				materialEditor.ShaderProperty(_RimColor, "Rim Color");
				materialEditor.ShaderProperty(_RimPower, "Rim Power");
				materialEditor.ShaderProperty(_RimPlus, "Rim Plus");

				EditorGUILayout.Space();
				materialEditor.ShaderProperty(_SpecularColor, "Specular Color");
				materialEditor.ShaderProperty(_Anisotropy, "Specular Direction Vertical");
				materialEditor.ShaderProperty(_AnisotropyBias, "Anisotropy Bias");
				materialEditor.ShaderProperty(_AnisotropyPower, "Specular Focus");
				materialEditor.ShaderProperty(_AnisotropyStrength, "Specular Strength");

				EditorGUILayout.Space();
				EditorGUILayout.EndFoldoutHeaderGroup();
			}

			if (EditorUtils.BeginFoldoutHeaderGroup(otherFoldout, "Other Settings"))
			{
				materialEditor.RenderQueueField();
				materialEditor.EnableInstancingField();

				EditorGUILayout.Space();
				EditorGUILayout.EndFoldoutHeaderGroup();
			}
		}
	}
}