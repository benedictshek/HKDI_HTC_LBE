using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MultiPassFX
{
    public class DrawParams
    {
        public Material material = null;
        public Shader shader = null;
        public int subMeshIndex = -1;
        public bool isMultiPass = false;
        public MaterialPropertyBlock block = new MaterialPropertyBlock();

        public bool IsValid
        {
            get
            {
                return !isMultiPass || material.enableInstancing;
            }
        }
    }

    public class DrawParamsList : IEnumerable<DrawParams>, IDisposable
    {
        private List<Material> sharedMaterials1 = new List<Material>();
        private List<Material> sharedMaterials2 = new List<Material>();
        private bool swapFlag = false;
        private List<Material> sharedMaterials => swapFlag ? sharedMaterials1 : sharedMaterials2;
        private List<DrawParams> drawParamsList = new List<DrawParams>();
        public int Count => drawParamsList.Count;
        public DrawParams this[int index] => drawParamsList[index];

        public DrawParamsList() {}

        private void BuildDrawParams()
        {
            for (int i = 0; i < sharedMaterials.Count; ++i)
            {
                var material = sharedMaterials[i];
                if (material == null)
                    continue;

                var isMultiPass = !string.IsNullOrEmpty(material.GetTag("IsMultiPass", false));

                drawParamsList.Add(new DrawParams()
                {
                    material = material,
                    shader = material.shader,
                    subMeshIndex = i,
                    isMultiPass = isMultiPass,
                });
            }
        }

        private bool DrawParamsListIsDirty()
        {
            if (sharedMaterials1.Count != sharedMaterials2.Count)
                return true;

            for (int i = 0; i < sharedMaterials1.Count; ++i)
            {
                if (sharedMaterials1[i] != sharedMaterials2[i])
                    return true;
            }

            foreach (var drawParam in drawParamsList)
            {
                if (drawParam.subMeshIndex >= sharedMaterials.Count)
                    return true;

                if (drawParam.material != sharedMaterials[drawParam.subMeshIndex])
                    return true;

                if (drawParam.material.shader != drawParam.shader)
                    return true;

                return false;
            }

            return false;
        }

        public void Update(Renderer renderer)
        {
            swapFlag = !swapFlag;

            renderer.GetSharedMaterials(sharedMaterials);

            if (DrawParamsListIsDirty())
            {
                ReleaseDrawParams();
                BuildDrawParams();
            }

            foreach (var drawParam in drawParamsList)
            {
                renderer.GetPropertyBlock(drawParam.block, drawParam.subMeshIndex);
            }
        }

        private void ReleaseDrawParams()
        {
            drawParamsList.Clear();
        }

        public void Dispose()
        {
            sharedMaterials1.Clear();
            sharedMaterials2.Clear();
            ReleaseDrawParams();
        }

        public IEnumerator<DrawParams> GetEnumerator()
        {
            return drawParamsList.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return drawParamsList.GetEnumerator();
        }
    }

    [RequireComponent(typeof(Renderer))]
    [ExecuteAlways]
    public abstract class MultiPassRenderer : MonoBehaviour
    {
        public static readonly int _PassCount = Shader.PropertyToID(nameof(_PassCount));

        [Range(1, 100)]
        public int passCount = 10;

        protected abstract Renderer m_Renderer { get; }

        protected Matrix4x4[] m_Matrices = new Matrix4x4[0];

        protected DrawParamsList m_DrawParamsList;

        private void OnEnable()
        {
            m_DrawParamsList = new DrawParamsList();
            Init();
        }

        protected abstract void Init();

        private void OnDisable()
        {
            m_DrawParamsList.Dispose();
            Release();
        }

        protected abstract void Release();

        private void LateUpdate()
        {
            if (m_Matrices.Length != passCount)
                m_Matrices = new Matrix4x4[passCount];

            if (m_Renderer == null)
                return;

            m_DrawParamsList.Update(m_Renderer);
            if (m_DrawParamsList.Count < 1)
                return;

            Draw();
        }

        protected abstract void Draw();
    }
}