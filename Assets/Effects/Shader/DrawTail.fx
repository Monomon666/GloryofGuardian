sampler uImage0 : register(s0); // 主纹理（弹幕纹理）
sampler uImage1 : register(s1); // 拖尾纹理（可选）
sampler uImage2 : register(s2); // 其他纹理（可选）
sampler uImage3 : register(s3); // 其他纹理（可选）

float3 uColor; // 弹幕的主颜色
float3 uSecondaryColor; // 弹幕的次颜色（用于拖尾）
float2 uScreenResolution; // 屏幕分辨率
float2 uScreenPosition; // 屏幕左上角坐标
float2 uTargetPosition; // 弹幕的目标位置
float2 uDirection; // 弹幕的移动方向
float uOpacity; // 弹幕的透明度
float uTime; // 时间变量（用于动态效果）
float uIntensity; // 拖尾强度
float uProgress; // 外部传入的插值（用于控制拖尾长度）
float2 uImageSize1; // 纹理1的尺寸
float2 uImageSize2; // 纹理2的尺寸
float2 uImageSize3; // 纹理3的尺寸
float2 uImageOffset; // 纹理偏移
float uSaturation; // 饱和度
float4 uSourceRect; // 纹理的源矩形
float2 uZoom; // 缩放

// 自定义变量
float Edge; // 边缘控制

// 拖尾颜色计算函数
float3 CalculateTrailColor(float2 coords, float progress)
{
    // 根据弹幕的主颜色和次颜色插值
    float3 trailColor = lerp(uColor, uSecondaryColor, progress);
    return trailColor;
}

// 像素着色器
float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    // 计算弹幕的屏幕像素位置
    float2 targetInScreenPos = (uTargetPosition - uScreenPosition);
    // 计算当前像素的屏幕像素坐标
    float2 pixelPos = (coords * uScreenResolution - targetInScreenPos);
    // 计算当前像素与弹幕中心的距离
    float r = length(pixelPos);
    // 计算拖尾的插值因子
    float factor = clamp(r * uProgress / 1920 - Edge, 0, 1);

    // 采样弹幕纹理
    float4 color = tex2D(uImage0, coords);

    // 计算拖尾颜色
    float3 trailColor = CalculateTrailColor(coords, factor);

    // 混合弹幕颜色和拖尾颜色
    float3 finalColor = lerp(color.rgb, trailColor, factor * uIntensity);

    // 返回最终颜色（透明度由弹幕纹理的alpha和uOpacity控制）
    return float4(finalColor, color.a * uOpacity);
}

// 技术块
technique Technique1
{
    pass DrawTail
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}