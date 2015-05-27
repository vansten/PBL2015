#include "Constants.fxh"

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

float3 SpecularColor;
float Glossiness;
float3 ReflectivityColor;
float ReflectivityBias;
float Transparency;
float3 padding01;

struct VertexShaderInput
{
    float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float3 Normal : NORMAL;
};

struct VertexShaderInputSkinned
{
	float4 Position : POSITION0;
	float2 TexCoord : TEXCOORD0;
	float3 Normal : NORMAL;
	int4 Indices : BLENDINDICES0;
	float4 Weights : BLENDWEIGHT0;
};

struct VertexShaderOutput
{
	float4 Position : POSITION0;
	float4 PositionWS : TEXCOORD2;
	float2 TexCoord : TEXCOORD0;
	float3 Normal : TEXCOORD1;
	float3 Reflection : TEXCOORD3;
	float4 ClipPlanes : TEXCOORD4;
	float CustomClipPlane : TEXCOORD5;
};

struct VertexShaderOutputShadows
{
	float4 Position : POSITION0;
	float4 PositionWS : TEXCOORD2;
	float2 TexCoord : TEXCOORD0;
	float3 Normal : TEXCOORD1;
	float3 Reflection : TEXCOORD3;
	float4 ClipPlanes : TEXCOORD4;
	float CustomClipPlane : TEXCOORD5;
	float4 PositionDLS : TEXCOORD6;
	float4 PositionProj : TEXCOORD8;
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
	float3 specular = pow(max(0.0000001f, dot(H, N)), Glossiness) * length(specularColor);

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
	ComputeSingleLight(-DirLight0Direction, DirLight0DiffuseColor,
		float3(DirLight0SpecularColor.x * SpecularColor.x, DirLight0SpecularColor.y * SpecularColor.y, DirLight0SpecularColor.z * SpecularColor.z), E, N, result);

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

ColorPair ComputeLightShadows(float3 posWS, float3 E, float3 N, float4 dirPos)
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
	projectedDLScoords.x = clamp((dirPos.x / dirPos.w) / 2.0f + 0.5f, 0.1f, 0.9f);
	projectedDLScoords.y = clamp((-dirPos.y / dirPos.w) / 2.0f + 0.5f, 0.1f, 0.9f);

	float depth = tex2D(DirLight0ShadowMapSampler, projectedDLScoords).r;
	float dist = dirPos.z / dirPos.w;

	[branch]
	if (depth < 0.001f || depth > 0.8f)
		depth = 1000000000.0f;

	// DirLight0
	ComputeSingleLight(-DirLight0Direction, DirLight0DiffuseColor,
		float3(DirLight0SpecularColor.x * SpecularColor.x, DirLight0SpecularColor.y * SpecularColor.y, DirLight0SpecularColor.z * SpecularColor.z), E, N, result);

	float shadow = saturate(exp(max(ESM_MIN, ESM_K * (depth - (dist - SHADOW_BIAS)))));
	shadow = 1.0f - (ESM_DIFFUSE_SCALE * (1.0f - shadow));
	result.Diffuse = lerp(AmbientLightColor, result.Diffuse, saturate(shadow));
	result.Specular = result.Specular * shadow;
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

	if (PointLightCount < 1) return result;

	// point light 01

	L = PointLightPositions[0] - posWS;
	//L.z = -L.z;
	Llength = length(L);
	att = saturate(ATTENUATION_MULTIPLIER * length(PointLightDiffuseColors[0]) * PointLightAttenuations[0] / max(Llength * Llength, MINIMUM_LENGTH_VALUE));

	float shadowMapDepth = texCUBE(Point0ShadowMapSampler, normalize(-(float3(L.x, L.y, -L.z) * att))).r;

	ComputeSingleLight(normalize(L), PointLightDiffuseColors[0],
		float3(PointLightSpecularColors[0].x * SpecularColor.x, PointLightSpecularColors[0].y * SpecularColor.y, PointLightSpecularColors[0].z * SpecularColor.z),
		E, N, temp);

