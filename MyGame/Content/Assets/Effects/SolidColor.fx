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
float4 SolidColor = float4(1, 0, 0, 1); // Default: Red
float AlphaThreshold = 0; // Pixels with alpha below this are considered transparent


// Pixel Shader
float4 MainPS(VertexShaderOutput input) : COLOR
{
    float4 texColor = tex2D(SpriteTextureSampler, input.TextureCoordinates);
    
    // If pixel alpha is above threshold, replace with solid color
    if (texColor.a > AlphaThreshold)
        return SolidColor;
    
    return texColor;
}


technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}