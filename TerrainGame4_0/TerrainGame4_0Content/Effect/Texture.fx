float4x4 World;
float4x4 View;
float4x4 Projection;

float4 AmbientColor;
float AmbientIntensity;

float4 DiffuseColor;
float DiffuseIntensity;
float3 DiffuseLightDirection;

float4 SpecularColor;
float SpecularIntensity;
float Shinniness;
float3 CameraPosition;

// Texturing variables
texture  ModelTexture;        // Our texture which will be mapped onto our object

// Pass coordinates to our texture sampler to get a color for a certain pixel
sampler TextureSampler = sampler_state {
    texture = <ModelTexture> ;
    magfilter = LINEAR;
    minfilter = LINEAR;
    mipfilter= LINEAR;
    AddressU = WRAP;
    AddressV = WRAP;};


struct VertexShaderInput
{
    float4 Position : POSITION0;
    float3 Normal : NORMAL0;
    float2 TexCoords : TEXCOORD0;
};

struct VertexShaderOutput
{
    float4 Position : POSITION0;
    float3 Normal : TEXCOORD0;
    float3 CameraView : TEXCOORD1;
    float2 TexCoords :TEXCOORD2;
};

VertexShaderOutput VertexShader1( VertexShaderInput input )
{
    VertexShaderOutput output;
   
    float4 worldPosition = mul( input.Position, World );
    float4 viewPosition = mul( worldPosition, View );
    output.Position = mul( viewPosition, Projection );           
       
    output.Normal = mul( input.Normal, World );
    output.CameraView = normalize( CameraPosition - worldPosition );    
   
    // Just pass the texture coordinates to the vertex shader output.
    // When they transfer to the pixel shader, they will be interpolated
    // per pixel.
    output.TexCoords = input.TexCoords;
   
    return output;
}

float4 PixelShader1( VertexShaderOutput input ) : COLOR0
{
    // Sample our texture at the specified texture coordinates to get the texture color
    float4 texColor = tex2D( TextureSampler, input.TexCoords );
   
    float3 lightdir = normalize( DiffuseLightDirection );
    float3 norm = normalize( input.Normal );
    float3 halfAngle = normalize( lightdir + input.CameraView );
    float specular = pow( saturate( dot( norm, halfAngle ) ), Shinniness ) * SpecularColor * SpecularIntensity;
   
    float4 diffuse = dot( lightdir, input.Normal ) * DiffuseIntensity * DiffuseColor;
    float4 ambient = AmbientIntensity * AmbientColor;
   
    return texColor * ( diffuse + ambient + specular );
}

technique Texturing
{
    pass Pass0
    {
        VertexShader = compile vs_1_1 VertexShader1();
        PixelShader = compile ps_2_0 PixelShader1();
    }
}
