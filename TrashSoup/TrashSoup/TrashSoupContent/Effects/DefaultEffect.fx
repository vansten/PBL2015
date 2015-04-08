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
};

struct ColorPair
{
	float3 Diffuse;
	float3 Specular;
};

ColorPair ComputeLight(float3 E, float3 N)
{
	ColorPair result;

	result.Diffuse = AmbientLightColor;
	result.Specular = 0;

	// DirLight0
	float3 L = -DirLight0Direction;
	float3 H = normalize(E + L);
	float intensity = max(dot(L, N), 0.0f);
	float3 color = DirLight0DiffuseColor;
	float3 specularColor = DirLight0SpecularColor;
	float3 specular = pow(max(0.0f, dot(H, N)), Glossiness) * length(specularColor);

	result.Diffuse += intensity * color;
	result.Specular += specular * specularColor * intensity;

	return result;
}

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

	output.Position = mul(input.Position, WorldViewProj);

	output.PositionWS = mul(input.Position, World);

	output.TexCoord = input.TexCoord;

	output.Normal = normalize(mul(input.Normal, World));

    return output;
}

float4 PixelShaderFunction(VertexShaderOutput input) : COLOR0
{
	float4 color = tex2D(DiffuseSampler, input.TexCoord);
	float alpha = color.a;
	color.a = 1.0f;

	ColorPair computedLight = ComputeLight(normalize(EyePosition - input.PositionWS.xyz), input.Normal);

	color = color * float4(computedLight.Diffuse, 1.0f) + alpha * float4(computedLight.Specular, 1.0f);

	color.a = Transparency;
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
