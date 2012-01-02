// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

float Time = 0;
float2 Center = float2(0.5,0.5);
float NoiseLevel = 0.005;
float Velocity = 0.02;
float ShadowBoxWidth = 0.7/2;
float ShadowBoxHeight = 0.7/2;
float2 ShadowBoxScale = float2(0.7,0.7);

// modify the sampler state on the zero texture sampler, used by SpriteBatch
sampler TextureSampler : register(s0) = 
sampler_state
{
    AddressU = Clamp;
    AddressV = Clamp;
};

float4 PixelShaderFunction(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
	float4 tex = tex2D(TextureSampler, ((texCoord - Center)/ShadowBoxScale+Center) ) ;		  
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

		// rounded corner

		// use the best-case c/c2
		if (c2<0) c2=0;
		if (c2>c) c= c2;

		//res.a=0;
		res = c *res + (1-c) * float4(0,0,0,1); //float4(0.13,0.02,0.042,0);
		//res.a=1-c;
		if (c==0)
		{
			//float4 bg=res;
			//res.a=0;
			res=tex;
			/*
			float rRoundedY = 2*0.1;
			float rRoundedX = 2*0.0625;
			float2 cor1 = float2(Center.x-ShadowBoxWidth+rRoundedX, Center.y-ShadowBoxHeight+rRoundedY);
			if (texCoord.x < cor1.x && texCoord.y < cor1.y )
			{			
				d = length( (texCoord - cor1) ); //* float2(0.625 , 1.0) );
				if (d<rRoundedY)
					res=float4(1,0,0,0); //bg;
			}
			*/
		}
	}
	

	return res * color;

}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
