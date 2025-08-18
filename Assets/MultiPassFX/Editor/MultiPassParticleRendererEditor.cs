using UnityEngine;
using UnityEditor;

namespace MultiPassFX
{
    [CustomEditor(typeof(MultiPassParticleRenderer))]
    public class MultiPassParticleRendererEditor : Editor
    {
        SerializedProperty passCount;
        SerializedProperty drawMaterial;
        Editor drawMaterialEditor;
        MultiPassParticleRenderer renderer => target as MultiPassParticleRenderer;
        ParticleSystemRenderer particleRenderer => renderer.GetComponent<ParticleSystemRenderer>();
        Material particleSharedMaterial => particleRenderer.sharedMaterial;

        private void OnEnable()
        {
            passCount = serializedObject.FindProperty(nameof(passCount));
            drawMaterial = serializedObject.FindProperty(nameof(drawMaterial));
            drawMaterialEditor = CreateEditor(drawMaterial.objectReferenceValue);
        }

        public override void OnInspectorGUI()
        {
            if (drawMaterialEditor == null || drawMaterialEditor.target != drawMaterial.objectReferenceValue)
                OnEnable();

            serializedObject.Update();

            EditorGUILayout.PropertyField(passCount);
            EditorGUILayout.PropertyField(drawMaterial);

            if (drawMaterialEditor != null && particleSharedMaterial != drawMaterial.objectReferenceValue)
            {
                EditorGUILayout.Space();

                drawMaterialEditor.DrawHeader();
                drawMaterialEditor.OnInspectorGUI();
            }

            serializedObject.ApplyModifiedProperties();
        }
    }
}