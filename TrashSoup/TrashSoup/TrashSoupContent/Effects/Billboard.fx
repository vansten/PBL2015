float4x4 World;
float4x4 View;
float4x4 Projection;
float3 CamPos;
float3 AllowedRotDir;

// TODO: add effect parameters here.
Texture BillboardTexture;
float4 particleColor;
sampler textureSampler = sampler_state
{
	texture = <BillboardTexture>;
	magfilter = LINEAR;
	minfilter = LINEAR;
	mipfilter = LINEAR;
	AddressU = CLAMP;
	AddressV = CLAMP;
};

struct VertexShaderInput
{
    float4 Position : POSITION;
	float2 TexCoord : TEXCOORD0;

    // TODO: add input channels such as texture
    // coordinates and vertex colors here.
};

struct VertexShaderOutput
{
    float4 Position : POSITION;
	float2 TexCoord : TEXCOORD0;

    // TODO: add vertex shader outputs such as colors and texture
    // coordinates here. These values will automatically be interpolated
    // over the triangle, and provided as input to your pixel shader.
};

struct PixelShaderOutput
{
	float4 Color : COLOR0;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

	float3 center = mul(input.Position, World);
	float3 eyeVector = center - CamPos;

	float3 upVector = AllowedRotDir;
	upVector = normalize(upVector);
	float3 sideVector = cross(eyeVector, upVector);
	sideVector = normalize(sideVector);

	float3 finalPosition = center;
	finalPosition += (input.TexCoord.x - 0.5f) * sideVector;
	finalPosition += (1.5f - input.TexCoord.y*1.5f) * upVector;

	float4 finalPosition4 = float4(finalPosition, 1);

	float4x4 preViewProjection = mul(View, Projection);
	output.Position = mul(finalPosition4, preViewProjection);

	output.TexCoord = input.TexCoord;
    // TODO: add your vertex shader code here.

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
    // TODO: add your pixel shader code here.
	float4 output;
	output = tex2D(textureSampler, input.TexCoord);
	output.r = particleColor.r * output.a;
	output.g = particleColor.g * output.a;
	output.b = particleColor.b * output.a;
	return output;
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
