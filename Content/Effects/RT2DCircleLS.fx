sampler uImage0 : register(s0);//gd.Texture[0]//光效
sampler uImage1 : register(s1);//gd.Texture[1]//纹理
//使用圆形光源进行纹理显形
float OffsetX = 0;//X偏移传入
float OffsetY = 0;//Y偏移传入
float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 c = tex2D(uImage0,coords);
	float a = max(c.r,max(c.g,c.b));
	
    float4 c1 = tex2D(uImage1, float2((coords.x + OffsetX) % 1, (coords.y + OffsetY) % 1));
    return c1 * a;
}

technique Technique1
{
	pass Tentacle
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
   
}