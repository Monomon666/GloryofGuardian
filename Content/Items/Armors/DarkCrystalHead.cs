using GloryofGuardian.Common;
using GloryofGuardian.Content.Items.Armor;
using Terraria.ID;

namespace GloryofGuardian.Content.Items.Armors
{
    [AutoloadEquip(EquipType.Head)]
    public class DarkCrystalHead : ModItem
    {
        public override string Texture => GOGConstant.Armors + Name;

        public override void SetDefaults() {
            Item.width = 22;
            Item.height = 22;
            Item.value = 10000;
            Item.rare = -12;
            Item.defense = 100;
        }

        public override void ArmorSetShadows(Player player) {
            //player.armorEffectDrawShadow = true;

            //player.armorEffectDrawShadowBasilisk = true;
            //player.armorEffectDrawShadowEOCShield = true;
            //player.armorEffectDrawShadowLokis = true;
            //player.armorEffectDrawShadowSubtle = true;
            //player.armorEffectDrawOutlines = true;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) {
            if (body.type == ModContent.ItemType<DarkCrystalBody>()) {
                return legs.type == ModContent.ItemType<DarkCrystalLegs>();
            }
            return false;
        }

        public override void UpdateEquip(Player player) {

        }

        public override void UpdateArmorSet(Player player) {

        }

        //public override Color? GetAlpha(Color lightColor) {
        //    return base.GetAlpha(lightColor);
        //}

        //public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor) {
        //    base.DrawArmorColor(drawPlayer, shadow, ref color, ref glowMask, ref glowMaskColor);
        //}

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.DirtBlock, 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
