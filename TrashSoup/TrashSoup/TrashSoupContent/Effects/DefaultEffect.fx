#define POINT_MAX_LIGHTS_PER_OBJECT 10
#define MINIMUM_LENGTH_VALUE 0.00000000001f
#define ATTENUATION_MULTIPLIER 8
#define SKINNED_EFFECT_MAX_BONES 72
#define WEIGHTS_PER_VERTEX 4

float4x4 World;
float4x4 WorldViewProj;
float4x4 WorldInverseTranspose;

float3 AmbientLightColor;

float3 DirLight0Direction;
float3 DirLight0DiffuseColor;
float3 DirLight0SpecularColor;
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

float3 DirLight1Direction;
float3 DirLight1DiffuseColor;
float3 DirLight1SpecularColor;

float3 DirLight2Direction;
float3 DirLight2DiffuseColor;
float3 DirLight2SpecularColor;

float3 PointLightDiffuseColors[POINT_MAX_LIGHTS_PER_OBJECT];
float3 PointLightPositions[POINT_MAX_LIGHTS_PER_OBJECT];
float3 PointLightSpecularColors[POINT_MAX_LIGHTS_PER_OBJECT];
float PointLightAttenuations[POINT_MAX_LIGHTS_PER_OBJECT];
uint PointLightCount;

float4x3 Bones[SKINNED_EFFECT_MAX_BONES];

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

texture DiffuseMap;
sampler DiffuseSampler = sampler_state
{
	texture = <DiffuseMap>;
	MipFilter = Linear;
	MinFilter = Linear;
	MagFilter = Linear;
};

float3 EyePosition;

float3 SpecularColor;
float Glossiness;
float Transparency;
float3 padding01;

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float3 Normal : NORMAL;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float4 PositionWS : TEXCOORD2;
	float2 TexCoord : TEXCOORD0;
	float3 Normal : TEXCOORD1;
	float4 ClipPlanes : TEXCOORD3;
	float CustomClipPlane : TEXCOORD4;
	float4 PositionDLS : TEXCOORD5;
	float4 PositionPLS : TEXCOORD6;
};

struct ColorPair
{
	float3 Diffuse;
	float3 Specular;
};

inline void ComputeSingleLight(float3 L, float3 color, float3 specularColor, float3 E, float3 N, inout ColorPair pair)
{
	float3 H = normalize(normalize(E) + L);
	float intensity = max(dot(L, N), 0.0f);
	float3 specular = pow(max(0.0f, dot(H, N)), Glossiness) * length(specularColor);

	pair.Diffuse += intensity * color;
	pair.Specular += specular * specularColor * pair.Diffuse;
}

