#if OPENGL
#define SV_POSITION POSITION
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


// Wave effect parameters
float Time = 0;
float WaveSpeed = 5.0; // Speed of the wave animation
float WaveFrequency = 15.0; // Frequency of the wave (number of waves across the texture)
float WaveAmplitude = 0.01; // Amplitude of the wave (how much it distorts the texture)


float4 MainPS(VertexShaderOutput input) : COLOR
{
    float2 texCoord = input.TextureCoordinates;
    
    // Apply wave distortion to the x-coordinate based on the y-coordinate and time
    texCoord.x += sin((texCoord.y * WaveFrequency) + (Time * WaveSpeed)) * WaveAmplitude;
    return tex2D(SpriteTextureSampler, texCoord) * input.Color;
}


technique WaveDrawing
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL MainPS();
    }
};