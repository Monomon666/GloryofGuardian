sampler uImage0 : register(s0);//gd.Texture[0]
sampler uImage1 : register(s1);//gd.Texture[1]
//以亮度为判据实现纹理覆盖
float m;
float n;
float OffsetX = 0;//X偏移传入
float OffsetY = 0; //Y偏移传入
float4 color0;
float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    float4 c = tex2D(uImage0,coords);
	float a = max(c.r,max(c.g,c.b));
    if(a > m)//明度超过m的部分被替换为背景图片
	{
        float4 c1 = tex2D(uImage1, float2((coords.x + OffsetX) % 1, (coords.y + OffsetY) % 1));
		return c1;
	}
	else if(abs(a - m) < n)//明度与m差值小于n的部分，替换为纯色当作描边
        return color0;
	else
		return c*a;
}

technique Technique1
{
	pass Tentacle
	{
		PixelShader = compile ps_2_0 PixelShaderFunction();
	}
   
}