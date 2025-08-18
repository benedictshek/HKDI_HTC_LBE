using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MultiPassFX
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(SkinnedMeshRenderer))]
    [ExecuteAlways]
    public class MultiPassSkinnedMeshRenderer : MonoBehaviour
    {
        [Range(1, 100)]
        public int passCount = 10;

        public bool useBakeMesh = false;

        [SerializeField]
        private List<DrawMaterialPair> drawMaterialPairs = new List<DrawMaterialPair>();

        private SkinnedMeshRenderer skinnedMeshRenderer;

        private Matrix4x4[] matrices = new Matrix4x4[0];

        private Mesh bakeMesh;

        private GraphicsBuffer vertexBuffer;

        private void OnEnable()
        {
            if (skinnedMeshRenderer == null)
                skinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        }

        private void ReleaseBakeMesh()
        {
            CoreUtils.Destroy(bakeMesh);
            bakeMesh = null;
        }

        private void ReleaseVertexBuffer()
        {
            if (vertexBuffer != null)
                vertexBuffer.Dispose();

            vertexBuffer = null;
        }

        private void OnDisable()
        {
            ReleaseBakeMesh();
            ReleaseVertexBuffer();
        }

        private void LateUpdate()
        {
            var sharedMesh = skinnedMeshRenderer.sharedMesh;
            if (sharedMesh == null)
                return;

            var subMeshCount = sharedMesh.subMeshCount;

            if (matrices.Length != passCount)
                matrices = new Matrix4x4[passCount];

            var useVertexBuffer = !Utils.ForceBakeMesh && !useBakeMesh;

            if (useVertexBuffer)
            {
                ReleaseBakeMesh();
                if (vertexBuffer == null)
                {
                    skinnedMeshRenderer.vertexBufferTarget |= GraphicsBuffer.Target.Raw;
                    vertexBuffer = skinnedMeshRenderer.GetVertexBuffer();
                }

                var rootBone = skinnedMeshRenderer.rootBone == null ? transform : skinnedMeshRenderer.rootBone;

                var position = rootBone.position;
                var rotation = rootBone.rotation;
                var scale = Vector3.one;
                var matrix = Matrix4x4.TRS(position, rotation, scale);

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

                    if (pair.subMeshIndex < 0 || pair.subMeshIndex >= subMeshCount)
                    {
                        Debug.LogWarning($"[MultiPassFX] subMeshIndex invalid !");
                        continue;
                    }

                    if (pair.block == null)
                        pair.block = new MaterialPropertyBlock();

                    pair.block.SetFloat(ShaderProperties._PassCount, passCount);
                    pair.block.SetBuffer(ShaderProperties._VertexBuffer, vertexBuffer);
                    pair.drawMaterial.EnableKeyword(ShaderKeywords._ENABLE_SKINNED_MESH_BUFFER);

                    var renderParams = new RenderParams(pair.drawMaterial)
                    {
                        layer = gameObject.layer,
                        receiveShadows = skinnedMeshRenderer.receiveShadows,
                        rendererPriority = skinnedMeshRenderer.rendererPriority,
                        renderingLayerMask = skinnedMeshRenderer.renderingLayerMask,
                        lightProbeUsage = skinnedMeshRenderer.lightProbeUsage,
                        reflectionProbeUsage = skinnedMeshRenderer.reflectionProbeUsage,
                        shadowCastingMode = skinnedMeshRenderer.shadowCastingMode,
                        worldBounds = skinnedMeshRenderer.bounds,
                        matProps = pair.block,
                    };

                    Graphics.RenderMeshInstanced(renderParams, sharedMesh, pair.subMeshIndex, matrices);
                }
            }
            else
            {
                ReleaseVertexBuffer();
                if (bakeMesh == null)
                    bakeMesh = new Mesh();

                skinnedMeshRenderer.BakeMesh(bakeMesh, true);

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

                    if (pair.subMeshIndex < 0 || pair.subMeshIndex >= subMeshCount)
                    {
                        Debug.LogWarning($"[MultiPassFX] subMeshIndex invalid !");
                        continue;
                    }

                    if (pair.block == null)
                        pair.block = new MaterialPropertyBlock();

                    pair.block.SetFloat(ShaderProperties._PassCount, passCount);
                    pair.drawMaterial.DisableKeyword(ShaderKeywords._ENABLE_SKINNED_MESH_BUFFER);

                    var renderParams = new RenderParams(pair.drawMaterial)
                    {
                        layer = gameObject.layer,
                        receiveShadows = skinnedMeshRenderer.receiveShadows,
                        rendererPriority = skinnedMeshRenderer.rendererPriority,
                        renderingLayerMask = skinnedMeshRenderer.renderingLayerMask,
                        lightProbeUsage = skinnedMeshRenderer.lightProbeUsage,
                        reflectionProbeUsage = skinnedMeshRenderer.reflectionProbeUsage,
                        shadowCastingMode = skinnedMeshRenderer.shadowCastingMode,
                        worldBounds = skinnedMeshRenderer.bounds,
                        matProps = pair.block,
                    };

                    Graphics.RenderMeshInstanced(renderParams, bakeMesh, pair.subMeshIndex, matrices);
                }
            }
        }
    }
}