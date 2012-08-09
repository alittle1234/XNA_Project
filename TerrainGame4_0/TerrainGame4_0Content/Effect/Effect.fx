//------------------------------------------------------
//--                                                	--
//--				www.riemers.net			--
//--				Basic shaders			--
//--			Use/modify as you like              --
//--                                                	--
//------------------------------------------------------

struct VertexToPixel
{
    float4 Position   	: POSITION;    
    float4 Color		: COLOR0;
    float LightingFactor: TEXCOORD0;
    float2 TextureCoords: TEXCOORD1;
};


struct PixelToFrame
{
    float4 Color : COLOR0;
};

//------- XNA-to-HLSL variables --------
float4x4 xView;
float4x4 xProjection;
float4x4 xWorld;
float3 xLightDirection;
float xAmbient;
bool xEnableLighting;
bool xShowNormals;

//------- Texture Samplers --------

Texture xTexture;
sampler TextureSampler = sampler_state { texture = <xTexture>; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};


////////////////////////////////////////////////////////////////////////////////////////
//------- Technique: Pretransformed --------
////////////////////////////////////////////////////////////////////////////////////////


VertexToPixel PretransformedVS( float4 inPos : POSITION, float4 inColor: COLOR)
{	
	VertexToPixel Output = (VertexToPixel)0;
	
	Output.Position = inPos;
	Output.Color = inColor;
    
	return Output;    
}

PixelToFrame PretransformedPS(VertexToPixel PSIn) 
{
	PixelToFrame Output = (PixelToFrame)0;		
	
	Output.Color = PSIn.Color;

	return Output;
}

technique Pretransformed_2_0
{
	pass Pass0
	{   
		VertexShader = compile vs_2_0 PretransformedVS();
		PixelShader  = compile ps_2_0 PretransformedPS();
	}
}

technique Pretransformed
{
	pass Pass0
	{   
		VertexShader = compile vs_2_0 PretransformedVS();
		PixelShader  = compile ps_2_0 PretransformedPS();
	}
}


////////////////////////////////////////////////////////////////////////////////////////
//------- Technique: Colored --------
////////////////////////////////////////////////////////////////////////////////////////

VertexToPixel ColoredVS( float4 inPos : POSITION, float4 inColor: COLOR, float3 inNormal: NORMAL)
{	
	VertexToPixel Output = (VertexToPixel)0;
	float4x4 preViewProjection = mul (xView, xProjection);
	float4x4 preWorldViewProjection = mul (xWorld, preViewProjection);
    
	Output.Position = mul(inPos, preWorldViewProjection);
	Output.Color = inColor;
	
	float3 Normal = normalize(mul(normalize(inNormal), xWorld));	
	Output.LightingFactor = 1;
	if (xEnableLighting)
		Output.LightingFactor = saturate(dot(Normal, -xLightDirection));
    
	return Output;    
}

PixelToFrame ColoredPS(VertexToPixel PSIn) 
{
	PixelToFrame Output = (PixelToFrame)0;		
    
	Output.Color = PSIn.Color;
	Output.Color.rgb *= saturate(PSIn.LightingFactor + xAmbient);
	
	return Output;
}

technique Colored_2_0
{
	pass Pass0
	{   
		VertexShader = compile vs_2_0 ColoredVS();
		PixelShader  = compile ps_2_0 ColoredPS();
	}
}

technique Colored
{
	pass Pass0
	{   
		VertexShader = compile vs_2_0 ColoredVS();
		PixelShader  = compile ps_2_0 ColoredPS();
	}
}

////////////////////////////////////////////////////////////////////////////////////////
//------- Technique: MultiTextured --------
////////////////////////////////////////////////////////////////////////////////////////

Texture xTexture0;
Texture xTexture1;
Texture xTexture2;
//Texture xTexture3;

sampler TextureSampler0 = sampler_state { texture = <xTexture> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = wrap; AddressV = wrap;};
sampler TextureSampler1 = sampler_state { texture = <xTexture0> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = wrap; AddressV = wrap;};
sampler TextureSampler2 = sampler_state { texture = <xTexture1> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};
sampler TextureSampler3 = sampler_state { texture = <xTexture2> ; magfilter = LINEAR; minfilter = LINEAR; mipfilter=LINEAR; AddressU = mirror; AddressV = mirror;};

struct MTVertexToPixel
{
    float4 Position         : POSITION;    
    float4 Color            : COLOR0;
    float3 Normal            : TEXCOORD0;
    float2 TextureCoords    : TEXCOORD1;
    float4 LightDirection    : TEXCOORD2;
    float4 TextureWeights    : TEXCOORD3;
    float    Depth            : TEXCOORD4;

};

struct MTPixelToFrame
{
    float4 Color : COLOR0;
};

