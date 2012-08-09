float4x4 xCamerasViewProjection;
float4x4 xLightsViewProjection;
float4x4 xWorld;

bool xSolidBrown = false;
float4 xColor = float4(0.25f, 0.21f, 0.20f, 1);
bool xMultiTextured = false;
bool xToon = true;
bool xSky = false;

float xSaturation = 0.0f;

// Specular Variables
float4 SpecularColor;
float SpecularIntensity;
float Shinniness;                // Sharpness the specular highlights. Higher number = sharper highlight
float3 CameraPosition;

// Settings controlling the Toon lighting technique.
float ToonThresholds[4] = {
 0.8,
 0.6, 
 0.4, 
 0.2 };
float ToonBrightnessLevels[5] = {
 1.3, 
 1.1, 
 0.9, 
 0.7, 
 0.5 };


float3 xLightPos;
float xLightPower;
float xAmbient;

float epsilon = 0.00003f;

Texture xTexture;

sampler TextureSampler = sampler_state 
{ 
	texture = <xTexture> ;
	magfilter = linear;
	minfilter = linear;
		 
	//mipfilter=ANISOTROPIC; 
	
	AddressU = Wrap; 
	AddressV = Wrap;
};

Texture xTexture0;
Texture xTexture1;
Texture xTexture2;

sampler TextureSampler0 = sampler_state { texture = <xTexture0> ;
 magfilter = linear;
  minfilter = linear;
  // mipfilter=ANISOTROPIC;
    AddressU = wrap;
     AddressV = wrap;};
sampler TextureSampler1 = sampler_state { texture = <xTexture1> ;
 magfilter = linear;
  minfilter = linear;
    //mipfilter=ANISOTROPIC;
      AddressU = wrap;
         AddressV = wrap;};
sampler TextureSampler2 = sampler_state { texture = <xTexture2> ;
 magfilter = linear;
  minfilter = linear;
  // mipfilter=ANISOTROPIC;
    AddressU = wrap;
     AddressV = wrap;};


bool xDecal = false;
Texture xDecalTex;

sampler DecalTexSampler = sampler_state 
{ 
	texture = <xDecalTex> ;
	magfilter = POINT;
	minfilter = POINT;
		 
	//mipfilter=linear; 
	
	AddressU = clamp; 
	AddressV = clamp;
};



struct PixelToFrame
{
    float4 Color        : COLOR0;
};


 float DotProduct(float3 lightPos, float3 pos3D, float3 normal)
 {
     float3 lightDir = normalize(pos3D - lightPos);
     return dot(-lightDir, normal);    
 }


 //----------------------------------------------------------------
//------- ShadowMap -------------------------------------------
//----------------------------------------------------------------

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


//-----------------------------------------------------------------------------
// Fog settings
//-----------------------------------------------------------------------------

float		FogEnabled		: register(c0);
float		FogStart		: register(c1);
float		FogEnd			: register(c2);
float4		FogColor		: register(c3);



//-----------------------------------------------------------------------------
// Compute fog factor
//-----------------------------------------------------------------------------
float ComputeFogFactor(float d)
{
	//endColor=lerp(color,fogcolor,(depth-fogstart)/(fogend-fogstart))
    return clamp((d - FogStart) / (FogEnd - FogStart), 0, 1) * FogEnabled;
}



//----------------------------------------------------------------
//------- ShadowedScene -------------------------------------------
//----------------------------------------------------------------

Texture xShadowMap;

sampler ShadowMapSampler = sampler_state 
{ 
	texture = <xShadowMap> ;
	magfilter = POINT;
	minfilter = POINT; //ANISOTROPIC POINT LINEAR;
	
	mipfilter = POINT;
	
	AddressU = clamp;
	AddressV = clamp;
};

struct SSceneVertexToPixel
{
    float4 Position             : POSITION;
    float4 Pos2DAsSeenByLight   : TEXCOORD0;
    
    float2 TexCoords            : TEXCOORD1;
	float3 Normal               : TEXCOORD2;
	float4 Position3D           : TEXCOORD3;
	float4 TextureWeights		: TEXCOORD4;
	float  Depth				: TEXCOORD5;
	float2 ProjTexCords			: TEXCOORD6;
	float3 CameraView			: TEXCOORD7;
};

