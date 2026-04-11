#if OPENGL
#define VS_SHADERMODEL vs_3_0
#define PS_SHADERMODEL ps_3_0
#else
#define VS_SHADERMODEL vs_4_0_level_9_1
#define PS_SHADERMODEL ps_4_0_level_9_1
#endif

Texture2D SpriteTexture;
sampler2D SpriteTextureSampler = sampler_state
{
    Texture = <SpriteTexture>;
};

#include "common.fxh"


// Custom parameters
float4 Color1 = float4(1, 0, 0, 1); // Default: Red
float4 Color2 = float4(0, 0, 1, 1); // Default: Blue


// Pixel Shader
float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 texCoord = input.TextureCoordinates;
    
    // Interpolate between Color1 and Color2 based on the y-coordinate
    float4 gradientColor = lerp(Color1, Color2, texCoord.y);
    return gradientColor * input.Color;
}


technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}