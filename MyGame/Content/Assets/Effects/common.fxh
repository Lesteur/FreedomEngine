#ifndef COMMON
#define COMMON

// Defines the data structure coming from the C# application
struct VertexShaderInput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
    float2 TexCoord : TEXCOORD0; // Global UV coordinates (Atlas)
};

// Defines the data structure passed from the Vertex Shader to the Pixel Shader
struct VertexShaderOutput
{
    float4 Position : SV_POSITION;
    float4 Color : COLOR0;
    float2 TextureCoordinates : TEXCOORD0; // Global UV coordinates
};

#endif