	float shadowP = saturate(exp(max(ESM_MIN, ESM_K * (shadowMapDepth - (Llength / SHADOW_POINT_MAX_DIST - SHADOW_BIAS)))));
	shadowP = 1.0f - (ESM_DIFFUSE_SCALE * (1.0f - shadowP));
	temp.Diffuse = temp.Diffuse * att;
	temp.Diffuse = lerp(0.0f, temp.Diffuse, saturate(shadowP));
	temp.Specular = temp.Specular * att;
	result.Diffuse += temp.Diffuse;
	result.Specular += temp.Specular;

	temp.Diffuse = 0;
	temp.Specular = 0;

	for (uint i = 1; i < PointLightCount; ++i)
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

ColorPair ComputeLightBlurredShadows(float3 posWS, float3 E, float3 N, float2 coords)
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
	ComputeSingleLight(-DirLight0Direction, DirLight0DiffuseColor,
		float3(DirLight0SpecularColor.x * SpecularColor.x, DirLight0SpecularColor.y * SpecularColor.y, DirLight0SpecularColor.z * SpecularColor.z), E, N, result);

	// computin shadows
	float3 shadowCol = tex2D(DirLight0ShadowMapSampler, coords).xyz;

		result.Diffuse *= shadowCol.r + AmbientLightColor;
	result.Specular *= shadowCol.r;

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

	if (PointLightCount < 1) return result;

	// point light 01

	L = PointLightPositions[0] - posWS;
	//L.z = -L.z;
	Llength = length(L);
	att = saturate(ATTENUATION_MULTIPLIER * length(PointLightDiffuseColors[0]) * PointLightAttenuations[0] / max(Llength * Llength, MINIMUM_LENGTH_VALUE));

	ComputeSingleLight(normalize(L), PointLightDiffuseColors[0],
		float3(PointLightSpecularColors[0].x * SpecularColor.x, PointLightSpecularColors[0].y * SpecularColor.y, PointLightSpecularColors[0].z * SpecularColor.z),
		E, N, temp);

	temp.Diffuse = temp.Diffuse * att * shadowCol.g;
	temp.Specular = temp.Specular * att * shadowCol.g;
	result.Diffuse += temp.Diffuse;
	result.Specular += temp.Specular;

	temp.Diffuse = 0;
	temp.Specular = 0;

