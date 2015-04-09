#define POINT_MAX_LIGHTS_PER_OBJECT 10
#define MINIMUM_LENGTH_VALUE 0.00000000001f
#define ATTENUATION_MULTIPLIER 8

float4x4 World;
float4x4 WorldViewProj;
float4x4 WorldInverseTranspose;

float3 AmbientLightColor;

float3 DirLight0Direction;
float3 DirLight0DiffuseColor;
float3 DirLight0SpecularColor;
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

texture DiffuseMap;
sampler DiffuseSampler = sampler_state
{
	texture = <DiffuseMap>;
	MipFilter = Linear;
	MinFilter = Linear;
	MagFilter = Linear;
};

texture NormalMap;
sampler NormalSampler = sampler_state
{
	texture = <NormalMap>;
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

ColorPair ComputeLight(float3 posWS, float3 E, float3 N)
{
	E = normalize(E);
	N = normalize(N);

	ColorPair result;
	ColorPair temp;

	result.Diffuse = AmbientLightColor;
	result.Specular = 0;
	temp.Diffuse = 0;
	temp.Specular = 0;

	// DirLight0
	ComputeSingleLight(-DirLight0Direction, DirLight0DiffuseColor, DirLight0SpecularColor, E, N, result);

	// DirLight1
	ComputeSingleLight(-DirLight1Direction, DirLight1DiffuseColor, DirLight1SpecularColor, E, N, result);

	// DirLight2
	ComputeSingleLight(-DirLight2Direction, DirLight2DiffuseColor, DirLight2SpecularColor, E, N, result);

	// point lights
	float3 L;
	float Llength;
	float att;
	for (uint i = 0; i < PointLightCount; ++i)
	{
		L = PointLightPositions[i] - posWS;
		Llength = length(L);
		ComputeSingleLight(normalize(L), PointLightDiffuseColors[i], PointLightSpecularColors[i], E, N, temp);

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

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 color = tex2D(DiffuseSampler, input.TexCoord);
	float alpha = color.a;
	color.a = 1.0f;

	// computin normals

	float3 nAdj = (tex2D(NormalSampler, input.TexCoord)).xyz;
	input.Normal = normalize(input.Normal);

	nAdj.x = (nAdj.x * 2) - 1;
	nAdj.y = (nAdj.y * 2) - 1;
	nAdj.z = (nAdj.z * 2) - 1;

	input.Normal = input.Normal + nAdj;
	input.Normal = normalize(input.Normal);

	////////

	ColorPair computedLight = ComputeLight(input.PositionWS.xyz, EyePosition - input.PositionWS.xyz, input.Normal);

	color = color * float4(computedLight.Diffuse, 1.0f) + alpha * float4(computedLight.Specular, 1.0f);

	color *= Transparency;

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