struct ShadedVertexObj
{
    float4 Position             : POSITION;
    float4 Pos2DAsSeenByLight   : TEXCOORD0;
    
    float2 TexCoords            : TEXCOORD1;
	float3 Normal               : TEXCOORD2;
	float4 Position3D           : TEXCOORD3;
	
	float  Depth				: TEXCOORD4;
	float2 ProjTexCords			: TEXCOORD5;
	float3 CameraView			: TEXCOORD6;
};

//----DRAW SCENE  VS--------------------------------------------
SSceneVertexToPixel ShadowedSceneVS_Ter( float4 inPos : POSITION,
												float2 inTexCoords : TEXCOORD0,
  												float3 inNormal : NORMAL,
  												float4 inTexWeights : TEXCOORD1)
{
    SSceneVertexToPixel Output = (SSceneVertexToPixel)0;


	float4x4 preWorldViewProjection = mul (xWorld, xCamerasViewProjection);
    float4x4 preLightsWorldViewProjection = mul (xWorld, xLightsViewProjection);
     
    Output.Position = mul(inPos, preWorldViewProjection);    
    Output.Pos2DAsSeenByLight = mul(inPos, preLightsWorldViewProjection);
      
    Output.Normal = normalize(mul(inNormal, (float3x3)xWorld));    
    Output.Position3D = mul(inPos, xWorld);
    Output.TexCoords = inTexCoords;    
    
    Output.CameraView = normalize( CameraPosition - Output.Position3D );

	Output.TextureWeights = inTexWeights;
	Output.Depth = Output.Position.z/Output.Position.w;
	
	
    float2 ProTexCords;
	ProTexCords[0] = Output.Pos2DAsSeenByLight.x / Output.Pos2DAsSeenByLight.w /
								2.0f + 0.5f;
	ProTexCords[1] = -Output.Pos2DAsSeenByLight.y / Output.Pos2DAsSeenByLight.w /
								2.0f + 0.5f;
	Output.ProjTexCords = ProTexCords;
	
    return Output;
}

//----DRAW SCENE  VS--------------------------------------------
ShadedVertexObj ShadowedSceneVS_Obj( float4 inPos : POSITION,
												float2 inTexCoords : TEXCOORD0,
  												float3 inNormal : NORMAL)
{
    ShadedVertexObj Output = (ShadedVertexObj)0;


	float4x4 preWorldViewProjection = mul (xWorld, xCamerasViewProjection);
    float4x4 preLightsWorldViewProjection = mul (xWorld, xLightsViewProjection);
     
    Output.Position = mul(inPos, preWorldViewProjection);    
    Output.Pos2DAsSeenByLight = mul(inPos, preLightsWorldViewProjection);
      
    Output.Normal = normalize(mul(inNormal, (float3x3)xWorld));    
    Output.Position3D = mul(inPos, xWorld);
    Output.TexCoords = inTexCoords;    
    
    Output.CameraView = normalize( CameraPosition - Output.Position3D );

	
	

	Output.Depth = Output.Position.z/Output.Position.w;
	
	
    float2 ProTexCords;
	ProTexCords[0] = Output.Pos2DAsSeenByLight.x / Output.Pos2DAsSeenByLight.w /
								2.0f + 0.5f;
	ProTexCords[1] = -Output.Pos2DAsSeenByLight.y / Output.Pos2DAsSeenByLight.w /
								2.0f + 0.5f;
	Output.ProjTexCords = ProTexCords;
	
    return Output;
}

//----SPECULAR FUNC--------------------------------------------
//--------------------------------------------------------------
float GetSpecular(SSceneVertexToPixel input)
{
	// Normalize our variables
    float3 lightdir = normalize( xLightPos );
    float3 norm = normalize( input.Normal );
   
    // Calculate the half angle: the half the angle between our light direction and camera view
    float3 halfAngle = normalize( lightdir + input.CameraView );

    // Take the dot product between the normal of the pixel and the half angle. The closer the two
    // vectors are to pointing in the same direction, the higher the dot product. Take that to the [Shinniness]
    // power to make the highlight strong in the middle and fade out as you get towards the edges.
    
    return pow( saturate( dot( norm, halfAngle ) ), Shinniness ) * SpecularColor * SpecularIntensity;
    
}