	for (uint i = 1; i < PointLightCount; ++i)
	{
		L = PointLightPositions[i] - posWS;
		Llength = length(L);
		ComputeSingleLight(normalize(L), PointLightDiffuseColors[i],
			float3(PointLightSpecularColors[i].x * SpecularColor.x, PointLightSpecularColors[i].y * SpecularColor.y, PointLightSpecularColors[i].z * SpecularColor.z),
			E, N, temp);

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

inline float3 PrawieNormalize(in float3 arg)
{
	return (arg * 0.9999f) / length(arg);
}

VertexShaderOutput VertexShaderFunction(VertexShaderInput input)
{
    VertexShaderOutput output;

	output.Position = mul(input.Position, WorldViewProj);

	output.PositionWS = mul(input.Position, World);

	output.TexCoord = input.TexCoord;

	output.Normal = normalize(mul(input.Normal, WorldInverseTranspose));

	output.Reflection = reflect(PrawieNormalize(output.PositionWS.xyz - EyePosition), PrawieNormalize(output.Normal));

	output.ClipPlanes.x = dot(output.PositionWS, BoundingFrustum[0]);
	output.ClipPlanes.y = dot(output.PositionWS, BoundingFrustum[1]);
	output.ClipPlanes.z = dot(output.PositionWS, BoundingFrustum[2]);
	output.ClipPlanes.w = dot(output.PositionWS, BoundingFrustum[3]);
	output.CustomClipPlane = dot(output.PositionWS, CustomClippingPlane);

    return output;
}

VertexShaderOutput VertexShaderFunctionSkinned(VertexShaderInputSkinned input)
{
	VertexShaderOutput output;

	Skin(input);

	output.Position = mul(input.Position, WorldViewProj);

	output.PositionWS = mul(input.Position, World);

	output.TexCoord = input.TexCoord;

	output.Normal = normalize(mul(input.Normal, WorldInverseTranspose));

	output.Reflection = reflect(PrawieNormalize(output.PositionWS.xyz - EyePosition), PrawieNormalize(output.Normal));

	output.ClipPlanes.x = dot(output.PositionWS, BoundingFrustum[0]);
	output.ClipPlanes.y = dot(output.PositionWS, BoundingFrustum[1]);
	output.ClipPlanes.z = dot(output.PositionWS, BoundingFrustum[2]);
	output.ClipPlanes.w = dot(output.PositionWS, BoundingFrustum[3]);
	output.CustomClipPlane = dot(output.PositionWS, CustomClippingPlane);

	return output;
}

VertexShaderOutputShadows VertexShaderFunctionShadows(VertexShaderInput input)
{
	VertexShaderOutputShadows output;

	output.Position = mul(input.Position, WorldViewProj);

	output.PositionWS = mul(input.Position, World);

	output.TexCoord = input.TexCoord;

	output.Normal = normalize(mul(input.Normal, WorldInverseTranspose));

	output.Reflection = reflect(PrawieNormalize(output.PositionWS.xyz - EyePosition), PrawieNormalize(output.Normal));

	output.ClipPlanes.x = dot(output.PositionWS, BoundingFrustum[0]);
	output.ClipPlanes.y = dot(output.PositionWS, BoundingFrustum[1]);
	output.ClipPlanes.z = dot(output.PositionWS, BoundingFrustum[2]);
	output.ClipPlanes.w = dot(output.PositionWS, BoundingFrustum[3]);
	output.CustomClipPlane = dot(output.PositionWS, CustomClippingPlane);

	output.PositionDLS = mul(input.Position, DirLight0WorldViewProj);

	output.PositionProj = output.Position;

	return output;
}

VertexShaderOutputShadows VertexShaderFunctionSkinnedShadows(VertexShaderInputSkinned input)
{
	VertexShaderOutputShadows output;

	Skin(input);

	output.Position = mul(input.Position, WorldViewProj);

	output.PositionWS = mul(input.Position, World);

	output.TexCoord = input.TexCoord;

	output.Normal = normalize(mul(input.Normal, WorldInverseTranspose));

	output.Reflection = reflect(PrawieNormalize(output.PositionWS.xyz - EyePosition), PrawieNormalize(output.Normal));

	output.ClipPlanes.x = dot(output.PositionWS, BoundingFrustum[0]);
	output.ClipPlanes.y = dot(output.PositionWS, BoundingFrustum[1]);
	output.ClipPlanes.z = dot(output.PositionWS, BoundingFrustum[2]);
	output.ClipPlanes.w = dot(output.PositionWS, BoundingFrustum[3]);
	output.CustomClipPlane = dot(output.PositionWS, CustomClippingPlane);

	output.PositionDLS = mul(input.Position, DirLight0WorldViewProj);

	output.PositionProj = output.Position;

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

	// computin normals

	float3 nAdj = (tex2D(NormalSampler, input.TexCoord)).xyz;
	input.Normal = normalize(input.Normal);

	nAdj.x = (nAdj.x * 2) - 1;
	nAdj.y = (nAdj.y * 2) - 1;
	nAdj.z = (nAdj.z) - 1;

	input.Normal = input.Normal + nAdj;
	input.Normal = normalize(input.Normal);

	////////

	// computin cube

	float3 reflection = (texCUBE(CubeSampler, normalize(input.Reflection))).xyz;
	reflection = reflection * ReflectivityColor;

	////////

	ColorPair computedLight = ComputeLight(input.PositionWS.xyz, EyePosition - input.PositionWS.xyz, input.Normal);

	color =  (color * float4(computedLight.Diffuse, 1.0f) + alpha * float4(computedLight.Specular, 1.0f)) + 
		ReflectivityBias * float4(computedLight.Diffuse, 1.0f) * (alpha * float4(reflection, 1.0f));

	color *= Transparency;

    return color;
}

float4 PixelShaderFunctionShadows(VertexShaderOutputShadows input) : COLOR0
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

	// computin normals

	float3 nAdj = (tex2D(NormalSampler, input.TexCoord)).xyz;
		input.Normal = normalize(input.Normal);

	nAdj.x = (nAdj.x * 2) - 1;
	nAdj.y = (nAdj.y * 2) - 1;
	nAdj.z = (nAdj.z) - 1;

	input.Normal = input.Normal + nAdj;
	input.Normal = normalize(input.Normal);

	////////

	// computin cube

	float3 reflection = (texCUBE(CubeSampler, normalize(input.Reflection))).xyz;
		reflection = reflection * ReflectivityColor;

	////////

	ColorPair computedLight = ComputeLightShadows(input.PositionWS.xyz, EyePosition - input.PositionWS.xyz, input.Normal, input.PositionDLS);

	color = (color * float4(computedLight.Diffuse, 1.0f) + alpha * float4(computedLight.Specular, 1.0f)) +
		ReflectivityBias * float4(computedLight.Diffuse, 1.0f) * (alpha * float4(reflection, 1.0f));

	color *= Transparency;

	return color;
}

float4 PixelShaderFunctionBlurredShadows(VertexShaderOutputShadows input) : COLOR0
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

