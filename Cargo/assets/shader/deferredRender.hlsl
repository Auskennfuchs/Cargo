Texture2D AlbedoTexture : register(t0);
SamplerState Sampler : register(s0);

cbuffer PerObject: register(b0)
{
	matrix worldViewProjMatrix;
	matrix worldMatrix;
	matrix viewMatrix;
	matrix projMatrix;
};

struct VS_Input
{
	float3 Position : POSITION;
	float3 Normal : NORMAL;
	float2 UV : TEXCOORD0;
};


struct VS_Output
{
	float4 Position : SV_Position;
	float3 Normal : NORMAL;
	float2 UV: TEXCOORD0;
};

struct PS_Output
{
	float4 Diffuse : SV_Target0;
	float4 Normal : SV_Target1;
	float4 Position: SV_Target2;
};

VS_Output VSMain(VS_Input input) {
	VS_Output output;

	output.Position = mul(float4(input.Position, 1.0f), worldViewProjMatrix);
	output.Normal = normalize(mul(input.Normal, (float3x3)worldMatrix));
	output.UV = input.UV;

	return output;
}

PS_Output PSMain(VS_Output input)
{
	PS_Output output;

	float3 diffuse = AlbedoTexture.Sample(Sampler, input.UV).rgb;

	output.Diffuse = float4(diffuse, 1);
	output.Normal = float4(input.Normal/2.0f+0.5f, 1);
	output.Position = input.Position;

	return output;
}
