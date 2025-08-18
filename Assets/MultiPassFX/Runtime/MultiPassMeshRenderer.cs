using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MultiPassFX
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(MeshRenderer), typeof(MeshFilter))]
    [ExecuteAlways]
    public class MultiPassMeshRenderer : MonoBehaviour
    {
        [Range(5, 100)]
        public int passCount = 10;
        [SerializeField]
        private List<DrawMaterialPair> drawMaterialPairs = new List<DrawMaterialPair>();

        private MeshRenderer meshRenderer;
        private MeshFilter meshFilter;
        private Matrix4x4[] matrices = new Matrix4x4[0];

        private void OnEnable()
        {
            if (meshRenderer == null)
                meshRenderer = GetComponent<MeshRenderer>();

            if (meshFilter == null)
                meshFilter = GetComponent<MeshFilter>();
        }

        private void LateUpdate()
        {
            var mesh = meshFilter.sharedMesh;
            if (mesh == null)
                return;

            var subMeshCount = mesh.subMeshCount;

            if (matrices.Length != passCount)
                matrices = new Matrix4x4[passCount];

            var matrix = transform.localToWorldMatrix;
            for (int i = 0; i < passCount; ++i)
            {
                matrices[i] = matrix;
            }

            foreach (var pair in drawMaterialPairs)
            {
                if (pair.drawMaterial == null)
                    continue;

                if (!pair.drawMaterial.enableInstancing)
                {
                    Debug.LogWarning($"[MultiPassFX] Plesase make sure material {pair.drawMaterial.name} enable instancing is on");
                    continue;
                }

                var subMeshIndex = Mathf.Clamp(pair.subMeshIndex, 0, subMeshCount - 1);

                if (pair.block == null)
                    pair.block = new MaterialPropertyBlock();

                pair.block.SetFloat(ShaderProperties._PassCount, passCount);
                pair.drawMaterial.DisableKeyword(ShaderKeywords._ENABLE_SKINNED_MESH_BUFFER);

                var renderParams = new RenderParams(pair.drawMaterial)
                {
                    layer = gameObject.layer,
                    receiveShadows = meshRenderer.receiveShadows,
                    rendererPriority = meshRenderer.rendererPriority,
                    renderingLayerMask = meshRenderer.renderingLayerMask,
                    lightProbeUsage = meshRenderer.lightProbeUsage,
                    reflectionProbeUsage = meshRenderer.reflectionProbeUsage,
                    shadowCastingMode = meshRenderer.shadowCastingMode,
                    worldBounds = meshRenderer.bounds,
                    matProps = pair.block,
                };

                Graphics.RenderMeshInstanced(renderParams, mesh, subMeshIndex, matrices);
            }
        }
    }
}