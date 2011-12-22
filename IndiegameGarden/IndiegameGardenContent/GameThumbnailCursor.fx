
float2 Center = float2(0.5,0.5);
float Time = 0;
float NoiseLevel = 0.001;
float Velocity = 0.02;
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
	lDif += NoiseLevel * noise(Time);
	float lWarped = (1-Velocity)*lDif + Velocity * lDif * lDif;
	float t = -Time;
	float2 vTexSample = Center + (lWarped * vDifNorm) + (Velocity * t * 0.8334 * vDifNorm); 
	res = tex2D(TextureSampler, vTexSample ) ;		  
	alpha = 1-2.0*lDif; //*lDif ;//*2.5;
	if (alpha < 0)
		alpha = 0;
	res *= alpha;
	res.a = alpha;

	
	// check for shadow box bounds
	if ( texCoord.x < (Center.x + ShadowBoxWidth) &&
		 texCoord.x > (Center.x - ShadowBoxWidth) &&
		 texCoord.y > (Center.y - ShadowBoxHeight) &&
		 texCoord.y < (Center.y + ShadowBoxHeight) )
	{
		float d=0;
		if (texCoord.x < Center.x)
			d = abs(texCoord.x - (Center.x-ShadowBoxWidth) );
		else if (texCoord.x > Center.x)
			d = abs(texCoord.x - (Center.x+ShadowBoxWidth) );
		float c = 1-23*d;
		if (c<0) c=0;
		
		if (texCoord.y < Center.y)
			d = abs(texCoord.y - (Center.y-ShadowBoxHeight) );
		else if (texCoord.y > Center.y)
			d = abs(texCoord.y - (Center.y+ShadowBoxHeight) );
		float c2 = 1-23*d;
		if (c2<0) c2=0;
		if (c2>c) c= c2;

		//res.a=0;
		res = c *res + (1-c) * float4(0,0,0,1); //float4(0.13,0.02,0.042,0);
		//res.a=1-c;
		if (c==0)
		{
			res.a=0;
		}
	}
	

	return res;

}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
