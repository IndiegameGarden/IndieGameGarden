// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

float2 Center = float2(0.5,0.5);
float Time = 0;
float NoiseLevel = 0.025;
float Velocity = 0.0001;
float ShadowBoxWidth = 0.5/2;
float ShadowBoxHeight = 0.5/2;

// modify the sampler state on the zero texture sampler, used by SpriteBatch
sampler TextureSampler : register(s0) = 
sampler_state
{
    AddressU = Clamp;
    AddressV = Clamp;
};

float4 PixelShaderFunction(float2 texCoord : TEXCOORD0) : COLOR0
{
	float4 tex = tex2D(TextureSampler, ((texCoord - Center)*2+Center) ) ;		  
	float4 res = float4(0,0,0,0);
	float alpha ;
	float2 vDif = texCoord - Center ;
	float2 vDifNorm = normalize(vDif);
	float lDif = length(vDif);
	lDif += NoiseLevel * noise(Time/4) ; 
	float lWarped = (1-Velocity)*lDif + Velocity * lDif * lDif;
	float t = -Time;
	float2 vTexSample = Center + (lWarped * vDifNorm) + (Velocity * t * 0.8334 * vDifNorm); 
	res = tex2D(TextureSampler, vTexSample ) ;		  
	
	alpha = 1-2.0*lWarped*lWarped; 
	if (alpha < 0)
		alpha = 0;
	res *= alpha;
	//res.a *= alpha;
	return res;

}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