float GetSpecular(ShadedVertexObj input)
{
	// Normalize our variables
    float3 lightdir = normalize( xLightPos );
    float3 norm = normalize( input.Normal );
   
    // Calculate the half angle: the half the angle between our light direction and camera view
    float3 halfAngle = normalize( lightdir + input.CameraView );

    // Take the dot product between the normal of the pixel and the half angle. The closer the two
    // vectors are to pointing in the same direction, the higher the dot product. Take that to the [Shinniness]
    // power to make the highlight strong in the middle and fade out as you get towards the edges.
    
    return pow( saturate( dot( norm, halfAngle ) ), Shinniness ) * SpecularColor * SpecularIntensity;
    
}

	
//----MULTI-TEX FUNC--------------------------------------------
//--------------------------------------------------------------
float4 GetMultiTexture(SSceneVertexToPixel PSIn)
{
         
    //float4 anotherColor;
    //if( xDecal )
	//{
		//
		//anotherColor = tex2D(DecalTexSampler, PSIn.TexCoords);
		//anotherColor.rgb += anotherColor.rgb * 2;
		//
		//return  anotherColor;
		//
	//}
	//else
	//{
	
    float blendDistance = 0.990f;  // 985f // 990f  // 975f // 970f
	float blendWidth = 0.015f;
	
	float blendFactor = clamp((PSIn.Depth-blendDistance)/blendWidth, 0, 1);       
    
	float xWeight = PSIn.TextureWeights.x;
	float yWeight = PSIn.TextureWeights.y;
	float zWeight = PSIn.TextureWeights.z;
	float wWeight = PSIn.TextureWeights.w;

	
	

	float4 farColor = float4(0,0,0,0);
	if(blendFactor > 0)
	{
		float2 farTextureCoords = PSIn.TexCoords*2.2f;		
		if(xWeight > 0)
			farColor = tex2D(TextureSampler, farTextureCoords)*xWeight;
		if(yWeight > 0)
			farColor += tex2D(TextureSampler0, farTextureCoords)*yWeight;
		if(zWeight > 0)
			farColor += tex2D(TextureSampler1, farTextureCoords)*zWeight;
		if(wWeight > 0)
			farColor += tex2D(TextureSampler2, farTextureCoords)*wWeight;
	}
	    
	//float4 midColor;
	//float2 midTextureCoords = PSIn.TexCoords*2.0f;
	//midColor = tex2D(TextureSampler, midTextureCoords)*PSIn.TextureWeights.x;
	//midColor += tex2D(TextureSampler0, midTextureCoords)*PSIn.TextureWeights.y;
	//midColor += tex2D(TextureSampler1, midTextureCoords)*PSIn.TextureWeights.z;
	//midColor += tex2D(TextureSampler2, midTextureCoords)*PSIn.TextureWeights.w;    
	
	float4 nearColor = float4(0,0,0,0);
	if(blendFactor < 1)
	{
		float2 nearTextureCoords = PSIn.TexCoords*5.3f;
		if(xWeight > 0)
			nearColor = tex2D(TextureSampler, nearTextureCoords)*xWeight;
			//nearColor = float4(0,0,0,0);
		if(yWeight > 0)
			nearColor += tex2D(TextureSampler0, nearTextureCoords)*yWeight;
			//nearColor = float4(0,0,0,0);
		if(zWeight > 0)
			nearColor += tex2D(TextureSampler1, nearTextureCoords)*zWeight;
			//nearColor = float4(0,0,0,0);
		if(wWeight > 0)
			nearColor += tex2D(TextureSampler2, nearTextureCoords)*wWeight;
			//nearColor = float4(0,0,0,0);
	}
	//midColor = lerp(midColor, nearColor, blendFactor);
	
	
	
	//float4 returnColor = (  lerp(midColor, farColor, blendFactor)  );
	//return  returnColor;

	return (  lerp(nearColor, farColor, blendFactor)  );
	return nearColor;
	//}
    
}
//--------------------------------------------------------------


struct SScenePixelToFrame
{
    float4 Color : COLOR0;
};


