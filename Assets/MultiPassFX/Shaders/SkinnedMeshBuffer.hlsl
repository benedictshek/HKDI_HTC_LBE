#ifndef _SKINNED_MESH_BUFFER_
#define _SKINNED_MESH_BUFFER_

#include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"

#if _ENABLE_SKINNED_MESH_BUFFER
#define SKINNED_MESH_VERTEX_INPUT float4 positionOS : POSITION; float3 normalOS : NORMAL; float4 tangentOS : TANGENT; uint vertexID : SV_VertexID;
#else
#define SKINNED_MESH_VERTEX_INPUT float4 positionOS : POSITION; float3 normalOS : NORMAL; float4 tangentOS : TANGENT;
#endif

ByteAddressBuffer _VertexBuffer;
// float3 position
// float3 normal
// float4 tangent
// (3 + 3 + 4) * 10 = 40
#define VERTEX_DATA_STRIDE 40
#define NORMAL_OFFSET 12
#define TANGENT_OFFSET 24

float3 GetPositionOS(uint vertexID)
{
	float3 data = asfloat(_VertexBuffer.Load3(vertexID * VERTEX_DATA_STRIDE));
	return data;
}

float3 GetNormalOS(uint vertexID)
{
	float3 data = asfloat(_VertexBuffer.Load3(vertexID * VERTEX_DATA_STRIDE + NORMAL_OFFSET));
	return data;
}

float4 GetTangentOS(uint vertexID)
{
	float4 data = asfloat(_VertexBuffer.Load4(vertexID * VERTEX_DATA_STRIDE + TANGENT_OFFSET));
	return data;
}

void GetSkinnedMeshBuffer(uint vertexID, inout float3 positionOS)
{
	positionOS = GetPositionOS(vertexID);
}

void GetSkinnedMeshBuffer(uint vertexID, inout float3 positionOS, inout float3 normalOS)
{
	positionOS = GetPositionOS(vertexID);
	normalOS = GetNormalOS(vertexID);
}

void GetSkinnedMeshBuffer(uint vertexID, inout float3 positionOS, inout float3 normalOS, inout float3 tangentOS)
{
	positionOS = GetPositionOS(vertexID);
	normalOS = GetNormalOS(vertexID);
	tangentOS = GetTangentOS(vertexID).xyz;
}

#if _ENABLE_SKINNED_MESH_BUFFER
#define SETUP_SKINNED_MESH_INPUT(input) { GetSkinnedMeshBuffer(input.vertexID, input.positionOS.xyz, input.normalOS, input.tangentOS.xyz); }
#else
#define SETUP_SKINNED_MESH_INPUT(input)
#endif

#endif