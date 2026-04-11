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

float2 LightPosition;
float Radius;
float Intensity;
float2 RoomSize;

// Pixel Shader
float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 texCoord = input.TextureCoordinates;
    
    // Map the texture coordinate to room boundaries to get spatial pixel positions
    float2 pixelPos = texCoord * RoomSize;
    
    // Calculate distance from current pixel to the light origin
    float dist = distance(pixelPos, LightPosition);
    
    // Smoothstep creates a nice soft circular falloff, reaching 0 at 'Radius'
    float light = smoothstep(Radius, 0.0, dist);
    
    // Multiply by the light's intensity
    light *= Intensity;
    
    // Sample the base texture color and vertex color
    float4 baseColor = tex2D(SpriteTextureSampler, texCoord) * input.Color;
    
    // Multiply the RGB channels by the light, and preserve the original alpha.
    // Far away pixels (where light is 0) will output float3(0,0,0), which renders as black.
    return float4(baseColor.rgb * light, baseColor.a);
}

technique SpriteDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
}