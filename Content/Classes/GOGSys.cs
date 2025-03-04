using Terraria.ID;
using Terraria.Localization;

namespace GloryofGuardian.Content.Classes
{
    public class GOGSys : ModSystem
    {
        public static LocalizedText AnyCopperBar = Language.GetOrRegister("Mods.GloryofGuardian.AnyCopperBar", () => "任意铜锭");
        public static LocalizedText AnySilverBar = Language.GetOrRegister("Mods.GloryofGuardian.AnySilverBar", () => "任意银锭");
        public static LocalizedText AnyGoldBar = Language.GetOrRegister("Mods.GloryofGuardian.AnyGoldBar", () => "任意金锭");
        //添加合成组
        public override void AddRecipeGroups() {
            //任意铜锭,银锭,金锭
            RecipeGroup.RegisterGroup("GloryofGuardian.AnyCopperBar", new(() => "GloryofGuardian.AnyCopperBar", ItemID.CopperBar, ItemID.TinBar));
            RecipeGroup.RegisterGroup("GloryofGuardian.AnySilverBar", new(() => "GloryofGuardian.AnySilverBar", ItemID.SilverBar, ItemID.TungstenBar));
            RecipeGroup.RegisterGroup("GloryofGuardian.AnyGoldBar", new(() => "GloryofGuardian.AnyGoldBar", ItemID.GoldBar, ItemID.PlatinumBar));


        }
    }
}
