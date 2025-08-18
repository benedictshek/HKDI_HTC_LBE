using System;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace MultiPassFX
{
	public class CloudGUI : ShaderGUI
	{
		static readonly string _EDGE_DITHER = nameof(_EDGE_DITHER);

		MaterialProperty _CloudNoiseTex;
		MaterialProperty _CloudNoiseUVParams;
		MaterialProperty _CloudSmoothness;
		MaterialProperty _CloudLength;
		MaterialProperty _CloudShrink;
		MaterialProperty _CloudLayerAttenuation;
		MaterialProperty _DitherOn;
		MaterialProperty _DitherTex;
		MaterialProperty _DitherStrength;
		MaterialProperty _BaseMap;
		MaterialProperty _BaseColor;
		MaterialProperty _Color;
		MaterialProperty _ShadowColor;
		MaterialProperty _BackLayerEnhance;
		MaterialProperty _BackSSSRange;
		MaterialProperty _BackSSSIntensity;
		MaterialProperty _DepthFadeOn;
		MaterialProperty _DepthFadeDistance;
		MaterialProperty _CloudOutsideFade;
		MaterialProperty _Surface;
		MaterialProperty _SrcBlend;
		MaterialProperty _DstBlend;
		MaterialProperty _SrcBlendAlpha;
		MaterialProperty _DstBlendAlpha;
		MaterialProperty _ZWrite;
		MaterialProperty _Cull;

		void FindProperties(MaterialProperty[] properties)
		{
			_CloudNoiseTex = EditorUtils.FindMaterialProperty(nameof(_CloudNoiseTex), properties);
			_CloudNoiseUVParams = EditorUtils.FindMaterialProperty(nameof(_CloudNoiseUVParams), properties);
			_CloudSmoothness = EditorUtils.FindMaterialProperty(nameof(_CloudSmoothness), properties);
			_CloudLength = EditorUtils.FindMaterialProperty(nameof(_CloudLength), properties);
			_CloudShrink = EditorUtils.FindMaterialProperty(nameof(_CloudShrink), properties);
			_CloudLayerAttenuation = EditorUtils.FindMaterialProperty(nameof(_CloudLayerAttenuation), properties);
			_DitherOn = EditorUtils.FindMaterialProperty(nameof(_DitherOn), properties);
			_DitherTex = EditorUtils.FindMaterialProperty(nameof(_DitherTex), properties);
			_DitherStrength = EditorUtils.FindMaterialProperty(nameof(_DitherStrength), properties);
			_BaseMap = EditorUtils.FindMaterialProperty(nameof(_BaseMap), properties);
			_BaseColor = EditorUtils.FindMaterialProperty(nameof(_BaseColor), properties);
			_Color = EditorUtils.FindMaterialProperty(nameof(_Color), properties);
			_ShadowColor = EditorUtils.FindMaterialProperty(nameof(_ShadowColor), properties);
			_BackLayerEnhance = EditorUtils.FindMaterialProperty(nameof(_BackLayerEnhance), properties);
			_BackSSSRange = EditorUtils.FindMaterialProperty(nameof(_BackSSSRange), properties);
			_BackSSSIntensity = EditorUtils.FindMaterialProperty(nameof(_BackSSSIntensity), properties);
			_DepthFadeOn = EditorUtils.FindMaterialProperty(nameof(_DepthFadeOn), properties);
			_DepthFadeDistance = EditorUtils.FindMaterialProperty(nameof(_DepthFadeDistance), properties);
			_CloudOutsideFade = EditorUtils.FindMaterialProperty(nameof(_CloudOutsideFade), properties);
			_Surface = EditorUtils.FindMaterialProperty(nameof(_Surface), properties);
			_SrcBlend = EditorUtils.FindMaterialProperty(nameof(_SrcBlend), properties);
			_DstBlend = EditorUtils.FindMaterialProperty(nameof(_DstBlend), properties);
			_SrcBlendAlpha = EditorUtils.FindMaterialProperty(nameof(_SrcBlendAlpha), properties);
			_DstBlendAlpha = EditorUtils.FindMaterialProperty(nameof(_DstBlendAlpha), properties);
			_ZWrite = EditorUtils.FindMaterialProperty(nameof(_ZWrite), properties);
			_Cull = EditorUtils.FindMaterialProperty(nameof(_Cull), properties);
		}

		SavedBool blendFoldout;
		SavedBool shapeFoldout;
		SavedBool colorFoldout;
		SavedBool otherFoldout;

		bool firstTimeOpen = true;

		void FirstTimeOpen()
		{
			blendFoldout = new SavedBool("MultiPassFX.CloudGUI.BlendFoldout", true);
			shapeFoldout = new SavedBool("MultiPassFX.CloudGUI.ShapeFoldout", true);
			colorFoldout = new SavedBool("MultiPassFX.CloudGUI.ColorFoldout", true);
			otherFoldout = new SavedBool("MultiPassFX.CloudGUI.OtherFoldout", true);
		}

        public override void ValidateMaterial(Material material)
        {
			if (material.GetTexture(nameof(_DitherTex)) != null)
				material.EnableKeyword(_EDGE_DITHER);
			else
				material.DisableKeyword(_EDGE_DITHER);
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
				materialEditor.ShaderProperty(_CloudOutsideFade, "Cloud Outside Fade");
				GUI.enabled = true;

				EditorGUILayout.Space();
				EditorGUILayout.EndFoldoutHeaderGroup();
			}

			if (EditorUtils.BeginFoldoutHeaderGroup(shapeFoldout, "Shape Settings"))
			{
				materialEditor.ShaderProperty(_CloudNoiseTex, "Shape 3D Texture");

				var uvParams = _CloudNoiseUVParams.vectorValue;
				Vector3 uvSpeed = uvParams;
				float uvTiling = uvParams.w;

				EditorGUI.BeginChangeCheck();
				uvSpeed = EditorGUILayout.Vector3Field("Shape UV Speed", uvSpeed);
				uvTiling = EditorGUILayout.FloatField("Shape Tiling", uvTiling);
				if (EditorGUI.EndChangeCheck())
                {
					_CloudNoiseUVParams.vectorValue = new Vector4(uvSpeed.x, uvSpeed.y, uvSpeed.z, uvTiling);
                }

				EditorGUILayout.Space();

				materialEditor.ShaderProperty(_CloudSmoothness, "Cloud Tip Softness");
				materialEditor.ShaderProperty(_CloudLength, "Cloud Total Expand Length");
				materialEditor.ShaderProperty(_CloudShrink, "Cloud Shrink");

				EditorGUILayout.Space();

				materialEditor.ShaderProperty(_DitherTex, "Dither Texture");

				EditorGUILayout.HelpBox("Set Blue Noise Texture To Dither Edge", MessageType.Info);

				if (_DitherTex.textureValue != null)
				{
					materialEditor.ShaderProperty(_DitherStrength, "Dither Strength");
				}

				EditorGUILayout.Space();
				EditorGUILayout.EndFoldoutHeaderGroup();
			}

			if (EditorUtils.BeginFoldoutHeaderGroup(colorFoldout, "Color Settings"))
			{
				materialEditor.ShaderProperty(_BaseMap, "Base Map");
				materialEditor.ShaderProperty(_BaseColor, "Base Color");
				materialEditor.ShaderProperty(_ShadowColor, "Shadow Color");
				materialEditor.ShaderProperty(_CloudLayerAttenuation, "Depth Color Atteunation");
				materialEditor.ShaderProperty(_BackLayerEnhance, "Back Face Lighting");
				materialEditor.ShaderProperty(_BackSSSRange, "Back Face Lighting Range");
				materialEditor.ShaderProperty(_BackSSSIntensity, "Back Face Light Intensity");

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