MTVertexToPixel MultiTexturedVS( float4 inPos : POSITION, float3 inNormal: NORMAL, float2 inTexCoords: TEXCOORD0, float4 inTexWeights: TEXCOORD1)
{    
    MTVertexToPixel Output = (MTVertexToPixel)0;
    float4x4 preViewProjection = mul (xView, xProjection);
    float4x4 preWorldViewProjection = mul (xWorld, preViewProjection);
    
    Output.Position = mul(inPos, preWorldViewProjection);
    Output.Normal = mul(normalize(inNormal), xWorld);
    Output.TextureCoords = inTexCoords;
    Output.LightDirection.xyz = -xLightDirection;
    Output.LightDirection.w = 1;    
    Output.TextureWeights = inTexWeights;
    
    Output.Depth = Output.Position.z/Output.Position.w;
    
    return Output;    
}

MTPixelToFrame MultiTexturPS(MTVertexToPixel PSIn)
{
    MTPixelToFrame Output = (MTPixelToFrame)0;     
    
    float blendDistance = 0.99f;
	float blendWidth = 0.005f;
	
	float blendFactor = clamp((PSIn.Depth-blendDistance)/blendWidth, 0, 1);   
    
    float lightingFactor = 1;
    if (xEnableLighting)
        lightingFactor = saturate(saturate(dot(PSIn.Normal, PSIn.LightDirection)) + xAmbient);

	float4 farColor;
	farColor = tex2D(TextureSampler0, PSIn.TextureCoords)*PSIn.TextureWeights.x;
	farColor += tex2D(TextureSampler1, PSIn.TextureCoords)*PSIn.TextureWeights.y;
	farColor += tex2D(TextureSampler2, PSIn.TextureCoords)*PSIn.TextureWeights.z;
	farColor += tex2D(TextureSampler3, PSIn.TextureCoords)*PSIn.TextureWeights.w;
	    
	float4 midColor;
	float2 midTextureCoords = PSIn.TextureCoords*2.0f;
	midColor = tex2D(TextureSampler0, midTextureCoords)*PSIn.TextureWeights.x;
	midColor += tex2D(TextureSampler1, midTextureCoords)*PSIn.TextureWeights.y;
	midColor += tex2D(TextureSampler2, midTextureCoords)*PSIn.TextureWeights.z;
	midColor += tex2D(TextureSampler3, midTextureCoords)*PSIn.TextureWeights.w;    
	
	float4 nearColor;
	float2 nearTextureCoords = PSIn.TextureCoords*4.0f;
	nearColor = tex2D(TextureSampler0, nearTextureCoords)*PSIn.TextureWeights.x;
	nearColor += tex2D(TextureSampler1, nearTextureCoords)*PSIn.TextureWeights.y;
	nearColor += tex2D(TextureSampler2, nearTextureCoords)*PSIn.TextureWeights.z;
	nearColor += tex2D(TextureSampler3, nearTextureCoords)*PSIn.TextureWeights.w;
	        
	nearColor = lerp(nearColor, midColor, blendFactor);
	
	Output.Color = lerp(nearColor, farColor, blendFactor);

	Output.Color *= lightingFactor;

    return Output;
}

technique MultiTextured
{
    pass Pass0
    {
        VertexShader = compile vs_1_1 MultiTexturedVS();
        PixelShader = compile ps_2_0 MultiTexturPS();
    }
}

//------------------------------------
//------------------------------------
//------------------------------------



////////////////////////////////////////////////////////////////////////////////////////
//------- Technique: Textured --------
////////////////////////////////////////////////////////////////////////////////////////

VertexToPixel TexturedVS( float4 inPos : POSITION, float3 inNormal: NORMAL, float2 inTexCoords: TEXCOORD0)
{	
	VertexToPixel Output = (VertexToPixel)0;
	float4x4 preViewProjection = mul (xView, xProjection);
	float4x4 preWorldViewProjection = mul (xWorld, preViewProjection);
    
	Output.Position = mul(inPos, preWorldViewProjection);	
	Output.TextureCoords = inTexCoords;
	
	float3 Normal = normalize(mul(normalize(inNormal), xWorld));	
	Output.LightingFactor = 1;
	if (xEnableLighting)
		Output.LightingFactor = saturate(dot(Normal, -xLightDirection));
    
	return Output;    
}


PixelToFrame TexturedPS(VertexToPixel PSIn) 
{
	PixelToFrame Output = (PixelToFrame)0;		
	
	Output.Color = tex2D(TextureSampler, PSIn.TextureCoords);
	Output.Color.rgb *= saturate(PSIn.LightingFactor + xAmbient);

	return Output;
}

technique Textured_2_0
{
	pass Pass0
	{   
		VertexShader = compile vs_2_0 TexturedVS();
		PixelShader  = compile ps_2_0 TexturedPS();
	}
}

technique Textured
{
	pass Pass0
	{   
		VertexShader = compile vs_2_0 TexturedVS();
		PixelShader  = compile ps_2_0 TexturedPS();
	}
}


////////////////////////////////////////////////////////////////////////////////////////
//------- Technique: PointSprites --------
////////////////////////////////////////////////////////////////////////////////////////

