global using GloryofGuardian.Content.Class;
global using InnoVault;
global using Microsoft.Xna.Framework;
global using Terraria;
global using Terraria.ModLoader;
using GloryofGuardian.Skies;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;

namespace GloryofGuardian
{
    public class GloryofGuardianMod : Mod
    {
        internal static GloryofGuardianMod Instance;

        public float skycount = 0;

        public override void Load() {
            Instance = this;

            //加载天空
            if (skycount <= 1) skycount -= 0.01f;

            //梦红色
            SkyManager.Instance["LightPinkSky"] = new LightPinkSky();

            //加载Shader

            //终末


            //灰化特效
            Asset<Effect> GrayColorShader = GloryofGuardianMod.Instance.Assets.Request<Effect>("Assets/Effects/Shader/Gray", AssetRequestMode.AsyncLoad);
            Filters.Scene["Gray"] = new Filter(new ScreenShaderData(GrayColorShader, "Gray"), EffectPriority.VeryHigh);
            Filters.Scene["Gray"].Load();

            //拖尾特效,注意string填写的PassName来自fx注册的源码
            Asset<Effect> DrawTailShader = GloryofGuardianMod.Instance.Assets.Request<Effect>("Assets/Effects/Shader/DrawTail", AssetRequestMode.AsyncLoad);
            Filters.Scene["DrawTail"] = new Filter(new ScreenShaderData(DrawTailShader, "DrawTail"), EffectPriority.VeryHigh);
            Filters.Scene["DrawTail"].Load();

            //顶点特效
            Asset<Effect> Trail = GloryofGuardianMod.Instance.Assets.Request<Effect>("Assets/Effects/Shader/Trail");
            Filters.Scene["ColorBar"] = new Filter(new ScreenShaderData(Trail, "ColorBar"), EffectPriority.VeryHigh);
            Filters.Scene["ColorBar"].Load();
        }
    }
}
