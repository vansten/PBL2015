float4x4 World;
float4x4 WorldViewProj;
float4x4 WorldInverseTranspose;

float3 AmbientLightColor;

float4 BoundingFrustum[4];
float4 CustomClippingPlane;

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
	float4 ClipPlanes : TEXCOORD1;
	float CustomClipPlane : TEXCOORD2;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

    output.Position = mul(input.Position, WorldViewProj);

	float4 vertPos = mul(input.Position, World);
	output.TexCoord = vertPos - EyePosition;

	output.ClipPlanes.x = dot(vertPos, BoundingFrustum[0]);
	output.ClipPlanes.y = dot(vertPos, BoundingFrustum[1]);
	output.ClipPlanes.z = dot(vertPos, BoundingFrustum[2]);
	output.ClipPlanes.w = dot(vertPos, BoundingFrustum[3]);
	output.CustomClipPlane = dot(vertPos, CustomClippingPlane);

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	// clippin

	clip(input.ClipPlanes.x);
	clip(input.ClipPlanes.y);
	clip(input.ClipPlanes.z);
	clip(input.ClipPlanes.w);
	clip(input.CustomClipPlane);

	//////

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
