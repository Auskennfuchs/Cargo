SamplerState InputSampler: register(s3);
texture2D InputTexture: register(t0);

struct VS_Output
{
	float4 Position : SV_Position;
	float2 Tex : TEXCOORD0;
};


VS_Output FxaaVS(uint id : SV_VertexID)
{
	VS_Output Output;
	float2 uv = float2((id << 1) & 2, id & 2);
	Output.Position = float4(uv * float2(2, -2) + float2(-1, 1), 0, 1);
	Output.Tex = float2((id << 1) & 2, id & 2);

	return Output;
}

float4 FxaaPS(VS_Output input) : SV_Target 
{
	float3 col = InputTexture.Sample(InputSampler,input.Tex).rgb;
	col *= float3(1.0f, 0.5f, 0.3f);
	return float4(col, 1.0f);
}