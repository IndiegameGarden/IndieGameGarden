// (c) 2010-2012 TranceTrance.com. Distributed under the FreeBSD license in LICENSE.txt

// variables set from the outside - must be same for all sprites in a batch rendered with this shader.
// linear time - for animation control
float Time = 0;
// center constant
float2 Center = float2(0.5,0.5);
// radial motion noise
float NoiseLevel = 0.005;
// velocity of the outward running pixels effect
float Velocity = 0.02;
// size settings for the 'shadow box' border
float ShadowBoxWidth = 0.7/2;
float ShadowBoxHeight = 0.7/2;
float2 ShadowBoxScale = float2(0.6,0.6);

// modify the sampler state on the zero texture sampler, used by SpriteBatch
sampler TextureSampler : register(s0) = 
sampler_state
{
    AddressU = Clamp;
    AddressV = Clamp;
};

// shader uses 'color', the DrawColor, for special effects that are set per sprite:
// color.a - transparency result 0...1
// color.r - saturation 0...1 (0=black&white, 1=colored-full)
// color.g - intensity 0 (dark)...1 (light)
//
// alternative: color.r/g/b used as a 'time' variable.
//
float4 PixelShaderFunction(float4 color : COLOR0, float2 texCoord : TEXCOORD0) : COLOR0
{
	float4 tex = tex2D(TextureSampler, ((texCoord - Center)/ShadowBoxScale+Center) ) ;		  
	float4 res ;
	float time = color.r * 65536 + color.g*256 + color.b ;

	float alpha ;
	float2 vDif = texCoord - Center ;
	float2 vDifNorm = normalize(vDif);
	float lDif = length(vDif);
	lDif += NoiseLevel * noise(Time/3);
	float lWarped = (1-Velocity)*lDif + Velocity * lDif * lDif;
	float t = -time;
	float2 vTexSample = Center + (lWarped * vDifNorm) + (Velocity * t * 0.8334 * vDifNorm); 
	res = tex2D(TextureSampler, vTexSample ) ;		  
	alpha = 1-1.95*lDif; //2.0*lDif; //*lDif ;//*2.5;
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

		// use the best-case c/c2
		if (c2<0) c2=0;
		if (c2>c) c= c2;

		res = c *res + (1-c) * float4(0,0,0,1); //float4(0.13,0.02,0.042,0);
		if (c==0)
		{
			// copy the bitmap color as-is.
			res=tex;
		}
	}
	
	// apply saturation technique http://lukhezo.com/2011/03/12/saturationdesaturation-with-hlslpixel-shaders-and-wpf/
	/*
	float alphaBackup = res.a;
	float3  LuminanceWeights = float3(0.299,0.587,0.114);
	float    luminance = dot(res,LuminanceWeights);
	res = lerp(luminance, res, color.r);
	// apply color fade
	res *= color.g;
	//retain the alpha
	res.a = alphaBackup * color.a;
	*/
	//res *= color;

	// apply alpha factor
	res *= color.a; 
	return res ;

}

technique Technique1
{
    pass Pass1
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}
