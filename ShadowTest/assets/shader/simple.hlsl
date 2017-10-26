
cbuffer PerObject: register(b0)
{
	matrix worldMatrix;
	matrix viewMatrix;
	matrix projMatrix;
};


struct VertexShaderInput {
	float3 Position : POSITION;
};

struct VertexShaderOutput {
	float4 Position : SV_Position;
};

VertexShaderOutput VSMain(VertexShaderInput input) {
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = float4(input.Position, 1.0f);
	output.Position = mul(output.Position, worldMatrix);
	output.Position = mul(output.Position, viewMatrix);
	output.Position = mul(output.Position, projMatrix);

	return output;
}

float4 PSMain(VertexShaderOutput input) : SV_Target
{
	return float4(1.0f,0.0f,1.0f,1.0f);
}