#define POINT_MAX_LIGHTS_PER_OBJECT 10
#define MINIMUM_LENGTH_VALUE 0.00000000001f
#define ATTENUATION_MULTIPLIER 8
#define SKINNED_EFFECT_MAX_BONES 72
#define WEIGHTS_PER_VERTEX 4

float4x4 World;
float4x4 WorldViewProj;
float4x4 DirLight0WorldViewProj;
texture DirLight0ShadowMap;
sampler DirLight0ShadowMapSampler = sampler_state
{
	texture = <DirLight0ShadowMap>;
	MipFilter = Linear;
	MinFilter = Linear;
	MagFilter = Linear;
	AddressU = clamp;
	AddressV = clamp;
};

float4x4 Point0WorldViewProj;
textureCUBE Point0ShadowMap;
samplerCUBE Point0ShadowMapSampler = sampler_state
{
	texture = <Point0ShadowMap>;
	MipFilter = Linear;
	MinFilter = Linear;
	MagFilter = Linear;
};

float4 BoundingFrustum[4];
float4 CustomClippingPlane;

float4x3 Bones[SKINNED_EFFECT_MAX_BONES];

struct VertexShaderInput
{
    float4 Position : POSITION0;
};

struct VertexShaderInputSkinned
{
	float4 Position : POSITION0;
	int4 Indices : BLENDINDICES0;
	float4 Weights : BLENDWEIGHT0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float4 Position2D : TEXCOORD0;
	float4 ClipPlanes : TEXCOORD1;
	float CustomClipPlane : TEXCOORD2;
	float4 PositionDLS : TEXCOORD5;
	float4 PositionPLS : TEXCOORD6;
};

inline void Skin(inout VertexShaderInputSkinned input)
{
	float4x3 skinning = 0;

		[unroll]
	for (int i = 0; i < WEIGHTS_PER_VERTEX; ++i)
	{
		skinning += Bones[input.Indices[i]] * input.Weights[i];
	}

	input.Position.xyz = mul(input.Position, skinning);
}

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

	output.PositionDLS = mul(input.Position, DirLight0WorldViewProj);
	output.PositionPLS = mul(input.Position, Point0WorldViewProj);

    return output;
}

VertexShaderOutput VertexShaderFunctionSkinned(VertexShaderInputSkinned input)
{
	VertexShaderOutput output;

	Skin(input);

	output.Position = mul(input.Position, WorldViewProj);
	output.Position2D = output.Position;
	float4 positionWS = mul(input.Position, World);

		output.ClipPlanes.x = dot(positionWS, BoundingFrustum[0]);
	output.ClipPlanes.y = dot(positionWS, BoundingFrustum[1]);
	output.ClipPlanes.z = dot(positionWS, BoundingFrustum[2]);
	output.ClipPlanes.w = dot(positionWS, BoundingFrustum[3]);
	output.CustomClipPlane = dot(positionWS, CustomClippingPlane);

	output.PositionDLS = mul(input.Position, DirLight0WorldViewProj);
	output.PositionPLS = mul(input.Position, Point0WorldViewProj);

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
	
	float2 projectedDLScoords;
	projectedDLScoords.x = (input.PositionDLS.x / input.PositionDLS.w) / 2.0f + 0.5f;
	projectedDLScoords.y = (-input.PositionDLS.y / input.PositionDLS.w) / 2.0f + 0.5f;
	float depth = tex2D(DirLight0ShadowMapSampler, projectedDLScoords).r;
	float dist = input.PositionDLS.z / input.PositionDLS.w;

	float4 color = float4(0.0f, 0.0f, 0.0f, 1.0f);

	[branch]
	if ((saturate(projectedDLScoords.x) == projectedDLScoords.x) && (saturate(projectedDLScoords.y) == projectedDLScoords.y))
	{
		[branch]
		if ((dist - 0.005f) <= depth || depth <= 0.0001f)
		{
			color.xyzw = 1.0f;
		}
	}
	else
	{
		color.xyzw = 1.0f;
	}

	return color;
}

technique Main
{
	pass Shadow
	{
		VertexShader = compile vs_3_0 VertexShaderFunction();
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
};

technique Skinned
{
	pass Shadow
	{
		VertexShader = compile vs_3_0 VertexShaderFunctionSkinned();
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
};
