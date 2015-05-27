float4x4 World;
float4x4 WorldViewProj;
float4x4 WorldInverseTranspose;

float3 AmbientLightColor;

float4 BoundingFrustum[4];
float4 CustomClippingPlane;

texture DiffuseMap;
sampler DiffuseSampler = sampler_state
{
	texture = <DiffuseMap>;
	MipFilter = Linear;
	MinFilter = Linear;
	MagFilter = Linear;
};

float3 DiffuseColor;
float3 EyePosition;

float2 Size;
float3 CameraUp;
float3 CameraRight;

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float4 ClipPlanes : TEXCOORD1;
	float CustomClipPlane : TEXCOORD2;
};

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

	float3 position = input.Position.xyz;
	/*float2 offset = float2((input.TexCoord.x - 0.5f) * 2.0f,
							-(input.TexCoord.y - 0.5f) * 2.0f);

	position += offset.x * Size.x * CameraRight + offset.y * Size.y * CameraUp;*/

	float3 vertPos = mul(float4(position, 1), World);
    output.Position = mul(float4(position, 1), WorldViewProj);

	output.TexCoord = input.TexCoord;



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

	float4 color = float4(DiffuseColor, 1.0f) * tex2D(DiffuseSampler, input.TexCoord);
	color.a = 1.0f;
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
