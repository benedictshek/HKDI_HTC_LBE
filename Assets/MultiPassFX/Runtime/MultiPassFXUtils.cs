using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

namespace MultiPassFX
{
    [Serializable]
    public class DrawMaterialPair
    {
        public Material drawMaterial = null;
        public int subMeshIndex = 0;
        public MaterialPropertyBlock block = null;
    }

    public class SharedMaterialsCache : IDisposable
    {
        private Renderer renderer;
        private List<Material> sharedMaterials1 = new List<Material>();
        private List<Material> sharedMaterials2 = new List<Material>();
        private List<MaterialPropertyBlock> propertyBlocks = new List<MaterialPropertyBlock>();
        private bool swapFlag = false;

        private List<Material> sharedMaterials => swapFlag ? sharedMaterials1 : sharedMaterials2;

        public int Count => sharedMaterials.Count;

        public Material GetSharedMaterial(int index) => sharedMaterials[index];

        public MaterialPropertyBlock GetPropertyBlock(int index) => propertyBlocks[index];

        public SharedMaterialsCache(Renderer r)
        {
            renderer = r;
            renderer.GetSharedMaterials(sharedMaterials1);
            renderer.GetSharedMaterials(sharedMaterials2);
            EnsurePropertyBlocks();
        }

        private bool CheckIsDirty()
        {
            if (sharedMaterials1.Count != sharedMaterials2.Count)
                return true;

            for (int i = 0; i < Count; ++i)
            {
                if (sharedMaterials1[i] != sharedMaterials2[i])
                    return true;
            }

            return false;
        }

        private void EnsurePropertyBlocks()
        {
            if (sharedMaterials.Count != propertyBlocks.Count)
            {
                propertyBlocks.Clear();
                for (int i = 0; i < sharedMaterials.Count; ++i)
                {
                    propertyBlocks.Add(new MaterialPropertyBlock());
                }
            }
        }

        public void Update()
        {
            swapFlag = !swapFlag;
            renderer.GetSharedMaterials(sharedMaterials);

            EnsurePropertyBlocks();

            for (int i = 0; i < sharedMaterials.Count; ++i)
                renderer.GetPropertyBlock(propertyBlocks[i]);

            if (CheckIsDirty())
                onSharedMaterialsChange?.Invoke();
        }

        public void Dispose()
        {
            sharedMaterials1.Clear();
            sharedMaterials2.Clear();
            propertyBlocks.Clear();
        }

        public delegate void SharedMaterialsChangeDelegate();

        public SharedMaterialsChangeDelegate onSharedMaterialsChange = null;
    }

    public static class ShaderProperties
    {
        public static readonly int _PassCount = Shader.PropertyToID(nameof(_PassCount));
        public static readonly int _VertexBuffer = Shader.PropertyToID(nameof(_VertexBuffer));
        public static readonly int _Color = Shader.PropertyToID(nameof(_Color));
    }

    public static class ShaderKeywords
    {
        public static readonly string _ENABLE_SKINNED_MESH_BUFFER = nameof(_ENABLE_SKINNED_MESH_BUFFER);
    }

    public static class Utils
    {
        public static readonly bool ForceBakeMesh = SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES2 ||
            (SystemInfo.graphicsDeviceType == GraphicsDeviceType.OpenGLES3 && Graphics.minOpenGLESVersion < OpenGLESVersion.OpenGLES31AEP);
    }
}