//----DRAW SCENE  PS--------------------------------------------
SScenePixelToFrame ShadowedScenePS_Ter(SSceneVertexToPixel PSIn)
{
    SScenePixelToFrame Output = (SScenePixelToFrame)0;    

    float2 ProjectedTexCoords = PSIn.ProjTexCords;	
	
    float diffuseLightingFactor = 0;
    
    if ((saturate(ProjectedTexCoords).x == ProjectedTexCoords.x) &&
     (saturate(ProjectedTexCoords).y == ProjectedTexCoords.y))
    {
		// PIXEL IS IN VIEW OF LIGHT


		// calculate distance from light, check if in shadow
		float2 moments = tex2D(ShadowMapSampler, ProjectedTexCoords);    
		float DisFrmLight = moments.x; 	
		//
		
		 float realDistance = 
			PSIn.Pos2DAsSeenByLight.z/PSIn.Pos2DAsSeenByLight.w;			
		float bias = 	0.00001f; 	
        
        if ((realDistance - bias) <= DisFrmLight)
        {
			// NOT IN SHADOW
			
            diffuseLightingFactor = dot(normalize(xLightPos),  normalize(PSIn.Normal));
			diffuseLightingFactor = saturate(diffuseLightingFactor) * xLightPower;			
        }
        else
        {
			// IN SHADOW

			// calculate blur effect on edge of shadow
			float variance = moments.y - (DisFrmLight * DisFrmLight);        
			variance = min(max(variance, 0) + epsilon, 1);		
			//

			float depth_diff = DisFrmLight - bias  - realDistance ;
			
			float p_max = variance / (variance + depth_diff * depth_diff );			
				
			diffuseLightingFactor = 0.2; 
			diffuseLightingFactor *= p_max ;
			
        }
    }
	else
	{
		// PIXEL !not! IN VIEW OF LIGHT
	
			diffuseLightingFactor = dot(normalize(xLightPos),  normalize(PSIn.Normal));
			diffuseLightingFactor = saturate(diffuseLightingFactor);
			diffuseLightingFactor *= xLightPower;  
	}
	
        
    float4 baseColor = xColor;
    if (xMultiTextured == true)
	{
		baseColor = GetMultiTexture( PSIn );
		//baseColor = tex2D(TextureSampler, PSIn.TexCoords);
	}
	else
	{
		baseColor = tex2D(TextureSampler, PSIn.TexCoords);
	}		
		
    if (xSolidBrown == true)
        baseColor = xColor;
		
	//if (xSky == true)
	//{		
		//baseColor = tex2D(TextureSampler, PSIn.TexCoords);		
		//diffuseLightingFactor = 0.6;
	//}
				

	float light;
	light = diffuseLightingFactor + xAmbient + GetSpecular(PSIn);

	if( xToon )
	{
		float TBrightnessLevels;
	
		if (light > ToonThresholds[0])
			TBrightnessLevels = ToonBrightnessLevels[0];
        
		else if (light > ToonThresholds[1])
			TBrightnessLevels = ToonBrightnessLevels[1];
        
		else if (light > ToonThresholds[2])
			TBrightnessLevels = ToonBrightnessLevels[2];
		
		else if (light > ToonThresholds[3])
			TBrightnessLevels = ToonBrightnessLevels[3];
		
		else
			TBrightnessLevels = ToonBrightnessLevels[4];
        
    
		light *= TBrightnessLevels;
	}
     
	Output.Color = baseColor * ( light ) ;
	Output.Color = lerp(Output.Color, FogColor,  ComputeFogFactor(length(CameraPosition - PSIn.Position3D))  );

    return Output;
}

