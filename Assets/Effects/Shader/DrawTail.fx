sampler uImage0 : register(s0); // ��������Ļ����
sampler uImage1 : register(s1); // ��β������ѡ��
sampler uImage2 : register(s2); // ����������ѡ��
sampler uImage3 : register(s3); // ����������ѡ��

float3 uColor; // ��Ļ������ɫ
float3 uSecondaryColor; // ��Ļ�Ĵ���ɫ��������β��
float2 uScreenResolution; // ��Ļ�ֱ���
float2 uScreenPosition; // ��Ļ���Ͻ�����
float2 uTargetPosition; // ��Ļ��Ŀ��λ��
float2 uDirection; // ��Ļ���ƶ�����
float uOpacity; // ��Ļ��͸����
float uTime; // ʱ����������ڶ�̬Ч����
float uIntensity; // ��βǿ��
float uProgress; // �ⲿ����Ĳ�ֵ�����ڿ�����β���ȣ�
float2 uImageSize1; // ����1�ĳߴ�
float2 uImageSize2; // ����2�ĳߴ�
float2 uImageSize3; // ����3�ĳߴ�
float2 uImageOffset; // ����ƫ��
float uSaturation; // ���Ͷ�
float4 uSourceRect; // �����Դ����
float2 uZoom; // ����

// �Զ������
float Edge; // ��Ե����

// ��β��ɫ���㺯��
float3 CalculateTrailColor(float2 coords, float progress)
{
    // ���ݵ�Ļ������ɫ�ʹ���ɫ��ֵ
    float3 trailColor = lerp(uColor, uSecondaryColor, progress);
    return trailColor;
}

// ������ɫ��
float4 PixelShaderFunction(float2 coords : TEXCOORD0) : COLOR0
{
    // ���㵯Ļ����Ļ����λ��
    float2 targetInScreenPos = (uTargetPosition - uScreenPosition);
    // ���㵱ǰ���ص���Ļ��������
    float2 pixelPos = (coords * uScreenResolution - targetInScreenPos);
    // ���㵱ǰ�����뵯Ļ���ĵľ���
    float r = length(pixelPos);
    // ������β�Ĳ�ֵ����
    float factor = clamp(r * uProgress / 1920 - Edge, 0, 1);

    // ������Ļ����
    float4 color = tex2D(uImage0, coords);

    // ������β��ɫ
    float3 trailColor = CalculateTrailColor(coords, factor);

    // ��ϵ�Ļ��ɫ����β��ɫ
    float3 finalColor = lerp(color.rgb, trailColor, factor * uIntensity);

    // ����������ɫ��͸�����ɵ�Ļ�����alpha��uOpacity���ƣ�
    return float4(finalColor, color.a * uOpacity);
}

// ������
technique Technique1
{
    pass DrawTail
    {
        PixelShader = compile ps_2_0 PixelShaderFunction();
    }
}