using GloryofGuardian.Common;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace GloryofGuardian.Content.Items.Materials
{
    public class SoulofAnotherWorld : ModItem
    {
        public override string Texture => GOGConstant.Items + "Materials/" + Name;
        public override void SetStaticDefaults() {
            Main.RegisterItemAnimation(Item.type, new DrawAnimationVertical(9, 4));
            ItemID.Sets.AnimatesAsSoul[Item.type] = true;
            ItemID.Sets.ItemIconPulse[Item.type] = true;
            ItemID.Sets.ItemNoGravity[Item.type] = true;

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 5;
        }

        public override void SetDefaults() {
            Item.width = 18;
            Item.height = 18;
            Item.maxStack = 9999;
            Item.value = 10000;
            Item.rare = ItemRarityID.LightPurple;
        }

        public override Color? GetAlpha(Color drawColor) {
            return base.GetAlpha(drawColor);
        }

        public override void PostUpdate() {
            SetStaticDefaults();
            Lighting.AddLight(Item.Center, Color.WhiteSmoke.ToVector3() * 0.55f * Main.essScale);
        }
    }
}
