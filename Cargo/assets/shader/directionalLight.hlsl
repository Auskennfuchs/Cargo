Texture2D NormalTextureInput : register(t1);
Texture2D PositionTextureInput : register(t2);

cbuffer LightBuffer: register(b0) {
	float3 lightDir;
	float3 lightColor;
	float3 ambientColor;
}

cbuffer PerFrameBuffer : register(b1) {
	matrix invViewProjMatrix;
	float3 viewPosition;
}


struct VS_Input
{
	float3 Position : POSITION;
	float3 Normal : NORMAL;
};


struct VS_Output
{
	float4 Position : SV_Position;
};


VS_Output VSMain(uint id : SV_VertexID)
{
	VS_Output Output;
	float2 uv = float2((id << 1) & 2, id & 2);
	Output.Position = float4(uv * float2(2, -2) + float2(-1, 1), 1, 1);

	return Output;
}


float4 PSMain(VS_Output input) : SV_Target
{
	float4 normalSpec = NormalTextureInput.Load(int3(input.Position.xy, 0));
	float3 normal = normalSpec.xyz * 2 - 1.0;
	float specularIntensity = normalSpec.a;

	float4 position = PositionTextureInput.Load(int3(input.Position.xy, 0));
//	float4 pos = position / position.w;
	float4 pos = mul(position, invViewProjMatrix);
	pos /= pos.w;

	float3 lightVector = -normalize(lightDir);
	float NdL = saturate(dot(normal.xyz, lightVector));
	float3 col = lightColor.rgb*NdL+ambientColor;

	if (NdL > 0.0f) {
		float3 reflectionVector = normalize(reflect(normal,lightVector));
		float3 directionToCamera = normalize(viewPosition - pos.xyz);
		float specularLight = specularIntensity * pow(saturate(dot(reflectionVector, directionToCamera)), 16.0f);
		col += specularLight*lightColor.rgb;
	}

	return float4(saturate(col),1);
}
