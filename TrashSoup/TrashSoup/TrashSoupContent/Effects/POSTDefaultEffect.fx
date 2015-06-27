#define WEIGHT_0 1.0f
#define WEIGHT_1 0.9f
#define WEIGHT_2 0.55f
#define WEIGHT_3 0.18f
#define WEIGHT_4 0.1f

float4x4 WorldViewProj;

float ScreenWidth;
float ScreenHeight;

texture ScreenTexture;
sampler ScreenSampler = sampler_state
{
	texture = <ScreenTexture>;
	MipFilter = Point;
	MinFilter = Point;
	MagFilter = Point;
	AddressU = clamp;
	AddressV = clamp;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    output.Position = mul(input.Position, WorldViewProj);
	output.TexCoord = input.TexCoord;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 color = tex2D(ScreenSampler, input.TexCoord);
	color.r *= 1.0f;
	color.g *= 1.0f;
	color.b *= 1.0f;

    return color;
}

technique Main
{
	pass Pass1
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
}
