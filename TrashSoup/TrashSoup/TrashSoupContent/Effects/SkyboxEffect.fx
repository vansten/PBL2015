float4x4 World;
float4x4 WorldViewProj;
float4x4 WorldInverseTranspose;

float3 AmbientLightColor;

texture CubeMap;
samplerCUBE CubeSampler = sampler_state
{
	texture = <CubeMap>;
	MipFilter = Linear;
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = Mirror;
	AddressV = Mirror;
};

float3 EyePosition;

struct VertexShaderInput
{
    float4 Position : POSITION0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float3 TexCoord : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    output.Position = mul(input.Position, WorldViewProj);

	float4 vertPos = mul(input.Position, World);
	output.TexCoord = vertPos - EyePosition;

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 color = texCUBE(CubeSampler, normalize(input.TexCoord));
	color.a = 1;
    return color;
}

technique Main
{
    pass Pass1
    {
        VertexShader = compile vs_3_0 VertexShaderFunction();
        PixelShader = compile ps_3_0 PixelShaderFunction();
    }
}
