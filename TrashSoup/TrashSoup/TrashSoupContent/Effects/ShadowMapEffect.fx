#define POINT_MAX_LIGHTS_PER_OBJECT 10
#define MINIMUM_LENGTH_VALUE 0.00000000001f
#define ATTENUATION_MULTIPLIER 8

float4x4 World;
float4x4 WorldViewProj;

float4 BoundingFrustum[4];
float4 CustomClippingPlane;

struct VertexShaderInput
{
    float4 Position : POSITION0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float4 Position2D : TEXCOORD0;
	float4 ClipPlanes : TEXCOORD1;
	float CustomClipPlane : TEXCOORD2;
};


VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

	output.Position = mul(input.Position, WorldViewProj);
	output.Position2D = output.Position;
	float4 positionWS = mul(input.Position, World);

	output.ClipPlanes.x = dot(positionWS, BoundingFrustum[0]);
	output.ClipPlanes.y = dot(positionWS, BoundingFrustum[1]);
	output.ClipPlanes.z = dot(positionWS, BoundingFrustum[2]);
	output.ClipPlanes.w = dot(positionWS, BoundingFrustum[3]);
	output.CustomClipPlane = dot(positionWS, CustomClippingPlane);

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
	float4 color = (input.Position2D.z / (input.Position2D.w));

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