//----DRAW SCENE  PS--------------------------------------------
SScenePixelToFrame ShadowedScenePS_Obj(ShadedVertexObj PSIn)
{
    SScenePixelToFrame Output = (SScenePixelToFrame)0;    

    float2 ProjectedTexCoords = PSIn.ProjTexCords;
   
   
	float2 moments = tex2D(ShadowMapSampler, ProjectedTexCoords);
    
	float DisFrmLight = moments.x; 
	
    float variance = moments.y - (DisFrmLight * DisFrmLight);
        
    variance = min(max(variance, 0) + epsilon, 1);	
	
	
    float diffuseLightingFactor = 0;
    
   
    bool INSHADOW = false;
    
    if ((saturate(ProjectedTexCoords).x == ProjectedTexCoords.x) &&
     (saturate(ProjectedTexCoords).y == ProjectedTexCoords.y))
    {
		// PIXEL IS IN VIEW OF LIGHT
		
		float realDistance = 
			PSIn.Pos2DAsSeenByLight.z/PSIn.Pos2DAsSeenByLight.w;			
		float bias = 	0.00001f; 
	
        
        if ((realDistance - bias) <= DisFrmLight)
        {
			// NOT IN SHADOW
			
            diffuseLightingFactor = dot(normalize(xLightPos),  normalize(PSIn.Normal));
			diffuseLightingFactor = saturate(diffuseLightingFactor) * xLightPower;			
        }
        else
        {
			// IN SHADOW
			
			float depth_diff = DisFrmLight - bias  - realDistance ;
			
			float p_max = variance / (variance + depth_diff * depth_diff );			
				
			diffuseLightingFactor = 0.1; 
			diffuseLightingFactor *= p_max ;
			
        }
    }
	else
	{
		// PIXEL !not! IN VIEW OF LIGHT
	
			diffuseLightingFactor = dot(normalize(xLightPos),  normalize(PSIn.Normal));
			diffuseLightingFactor = saturate(diffuseLightingFactor);
			diffuseLightingFactor *= xLightPower;  
	}

	diffuseLightingFactor = dot(normalize(xLightPos),  normalize(PSIn.Normal));
			diffuseLightingFactor = saturate(diffuseLightingFactor);
			diffuseLightingFactor *= xLightPower;  
        
    float4 baseColor = float4(0,0,0,0);
    
	baseColor = tex2D(TextureSampler, PSIn.TexCoords);
		
    //if (xSolidBrown == true)
        //baseColor = xColor;
                //
    //if (xMultiTextured == true)            
		//baseColor = GetMultiTexture( PSIn );
		//
	if (xSky == true)
	{		
		baseColor = tex2D(TextureSampler, PSIn.TexCoords);
		diffuseLightingFactor = 0.6;
	}
				

	float light;
	light = diffuseLightingFactor + xAmbient + GetSpecular(PSIn);
	float TBrightnessLevels;
	
	if (light > ToonThresholds[0])
        TBrightnessLevels = ToonBrightnessLevels[0];
        
    else if (light > ToonThresholds[1])
        TBrightnessLevels = ToonBrightnessLevels[1];
        
    else if (light > ToonThresholds[2])
		TBrightnessLevels = ToonBrightnessLevels[2];
		
	else if (light > ToonThresholds[3])
		TBrightnessLevels = ToonBrightnessLevels[3];
		
    else
        TBrightnessLevels = ToonBrightnessLevels[4];
        
    if( xToon )
	    light *= TBrightnessLevels;
        
   
		Output.Color = baseColor * ( light ) ;
		//endColor=lerp(color,fogcolor,(depth-fogstart)/(fogend-fogstart))
		Output.Color = lerp(Output.Color, FogColor,  ComputeFogFactor(PSIn.Depth)  );
    return Output;
}

// USING DIFFERENT TECHNIQUE FOR TERRAIN, VERTEX SHADER WON'T REQURIE TEXWEIGHTS

technique ShadowedScene_Ter
{
    pass Pass0
    {
        VertexShader = compile vs_3_0 ShadowedSceneVS_Ter();
        PixelShader = compile ps_3_0 ShadowedScenePS_Ter();
    }
}

technique ShadowedScene_Obj
{
    pass Pass0
    {
        VertexShader = compile vs_3_0 ShadowedSceneVS_Obj();
        PixelShader = compile ps_3_0 ShadowedScenePS_Obj();
    }
}



//-----------------------------------------------------------------------------
// PostprocessEffect.fx
//
// Microsoft XNA Community Game Platform
// Copyright (C) Microsoft Corporation. All rights reserved.
//-----------------------------------------------------------------------------


// Settings controlling the edge detection filter.
float EdgeWidth = 1;
float EdgeIntensity = 1;

// How sensitive should the edge detection be to tiny variations in the input data?
// Smaller settings will make it pick up more subtle edges, while larger values get
// rid of unwanted noise.
float NormalThreshold = 0.5;
float DepthThreshold = 0.1;

