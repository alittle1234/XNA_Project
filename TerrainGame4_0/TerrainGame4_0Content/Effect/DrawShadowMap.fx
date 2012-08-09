//----------------------------------------------------------------
//------- ShadowMap -------------------------------------------
//----------------------------------------------------------------

float4x4 xLightsViewProjection;
float4x4 xWorld;

struct SMapVertexToPixel
{
    float4 Position     : POSITION;
    float4 Position2D    : TEXCOORD0;
};


//----CREATE SHADOW  VS--------------------------------------------

SMapVertexToPixel ShadowMapVertexShader( float4 inPos : POSITION)
{
    SMapVertexToPixel Output = (SMapVertexToPixel)0;

	float4x4 preLightsWorldViewProjection = mul (xWorld, xLightsViewProjection);
	
    Output.Position = mul(inPos, preLightsWorldViewProjection);
    Output.Position2D = Output.Position;


    return Output;
}



struct SMapPixelToFrame
{
    float4 Color : COLOR0;
};

//----CREATE SHADOW  PS--------------------------------------------
SMapPixelToFrame ShadowMapPixelShader(SMapVertexToPixel PSIn)
{
    SMapPixelToFrame Output = (SMapPixelToFrame)0; 
               
	float depth = (PSIn.Position2D.z/PSIn.Position2D.w);
	
//float4(depth, depth*depth, 0, 1);

    Output.Color = float4( depth,		depth*depth,		0,		1) ;

    return Output;
}

technique ShadowMap
{
    pass Pass0
    {
        VertexShader = compile vs_2_0 ShadowMapVertexShader();
        PixelShader = compile ps_2_0 ShadowMapPixelShader();
    }
}