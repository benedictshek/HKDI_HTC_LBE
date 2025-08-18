using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace MultiPassFX
{
    [CustomEditor(typeof(MultiPassMeshRenderer))]
    public class MultiPassMeshRendererEditor : Editor
    {
        SerializedProperty passCount;
        SerializedProperty drawMaterialPairs;
        MultiPassMeshRenderer renderer => target as MultiPassMeshRenderer;
        MeshRenderer meshRenderer => renderer.GetComponent<MeshRenderer>();
        MeshFilter meshFilter => renderer.GetComponent<MeshFilter>();
        Mesh mesh => meshFilter.sharedMesh;

        List<DrawMaterialPair> drawMaterialPairsMirror = new List<DrawMaterialPair>();
        List<Material> drawMaterialsList = new List<Material>();
        List<Editor> drawMaterialEditors = new List<Editor>();

        private void OnEnable()
        {
            passCount = serializedObject.FindProperty(nameof(passCount));
            drawMaterialPairs = serializedObject.FindProperty(nameof(drawMaterialPairs));
        }

        private void OnDisable()
        {
            drawMaterialPairsMirror.Clear();
            drawMaterialsList.Clear();
            drawMaterialEditors.Clear();
        }

        void UpdateDrawMaterialPairsMirror()
        {
            drawMaterialPairsMirror.Clear();
            drawMaterialsList.Clear();

            var arraySize = drawMaterialPairs.arraySize;

            for (int i = arraySize - 1; i >= 0; --i)
            {
                var pair = drawMaterialPairs.GetArrayElementAtIndex(i);
                var mat = pair.FindPropertyRelative("drawMaterial").objectReferenceValue as Material;
                var index = pair.FindPropertyRelative("subMeshIndex").intValue;
                if (index >= mesh.subMeshCount)
                    drawMaterialPairs.DeleteArrayElementAtIndex(i);
                else
                    drawMaterialPairsMirror.Add(new DrawMaterialPair() { drawMaterial = mat, subMeshIndex = index });

                if (mat != null && !drawMaterialsList.Contains(mat))
                    drawMaterialsList.Add(mat);
            }

            if (drawMaterialEditors.Count < drawMaterialsList.Count)
            {
                for (int i = drawMaterialEditors.Count; i < drawMaterialsList.Count; ++i)
                    drawMaterialEditors.Add(null);
            }
            else if (drawMaterialEditors.Count > drawMaterialsList.Count)
            {
                drawMaterialEditors.RemoveRange(drawMaterialsList.Count, drawMaterialEditors.Count - drawMaterialsList.Count);
            }

            for (int i = 0; i < drawMaterialsList.Count; ++i)
            {
                var editor = drawMaterialEditors[i];
                CreateCachedEditor(drawMaterialsList[i], null, ref editor);
                drawMaterialEditors[i] = editor;
            }
        }

        void ApplyDrawMaterialPairsMirror()
        {
            if (drawMaterialPairsMirror.Count != drawMaterialPairs.arraySize)
                drawMaterialPairs.arraySize = drawMaterialPairsMirror.Count;

            for (int i = 0; i < drawMaterialPairs.arraySize; ++i)
            {
                var pair = drawMaterialPairs.GetArrayElementAtIndex(i);
                pair.FindPropertyRelative("drawMaterial").objectReferenceValue = drawMaterialPairsMirror[i].drawMaterial;
                pair.FindPropertyRelative("subMeshIndex").intValue = drawMaterialPairsMirror[i].subMeshIndex;
            }
        }

        public override void OnInspectorGUI()
        {
            serializedObject.Update();

            EditorGUILayout.PropertyField(passCount);

            EditorGUILayout.Space();

            if (mesh == null)
            {
                EditorGUILayout.HelpBox("Please assign mesh to MeshFilter", MessageType.Error);
                return;
            }

            EditorGUILayout.ObjectField("Mesh: ", mesh, typeof(Mesh), false);

            EditorGUILayout.LabelField($"SubMesh: {mesh.subMeshCount}");

            EditorGUILayout.Space();
            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField("Draw Materials", EditorStyles.boldLabel);

            if (GUILayout.Button("Clear All", GUILayout.Width(80)))
            {
                drawMaterialPairs.ClearArray();
            }

            EditorGUILayout.EndHorizontal();

            UpdateDrawMaterialPairsMirror();

            for (int i = 0; i < mesh.subMeshCount; ++i)
            {
                DrawMaterialPair matchPair = null;

                for (int j = 0; j < drawMaterialPairsMirror.Count; ++j)
                {
                    var pair = drawMaterialPairsMirror[j];
                    if (pair.subMeshIndex == i)
                    {
                        matchPair = pair;
                        break;
                    }
                }

                var overrideMat = matchPair == null ? null : matchPair.drawMaterial;

                var newOverrideMat = (Material)EditorGUILayout.ObjectField($"SubMesh {i} Draw Material: ", overrideMat, typeof(Material), false);

                if (newOverrideMat != overrideMat)
                {
                    if (newOverrideMat == null)
                        drawMaterialPairsMirror.Remove(matchPair);
                    else if (overrideMat == null)
                        drawMaterialPairsMirror.Add(new DrawMaterialPair() { drawMaterial = newOverrideMat, subMeshIndex = i });
                    else
                        matchPair.drawMaterial = newOverrideMat;
                }
            }

            EditorGUILayout.Space();

            foreach (var matEditor in drawMaterialEditors)
            {
                if (matEditor == null)
                    continue;

                matEditor.DrawHeader();
                matEditor.OnInspectorGUI();
            }

            ApplyDrawMaterialPairsMirror();

            serializedObject.ApplyModifiedProperties();
        }
    }
}