	// computin normals

	float3 nAdj = (tex2D(NormalSampler, input.TexCoord)).xyz;
		input.Normal = normalize(input.Normal);

	nAdj.x = (nAdj.x * 2) - 1;
	nAdj.y = (nAdj.y * 2) - 1;
	nAdj.z = (nAdj.z) - 1;

	input.Normal = input.Normal + nAdj;
	input.Normal = normalize(input.Normal);

	////////

	// computin cube

	float3 reflection = (texCUBE(CubeSampler, normalize(input.Reflection))).xyz;
		reflection = reflection * ReflectivityColor;

	////////

	ColorPair computedLight = ComputeLightBlurredShadows(input.PositionWS.xyz, EyePosition - input.PositionWS.xyz, input.Normal,
		float2((input.PositionProj.x / input.PositionProj.w) / 2.0f + 0.5f, (-input.PositionProj.y / input.PositionProj.w) / 2.0f + 0.5f));

	color = (color * float4(computedLight.Diffuse, 1.0f) + alpha * float4(computedLight.Specular, 1.0f)) +
		ReflectivityBias * float4(computedLight.Diffuse, 1.0f) * (alpha * float4(reflection, 1.0f));

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

technique Skinned
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 VertexShaderFunctionSkinned();
		PixelShader = compile ps_3_0 PixelShaderFunction();
	}
}

technique MainShadows
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 VertexShaderFunctionShadows();
		PixelShader = compile ps_3_0 PixelShaderFunctionShadows();
	}
}

technique SkinnedShadows
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 VertexShaderFunctionSkinnedShadows();
		PixelShader = compile ps_3_0 PixelShaderFunctionShadows();
	}
}
 
technique MainBlurredShadows
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 VertexShaderFunctionShadows();
		PixelShader = compile ps_3_0 PixelShaderFunctionBlurredShadows();
	}
}

technique SkinnedBlurredShadows
{
	pass Pass1
	{
		VertexShader = compile vs_3_0 VertexShaderFunctionSkinnedShadows();
		PixelShader = compile ps_3_0 PixelShaderFunctionBlurredShadows();
	}
}