
cbuffer PerObject: register(b0)
{
	matrix worldMatrix;
	matrix viewMatrix;
	matrix projMatrix;
	matrix InvViewProj;
	matrix LightView;
	matrix LightProj;
	float3 LightPos;
};

SamplerState Sampler:register(s0);
texture2D AlbedoTexture: register(t0);

texture2D ShadowMap: register(t1);
SamplerState ShadowSampler:register(s1);

float3 ambient = float3(0.3f, 0.3f, 0.3f);

struct VertexShaderInput {
	float3 Position : POSITION;
	float3 Normal: NORMAL;
	float2 UV : TEXCOORD0;
};

struct VertexShaderOutput {
	float4 Position : SV_Position;
	float3 Normal: NORMAL;
	float2 UV : TEXCOORD0;
	float4 WorldPos : TEXCOORD1;
	float4 LightPos : TEXCOORD2;
	float3 LightRay : TEXCOORD3;
};

VertexShaderOutput VSMain(VertexShaderInput input) {
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = float4(input.Position, 1.0f);
	output.Position = mul(output.Position, worldMatrix);
	output.WorldPos = output.Position;
	output.Position = mul(output.Position, viewMatrix);
	output.Position = mul(output.Position, projMatrix);

	output.LightPos = mul(output.WorldPos, LightView);
	output.LightPos = mul(output.LightPos, LightProj);

	output.LightRay = output.LightPos.xyz - output.WorldPos.xyz;

	output.Normal = mul(float4(input.Normal,1.0f), worldMatrix).xyz;
	output.UV = input.UV;

	return output;
}

float4 PSMain(VertexShaderOutput input) : SV_Target
{
	float3 colorValue = AlbedoTexture.Sample(Sampler,input.UV).rgb;

	float2 shadowTexCoords;
	shadowTexCoords.x = 0.5f + (input.LightPos.x / input.LightPos.w * 0.5f);
	shadowTexCoords.y = 0.5f - (input.LightPos.y / input.LightPos.w * 0.5f);
	float pixelDepth = input.LightPos.z / input.LightPos.w;

	float3 L = normalize(-input.LightRay);
	float NdotL = saturate(dot(normalize(input.Normal), L));
	if ((saturate(shadowTexCoords.x) == shadowTexCoords.x) && (saturate(shadowTexCoords.y) == shadowTexCoords.y)) {
		float margin = acos(saturate(NdotL));
		float epsilon = 0.001 / margin;
		float shadowDepthValue = ShadowMap.Sample(ShadowSampler, shadowTexCoords.xy).r;
		if (shadowDepthValue < pixelDepth-0.000001f) {
				return float4(colorValue*0.2f, 1.0f);
		} 
	}

	if (NdotL > 0.0f) {
//		colorValue = colorValue*ambient + colorValue*NdotL;
	}
	return float4(colorValue,1);
}