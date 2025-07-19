using GloryofGuardian.Common;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace GloryofGuardian.Content.Items.Materials
{
    public class NanoBar : ModItem
    {
        public override string Texture => GOGConstant.Items + "Materials/" + Name;
        public override void SetStaticDefaults() {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 99;
        }

        public override void SetDefaults() {
            Item.width = 30;
            Item.height = 24;
            Item.maxStack = 9999;
            Item.value = 10000;
            Item.rare = ItemRarityID.White;
        }

        public override Color? GetAlpha(Color drawColor) {
            return base.GetAlpha(drawColor);
        }

        public override void AddRecipes() {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.ChlorophyteBar, 1);
            recipe.AddIngredient(ItemID.Nanites, 5);
            recipe.AddTile(TileID.SteampunkBoiler);
            recipe.Register();
        }
    }
}