// How dark should the edges get in response to changes in the input data?
float NormalSensitivity = 1;
float DepthSensitivity = 10;

// How should the sketch effect respond to changes of brightness in the input scene?
float SketchThreshold = 0.1;
float SketchBrightness = 0.333;

// Randomly offsets the sketch overlay pattern to create a hand-drawn animation effect.
float2 SketchJitter;

// Pass in the current screen resolution.
float2 ScreenResolution;


// This texture contains the main scene image, which the edge detection
// and/or sketch filter are being applied over the top of.
texture SceneTexture;

sampler SceneSampler : register(s0) = sampler_state
{
    Texture = (SceneTexture);
    
    MinFilter = Linear;
    MagFilter = Linear;
    
    AddressU = Clamp;
    AddressV = Clamp;
};


// This texture contains normals (in the color channels) and depth (in alpha)
// for the main scene image. Differences in the normal and depth data are used
// to detect where the edges of the model are.
texture NormalDepthTexture;

sampler NormalDepthSampler : register(s1) = sampler_state
{
    Texture = (NormalDepthTexture);
    
    MinFilter = Linear;
    MagFilter = Linear;
    
    AddressU = Clamp;
    AddressV = Clamp;
};


// This texture contains an overlay sketch pattern, used to create the hatched
// pencil drawing effect.
texture SketchTexture;

sampler SketchSampler : register(s2) = sampler_state
{
    Texture = (SketchTexture);

    AddressU = Wrap;
    AddressV = Wrap;
};


// Pixel shader applies the edge detection and/or sketch filter postprocessing.
// It is compiled several times using different settings for the uniform boolean
// parameters, producing different optimized versions of the shader depending on
// which combination of processing effects is desired.
float4 Pixelshader(float2 texCoord : TEXCOORD0, uniform bool applyEdgeDetect,
                                                uniform bool applySketch,
                                                uniform bool sketchInColor) : COLOR0
{
    // Look up the original color from the main scene.
    float3 scene = tex2D(SceneSampler, texCoord);
    
    // Apply the sketch effect?
    if (applySketch)
    {
        // Adjust the scene color to remove very dark values and increase the contrast.
        float3 saturatedScene = saturate((scene - SketchThreshold) * 2);
        
        // Look up into the sketch pattern overlay texture.
        float3 sketchPattern = tex2D(SketchSampler, texCoord + SketchJitter);
    
        // Convert into negative color space, and combine the scene color with the
        // sketch pattern. We need to do this multiply in negative space to get good
        // looking results, because pencil sketching works by drawing black ink
        // over an initially white page, rather than adding light to an initially
        // black background as would be more common in computer graphics.
        float3 negativeSketch = (1 - saturatedScene) * (1 - sketchPattern);
        
        // Convert the result into a positive color space greyscale value.
        float sketchResult = dot(1 - negativeSketch, SketchBrightness);
        
        // Apply the sketch result to the main scene color.
        if (sketchInColor)
            scene *= sketchResult;
        else
            scene = sketchResult;
    }
    
    // Apply the edge detection filter?
    if (applyEdgeDetect)
    {
        // Look up four values from the normal/depth texture, offset along the
        // four diagonals from the pixel we are currently shading.
        float2 edgeOffset = EdgeWidth / ScreenResolution;
        
        float4 n1 = tex2D(NormalDepthSampler, texCoord + float2(-1, -1) * edgeOffset);
        float4 n2 = tex2D(NormalDepthSampler, texCoord + float2( 1,  1) * edgeOffset);
        float4 n3 = tex2D(NormalDepthSampler, texCoord + float2(-1,  1) * edgeOffset);
        float4 n4 = tex2D(NormalDepthSampler, texCoord + float2( 1, -1) * edgeOffset);

        // Work out how much the normal and depth values are changing.
        float4 diagonalDelta = abs(n1 - n2) + abs(n3 - n4);

        float normalDelta = dot(diagonalDelta.xyz, 1);
        float depthDelta = diagonalDelta.w;
        
        // Filter out very small changes, in order to produce nice clean results.
        normalDelta = saturate((normalDelta - NormalThreshold) * NormalSensitivity);
        depthDelta = saturate((depthDelta - DepthThreshold) * DepthSensitivity);

        // Does this pixel lie on an edge?
        float edgeAmount = saturate(normalDelta + depthDelta) * EdgeIntensity;
        
        // Apply the edge detection result to the main scene color.
        scene *= (1 - edgeAmount);
    }

    return float4(scene, 1);
}


