Texture2D shadowMap : register(t0);
Texture2D depthMap : register(t1);
Texture2D colorMap : register(t2);
Texture2D normalMap : register(t3);

SamplerState samplerClamp : register(s0);

cbuffer Matrices {
	matrix LightView;
	matrix LightProj;
	matrix InvViewProj;
	float3 LightPos;
};

struct VS_Output
{
	float4 Position : SV_Position;
	float2 UV: TEXCOORD0;
};


VS_Output VSMain(uint id : SV_VertexID)
{
	VS_Output Output;
	float2 uv = float2((id << 1) & 2, id & 2);
	Output.Position = float4(uv * float2(2, -2) + float2(-1, 1), 1, 1);
	Output.UV = uv.xy;

	return Output;
}

float3 getLightPos(float4 wPos) {
	matrix cameraViewToProjectedLightSpace = mul(InvViewProj, mul(LightView, LightProj));
	float4 projectedEyeDir = mul(wPos, cameraViewToProjectedLightSpace);

	projectedEyeDir = projectedEyeDir / projectedEyeDir.w;

	return projectedEyeDir.xyz;
}

float4 PSMain(VS_Output input) : SV_Target
{
	float4 colorValue = colorMap.Sample(samplerClamp, input.UV);

	float4 wPos = depthMap.Sample(samplerClamp,input.UV);
	wPos.x = input.UV.x*2.0f - 1.0f;
	wPos.y = input.UV.y*2.0f - 1.0f;

	float3 lPos = getLightPos(wPos);

	if ((saturate(lPos.x) == lPos.x) && (saturate(lPos.y) == lPos.y)) {
		lPos.x = lPos.x / 2.0f + 0.5f;
		lPos.y = -lPos.y / 2.0f + 0.5f;

		float shadowDepthValue = shadowMap.Sample(samplerClamp, lPos.xy).r;
		if (shadowDepthValue < lPos.z) {
			return colorValue*float4(1.0f, 0, 0, 1.0f);
		}
	}

	return colorValue;

/*	float3 eyeDir = position.xyz - CameraPosition;
	float shadow = readShadowMap(eyeDir);
	float4 colorValue = colorMap.Sample(samplerClamp, input.UV);
	return colorValue*shadow;
*/
	/*	float4 depthValue = depthMap.Sample(samplerClamp,input.UV);
	float4 colorValue = colorMap.Sample(samplerClamp, input.UV);

	depthValue /= depthValue.w;
	depthValue = mul(depthValue, InvViewProj);
	depthValue /= depthValue.w;
	float3 sceneDepth = normalize(float3(-256.0f, 256.0f, 256.0f)- depthValue.xyz);

	float4 lightPos = mul(depthValue, LightView);
	lightPos = mul(lightPos, LightProj);
	lightPos.x = lightPos.x / lightPos.w / 2.0f + 0.5f;
	lightPos.y = -lightPos.y / lightPos.w / 2.0f + 0.5f;

	if ((saturate(lightPos.x) == lightPos.x) && (saturate(lightPos.y) == lightPos.y)) {
		float lightDepthValue = lightMap.Sample(samplerClamp, lightPos.xy).r;
		if (lightDepthValue < sceneDepth.z) {
			return float4(colorValue.rgb*0.2f,1.0f);
		}
	}
	return colorValue;*/
}