float4x4 WorldViewProjection;
Texture theTexture;
float4 particleColor;

// TODO: add effect parameters here.
sampler ColoredTextureSampler = sampler_state
{
	texture = <theTexture>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = POINT;
	AddressU = Clamp;
	AddressV = Clamp;
};

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 textureCoordinates : TEXCOORD0;

    // TODO: add input channels such as texture
    // coordinates and vertex colors here.
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 textureCoordinates : TEXCOORD0;

    // TODO: add vertex shader outputs such as colors and texture
    // coordinates here. These values will automatically be interpolated
    // over the triangle, and provided as input to your pixel shader.
};

struct PixelShaderInput
{
	float2 textureCoordinates : TEXCOORD0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    output.Position = mul(input.Position, WorldViewProjection);
	output.textureCoordinates = input.textureCoordinates;
    // TODO: add your vertex shader code here.

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    // TODO: add your pixel shader code here.
	float4 color = tex2D(ColoredTextureSampler, input.textureCoordinates);
	color.r = particleColor.r * color.a;
	color.g = particleColor.g * color.a;
	color.b = particleColor.b * color.a;
    return color;
}

technique Technique1
{
    pass Pass1
    {
        // TODO: set renderstates here.

        VertexShader = compile vs_2_0 VertexShaderFunction();
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