// Compile the pixel shader for doing edge detection without any sketch effect.
technique EdgeDetect
{
    pass P0
    {
        PixelShader = compile ps_2_0 Pixelshader(true, false, false);
    }
}

// Compile the pixel shader for doing edge detection with a monochrome sketch effect.
technique EdgeDetectMonoSketch
{
    pass P0
    {
        PixelShader = compile ps_2_0 Pixelshader(true, true, false);
    }
}

// Compile the pixel shader for doing edge detection with a colored sketch effect.
technique EdgeDetectColorSketch
{
    pass P0
    {
        PixelShader = compile ps_2_0 Pixelshader(true, true, true);
    }
}

// Compile the pixel shader for doing a monochrome sketch effect without edge detection.
technique MonoSketch
{
    pass P0
    {
        PixelShader = compile ps_2_0 Pixelshader(false, true, false);
    }
}

// Compile the pixel shader for doing a colored sketch effect without edge detection.
technique ColorSketch
{
    pass P0
    {
        PixelShader = compile ps_2_0 Pixelshader(false, true, true);
    }
}


// Vertex shader input structure.
struct VertexShaderInput
{
    float4 Position : POSITION0;
    float2 TextureCoordinate : TEXCOORD0;
    float3 Normal : NORMAL0;
   
};

// Output structure for the vertex shader that renders normal and depth information.
struct NormalDepthVertexShaderOutput
{
    float4 Position : POSITION0;
    float4 Color : COLOR0;
};


// Alternative vertex shader outputs normal and depth values, which are then
// used as an input for the edge detection filter in PostprocessEffect.fx.
NormalDepthVertexShaderOutput NormalDepthVertexShader(VertexShaderInput input)
{
    NormalDepthVertexShaderOutput output;

    // Apply camera matrices to the input position.
    float4x4 preCamWVP = mul (xWorld, xCamerasViewProjection);
    output.Position = mul(input.Position, preCamWVP);
    
    float3 worldNormal = mul(input.Normal, xWorld);

    // The output color holds the normal, scaled to fit into a 0 to 1 range.
    output.Color.rgb = (worldNormal + 1) / 2;

    // The output alpha holds the depth, scaled to fit into a 0 to 1 range.
    output.Color.a = output.Position.z / output.Position.w;
    
    if(xSky)
    {
		output.Color.a = 1;
		output.Color.rgb = 0;
	}
    
    return output;    
}


// Simple pixel shader for rendering the normal and depth information.
float4 NormalDepthPixelShader(float4 color : COLOR0) : COLOR0
{
    return color;
}

// Technique draws the object as normal and depth values.
technique NormalDepth
{
    pass P0
    {
        VertexShader = compile vs_2_0 NormalDepthVertexShader();
        PixelShader = compile ps_2_0 NormalDepthPixelShader();
    }
}




///////////////////////////////////////////////////////////
// Depth texture shader

float4x4	matWorldViewProj;

struct OUT_DEPTH
{
	float4 Position : POSITION;
	float Distance : TEXCOORD0;
};

OUT_DEPTH RenderDepthMapVS(float4 vPos: POSITION)
{
	OUT_DEPTH Out;
	// Translate the vertex using matWorldViewProj.
	Out.Position = mul(vPos, matWorldViewProj);
	// Get the distance of the vertex between near and far clipping plane in matWorldViewProj.
	Out.Distance.x = 1-(Out.Position.z/Out.Position.w);	
	
	return Out;
}

float4 RenderDepthMapPS( OUT_DEPTH In ) : COLOR
{ 
    return float4(In.Distance.x,0,0,1);
}

technique DepthMapShader
{
	pass P0
	{
		ZEnable = TRUE;
		ZWriteEnable = TRUE;
		AlphaBlendEnable = FALSE;
		
        VertexShader = compile vs_2_0 RenderDepthMapVS();
        PixelShader  = compile ps_2_0 RenderDepthMapPS();
	
	}
}