ColorPair ComputeLight(float3 posWS, float3 E, float3 N, float4 dirPos, float4 pointPos)
{
	E = normalize(E);
	N = normalize(N);

	ColorPair result;
	ColorPair temp;

	result.Diffuse = AmbientLightColor;
	result.Specular = 0;
	temp.Diffuse = 0;
	temp.Specular = 0;

	// shadows for DirLight0
	float2 projectedDLScoords;
	projectedDLScoords.x = (dirPos.x / dirPos.w) / 2.0f + 0.5f;
	projectedDLScoords.y = (-dirPos.y / dirPos.w) / 2.0f + 0.5f;

	float depth = tex2D(DirLight0ShadowMapSampler, projectedDLScoords).r;
	float dist = dirPos.z / dirPos.w;

	[branch]
	if ((saturate(projectedDLScoords.x) == projectedDLScoords.x) && (saturate(projectedDLScoords.y) == projectedDLScoords.y))
	{
		[branch]
		if ((dist - 0.005f) <= depth || depth <= 0.0001f)
		{
			// DirLight0
			ComputeSingleLight(-DirLight0Direction, DirLight0DiffuseColor,
				float3(DirLight0SpecularColor.x * SpecularColor.x, DirLight0SpecularColor.y * SpecularColor.y, DirLight0SpecularColor.z * SpecularColor.z), E, N, result);
		}
	}
	else
	{
		// DirLight0
		ComputeSingleLight(-DirLight0Direction, DirLight0DiffuseColor,
			float3(DirLight0SpecularColor.x * SpecularColor.x, DirLight0SpecularColor.y * SpecularColor.y, DirLight0SpecularColor.z * SpecularColor.z), E, N, result);
	}
	// DirLight1
	ComputeSingleLight(-DirLight1Direction, DirLight1DiffuseColor, 
		float3(DirLight1SpecularColor.x * SpecularColor.x, DirLight1SpecularColor.y * SpecularColor.y, DirLight1SpecularColor.z * SpecularColor.z), E, N, result);

	// DirLight2
	ComputeSingleLight(-DirLight2Direction, DirLight2DiffuseColor, 
		float3(DirLight2SpecularColor.x * SpecularColor.x, DirLight2SpecularColor.y * SpecularColor.y, DirLight2SpecularColor.z * SpecularColor.z), E, N, result);

	// point lights
	float3 L;
	float Llength;
	float att;
	for (uint i = 0; i < PointLightCount; ++i)
	{
		L = PointLightPositions[i] - posWS;
		Llength = length(L);
		ComputeSingleLight(normalize(L), PointLightDiffuseColors[i], 
			float3(PointLightSpecularColors[i].x * SpecularColor.x, PointLightSpecularColors[i].y * SpecularColor.y, PointLightSpecularColors[i].z * SpecularColor.z),
			E, N, temp);

		// shadows for point light 0 - TBA

		att = saturate(ATTENUATION_MULTIPLIER * length(PointLightDiffuseColors[i]) * PointLightAttenuations[i] / max(Llength * Llength, MINIMUM_LENGTH_VALUE));
		temp.Diffuse = temp.Diffuse * att;
		temp.Specular = temp.Specular * att;
		result.Diffuse += temp.Diffuse;
		result.Specular += temp.Specular;

		temp.Diffuse = 0;
		temp.Specular = 0;
	}


	return result;
}

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

	output.Position = mul(input.Position, WorldViewProj);

	output.PositionWS = mul(input.Position, World);

	output.TexCoord = input.TexCoord;

	output.Normal = normalize(mul(input.Normal, WorldInverseTranspose));

	output.ClipPlanes.x = dot(output.PositionWS, BoundingFrustum[0]);
	output.ClipPlanes.y = dot(output.PositionWS, BoundingFrustum[1]);
	output.ClipPlanes.z = dot(output.PositionWS, BoundingFrustum[2]);
	output.ClipPlanes.w = dot(output.PositionWS, BoundingFrustum[3]);
	output.CustomClipPlane = dot(output.PositionWS, CustomClippingPlane);

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

	float4 color = tex2D(DiffuseSampler, input.TexCoord);
	float alpha = color.a;
	color.a = 1.0f;

	ColorPair computedLight = ComputeLight(input.PositionWS.xyz, EyePosition - input.PositionWS.xyz, input.Normal, input.PositionDLS, input.PositionPLS);

	color = color * float4(computedLight.Diffuse, 1.0f) + alpha * float4(computedLight.Specular, 1.0f);

	color *= Transparency;
	//float2 projectedDLScoords;
	//projectedDLScoords.x = (input.PositionDLS.x / input.PositionDLS.w) / 2.0f + 0.5f;
	//projectedDLScoords.y = (-input.PositionDLS.y / input.PositionDLS.w) / 2.0f + 0.5f;
	//float4 color = tex2D(DirLight0ShadowMapSampler, projectedDLScoords);
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

struct VertexShaderInputSkinned
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float3 Normal : NORMAL;
	int4 Indices : BLENDINDICES0;
	float4 Weights : BLENDWEIGHT0;
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
	input.Normal = mul(input.Normal, (float3x3)skinning);
}

VertexShaderOutput VertexShaderFunctionSkinned(VertexShaderInputSkinned input)
{
	VertexShaderOutput output;

	Skin(input);

	output.Position = mul(input.Position, WorldViewProj);

	output.PositionWS = mul(input.Position, World);

	output.TexCoord = input.TexCoord;

	output.Normal = normalize(mul(input.Normal, WorldInverseTranspose));

	output.ClipPlanes.x = dot(output.PositionWS, BoundingFrustum[0]);
	output.ClipPlanes.y = dot(output.PositionWS, BoundingFrustum[1]);
	output.ClipPlanes.z = dot(output.PositionWS, BoundingFrustum[2]);
	output.ClipPlanes.w = dot(output.PositionWS, BoundingFrustum[3]);
	output.CustomClipPlane = dot(output.PositionWS, CustomClippingPlane);

	output.PositionDLS = mul(input.Position, DirLight0WorldViewProj);
	output.PositionPLS = mul(input.Position, Point0WorldViewProj);

	return output;
}


technique Skinned
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 VertexShaderFunctionSkinned();
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
}
