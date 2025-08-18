using UnityEngine;

namespace MultiPassFX
{
    [DisallowMultipleComponent]
    [RequireComponent(typeof(ParticleSystem), typeof(ParticleSystemRenderer))]
    [ExecuteAlways]
    public class MultiPassParticleRenderer : MonoBehaviour
    {
        [Range(1, 100)]
        public int passCount = 10;
        public Material drawMaterial;

        private ParticleSystem particle;
        private ParticleSystemRenderer particleRenderer;

        private ParticleSystem.Particle[] particles = new ParticleSystem.Particle[0];
        private Mesh[] meshes = new Mesh[0];
        private Matrix4x4[] matrices = new Matrix4x4[0];
        private MaterialPropertyBlock block;

        private void OnEnable()
        {
            if (particle == null)
                particle = GetComponent<ParticleSystem>();

            if (particleRenderer == null)
                particleRenderer = GetComponent<ParticleSystemRenderer>();

            particleRenderer.enabled = false;

            if (block == null)
                block = new MaterialPropertyBlock();
        }

        private void OnDisable()
        {
            particleRenderer.enabled = true;
        }

        private void LateUpdate()
        {
            if (particleRenderer.renderMode != ParticleSystemRenderMode.Mesh)
                return;

            if (drawMaterial == null)
                return;

            if (!drawMaterial.enableInstancing)
            {
                Debug.LogWarning($"[MultiPassFX] Plesase make sure material {drawMaterial.name} enable instancing is on");
                return;
            }

            var meshCount = particleRenderer.meshCount;
            if (meshCount != meshes.Length)
                meshes = new Mesh[meshCount];

            particleRenderer.GetMeshes(meshes);
            particleRenderer.GetPropertyBlock(block);

            var maxParticles = particle.main.maxParticles;
            if (particles.Length != maxParticles)
                particles = new ParticleSystem.Particle[maxParticles];

            var particleCount = particle.GetParticles(particles);
            if (particleCount < 1)
                return;

            if (matrices.Length != passCount)
                matrices = new Matrix4x4[passCount];

            var localToWorld = transform.localToWorldMatrix;
            block.SetFloat(ShaderProperties._PassCount, passCount);
            drawMaterial.DisableKeyword(ShaderKeywords._ENABLE_SKINNED_MESH_BUFFER);

            for (int i = 0; i < particleCount; ++i)
            {
                var p = particles[i];
                var meshIndex = p.GetMeshIndex(particle);
                var mesh = meshes[meshIndex];
                if (mesh == null)
                    continue;

                var scale = p.GetCurrentSize3D(particle);
                var position = p.position;
                var rotation = Quaternion.Euler(p.rotation3D);
                var matrix = localToWorld * Matrix4x4.TRS(position, rotation, scale);
                var color = p.GetCurrentColor(particle);

                for (int j = 0; j < passCount; ++j)
                {
                    matrices[j] = matrix;
                }

                block.SetColor(ShaderProperties._Color, color);

                var renderParams = new RenderParams(drawMaterial)
                {
                    layer = gameObject.layer,
                    worldBounds = TransformBounds(mesh.bounds, matrix),
                    rendererPriority = particleRenderer.rendererPriority,
                    matProps = block.isEmpty ? null : block
                };

                Graphics.RenderMeshInstanced(renderParams, mesh, 0, matrices);
            }
        }

        internal static Vector3[] m_Vertices = new Vector3[8];

        static Bounds TransformBounds(Bounds bounds, Matrix4x4 transform)
        {
            Vector3 boundsMin = bounds.min, boundsMax = bounds.max;
            m_Vertices[0] = new Vector3(boundsMin.x, boundsMin.y, boundsMin.z);
            m_Vertices[1] = new Vector3(boundsMax.x, boundsMin.y, boundsMin.z);
            m_Vertices[2] = new Vector3(boundsMax.x, boundsMax.y, boundsMin.z);
            m_Vertices[3] = new Vector3(boundsMin.x, boundsMax.y, boundsMin.z);
            m_Vertices[4] = new Vector3(boundsMin.x, boundsMin.y, boundsMax.z);
            m_Vertices[5] = new Vector3(boundsMax.x, boundsMin.y, boundsMax.z);
            m_Vertices[6] = new Vector3(boundsMax.x, boundsMax.y, boundsMax.z);
            m_Vertices[7] = new Vector3(boundsMin.x, boundsMax.y, boundsMax.z);

            Vector3 min = transform.MultiplyPoint(m_Vertices[0]);
            Vector3 max = min;

            for (int i = 1; i < 8; i++)
            {
                var point = transform.MultiplyPoint(m_Vertices[i]);
                min = Vector3.Min(min, point);
                max = Vector3.Max(max, point);
            }

            Bounds result = default;
            result.SetMinMax(min, max);
            return result;
        }
    }
}