using Terraria.ID;
using Terraria.Localization;

namespace GloryofGuardian.Content.Classes {
    public class GOGSys : ModSystem {
        public static LocalizedText AnyCopperBar = Language.GetOrRegister("Mods.GloryofGuardian.AnyCopperBar", () => "任意铜锭");
        public static LocalizedText AnyIronBar = Language.GetOrRegister("Mods.GloryofGuardian.AnyIronBar", () => "任意铁锭");
        public static LocalizedText AnySilverBar = Language.GetOrRegister("Mods.GloryofGuardian.AnySilverBar", () => "任意银锭");
        public static LocalizedText AnyGoldBar = Language.GetOrRegister("Mods.GloryofGuardian.AnyGoldBar", () => "任意金锭");
        public static LocalizedText AnyEvilMaterials = Language.GetOrRegister("Mods.GloryofGuardian.AnyGoldBar", () => "任意邪恶材料");
        public static LocalizedText AnyDungeonFactionFlags = Language.GetOrRegister("Mods.GloryofGuardian.AnyDungeonFactionFlags", () => "任意地牢装饰旗帜");
        //添加合成组
        public override void AddRecipeGroups() {
            //任意铜锭,铁锭,银锭,金锭
            RecipeGroup.RegisterGroup("GloryofGuardian.AnyCopperBar", new(() => AnyCopperBar.Value, ItemID.CopperBar, ItemID.TinBar));
            RecipeGroup.RegisterGroup("GloryofGuardian.AnyIronBar", new(() => AnyIronBar.Value, ItemID.IronBar, ItemID.LeadBar));
            RecipeGroup.RegisterGroup("GloryofGuardian.AnySilverBar", new(() => AnySilverBar.Value, ItemID.SilverBar, ItemID.TungstenBar));
            RecipeGroup.RegisterGroup("GloryofGuardian.AnyGoldBar", new(() => AnyGoldBar.Value, ItemID.GoldBar, ItemID.PlatinumBar));
            //任意邪恶材料
            RecipeGroup.RegisterGroup("GloryofGuardian.AnyEvilMaterials", new(() => AnyEvilMaterials.Value, ItemID.TissueSample, ItemID.ShadowScale));
            //任意地牢装饰旗帜
            RecipeGroup.RegisterGroup("GloryofGuardian.AnyDungeonFactionFlags", new(() => AnyDungeonFactionFlags.Value, ItemID.MarchingBonesBanner, ItemID.NecromanticSign, ItemID.RustedCompanyStandard, ItemID.RaggedBrotherhoodSigil, ItemID.MoltenLegionFlag, ItemID.DiabolicSigil));
        }

        //合成
        public override void PostAddRecipes() {
            //添加
            {
                //植物纤维绳索宝典 合成
                Recipe.Create(ItemID.CordageGuide)
                        .AddIngredient(ItemID.Book, 4)
                        .AddTile(TileID.Vines)
                        .Register();

                //生命木织机 合成
                Recipe.Create(ItemID.LivingLoom)
                        .AddIngredient(ItemID.Wood, 12)
                        .AddIngredient(ItemID.CordageGuide, 1)
                        .AddTile(TileID.Sawmill)
                        .Register();
            }
        }
    }
}
