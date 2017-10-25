﻿cbuffer MatrixBuffer
{
	matrix worldViewProjMatrix;
	matrix worldMatrix;
	matrix viewMatrix;
	matrix projMatrix;
};

struct VertexInputType
{
	float4 position : POSITION;
};

struct PixelInputType
{
	float4 lightViewPosition : SV_Position;
};

PixelInputType VSMain(VertexInputType input)
{
	PixelInputType output;

	input.position.w = 1.0f;
	output.lightViewPosition = mul(input.position, worldViewProjMatrix);

	return output;
}

void PSMain(PixelInputType input) { }