struct SpritesVertexOut
{
    float4 Position		: POSITION0;
    float1 Size 		: PSIZE;
};

struct SpritesPixelIn
{
    #ifdef XBOX
		float2 TexCoord : SPRITETEXCOORD;
	#else
		float2 TexCoord : TEXCOORD0;
	#endif
};

SpritesVertexOut PointSpritesVS (float4 Position : POSITION, float4 Color : COLOR0, float1 Size : PSIZE)
{
    SpritesVertexOut Output = (SpritesVertexOut)0;
     
    float4x4 preViewProjection = mul (xView, xProjection);
	float4x4 preWorldViewProjection = mul (xWorld, preViewProjection); 
    Output.Position = mul(Position, preWorldViewProjection);    
    Output.Size = 1/(pow(Output.Position.z,2)+1	) * Size;
    
    return Output;    
}

PixelToFrame PointSpritesPS(SpritesPixelIn PSIn)
{ 
    PixelToFrame Output = (PixelToFrame)0;    

    #ifdef XBOX
		float2 texCoord = abs(PSIn.TexCoord.xy);
    #else
		float2 texCoord = PSIn.TexCoord.xy;
    #endif

    Output.Color = tex2D(TextureSampler, texCoord);
    
    return Output;
}

technique PointSprites_2_0
{
	pass Pass0
	{   
		//PointSpriteEnable = true;
		VertexShader = compile vs_2_0 PointSpritesVS();
		PixelShader  = compile ps_2_0 PointSpritesPS();
	}
}

technique PointSprites
{
	pass Pass0
	{   
		//PointSpriteEnable = true;
		VertexShader = compile vs_2_0 PointSpritesVS();
		PixelShader  = compile ps_2_0 PointSpritesPS();
	}
}



///////////////////////////////////////////////////////////////////////////////////////////////////
/////// ShadowMap ////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////

float4x4 xLightsWorldViewProjection;
Texture xShadowMap;

sampler ShadowMapSampler = sampler_state 
{ 
	texture = <xShadowMap> ; 
	magfilter = LINEAR;
	minfilter = LINEAR;
	 
	mipfilter = LINEAR; 
	
	AddressU = clamp; 
	AddressV = clamp;
};

 struct SMapVertexToPixel
 {
     float4 Position     : POSITION;
     float4 Position2D    : TEXCOORD0;
 };
 
 struct SMapPixelToFrame
 {
     float4 Color : COLOR0;
 };
 
 SMapVertexToPixel ShadowMapVertexShader( float4 inPos : POSITION)
 {
     SMapVertexToPixel Output = (SMapVertexToPixel)0;
 
     Output.Position = mul(inPos, xLightsWorldViewProjection);
     Output.Position2D = Output.Position;
 
     return Output;
 }
 
 SMapPixelToFrame ShadowMapPixelShader(SMapVertexToPixel PSIn)
 {
     SMapPixelToFrame Output = (SMapPixelToFrame)0;            
 
     Output.Color = PSIn.Position2D.z/PSIn.Position2D.w;
 
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
 
 ///////////////////////////////////////////////////////////////////////////////////////////////////
/////// ShadowedScene ////////////////////////////////////////////////////////////////////
///////////////////////////////////////////////////////////////////////////////////////////////////
float4x4 xWorldViewProjection;

 struct SSceneVertexToPixel
 {
     float4 Position             : POSITION;
     float4 Pos2DAsSeenByLight : TEXCOORD0;
 };
 
 struct SScenePixelToFrame
 {
     float4 Color : COLOR0;
 };
 
 SSceneVertexToPixel ShadowedSceneVertexShader( float4 inPos : POSITION)
 {
     SSceneVertexToPixel Output = (SSceneVertexToPixel)0;
 
     Output.Position = mul(inPos, xWorldViewProjection);    
     Output.Pos2DAsSeenByLight = mul(inPos, xLightsWorldViewProjection);    
 
     return Output;
 }
 
 SScenePixelToFrame ShadowedScenePixelShader(SSceneVertexToPixel PSIn)
 {
     SScenePixelToFrame Output = (SScenePixelToFrame)0;    
 
     float2 ProjectedTexCoords;
     ProjectedTexCoords[0] = PSIn.Pos2DAsSeenByLight.x/PSIn.Pos2DAsSeenByLight.w/2.0f +0.5f;
     ProjectedTexCoords[1] = -PSIn.Pos2DAsSeenByLight.y/PSIn.Pos2DAsSeenByLight.w/2.0f +0.5f;
 
     Output.Color = tex2D(ShadowMapSampler, ProjectedTexCoords);
 
     return Output;
 }
 
 technique ShadowedScene
 {
     pass Pass0
     {
         VertexShader = compile vs_2_0 ShadowedSceneVertexShader();
         PixelShader = compile ps_2_0 ShadowedScenePixelShader();
     }
 }