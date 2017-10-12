
cbuffer PerObject: register(b0)
{
	matrix worldViewProjMatrix;
	matrix worldMatrix;
	matrix viewMatrix;
	matrix projMatrix;
};


struct VertexShaderInput {
	float3 Position : POSITION;
	float3 Normal : NORMAL;
};

struct VertexShaderOutput {
	float3 origPos : TEXCOORD0;
	float4 Position : SV_Position;
	float3 Normal : NORMAL;
};

VertexShaderOutput VSMain(VertexShaderInput input) {
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.origPos = input.Position;
	output.Position = float4(input.Position, 1.0f);
	output.Position = mul(output.Position, worldMatrix);
	output.Position = mul(output.Position, viewMatrix);
	output.Position = mul(output.Position, projMatrix);

	output.Normal = normalize(mul(input.Normal, (float3x3)worldMatrix));

	return output;
}

float4 PSMain(VertexShaderOutput input) : SV_Target
{
//	float c = (.1f*input.origPos.y + 1.0f) / 2.0f;
	return float4(input.Normal,1.0f);
}