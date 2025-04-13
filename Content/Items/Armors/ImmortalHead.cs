using GloryofGuardian.Common;
using GloryofGuardian.Content.Items.Armor;
using Terraria.ID;
using Terraria.Localization;

namespace GloryofGuardian.Content.Items.Armors
{
    [AutoloadEquip(EquipType.Head)]
    public class ImmortalHead : ModItem
    {
        public override string Texture => GOGConstant.Armors + Name;

        public override void SetDefaults() {
            Item.width = 22;
            Item.height = 22;
            Item.value = 10000;
            Item.rare = ItemRarityID.LightRed;

            Item.defense = 5;
        }

        public override void ArmorSetShadows(Player player) {
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) {
            if (body.type == ModContent.ItemType<ImmortalBody>()) {
                return legs.type == ModContent.ItemType<ImmortalLegs>();
            }
            return false;
        }

        public override void UpdateEquip(Player player)
        {
            Lighting.AddLight(player.Center, 100 * 0.01f, 100 * 0.01f, 100 * 0.01f);
            player.GetModPlayer<GOGModPlayer>().Gslot += 1;
            player.GetDamage<GuardianDamageClass>() += 0.12f;
            player.GetArmorPenetration(DamageClass.Generic) += 5;


        }

        public override void UpdateArmorSet(Player player) {
            player.setBonus = Language.GetTextValue("Mods.GloryofGuardian.ArmorSetBonuses.Immortal.Description");
            //player.GetModPlayer<GOGModPlayer>().   = true;
        }

        //public override Color? GetAlpha(Color lightColor) {
        //    return base.GetAlpha(lightColor);
        //}

        //public override void DrawArmorColor(Player drawPlayer, float shadow, ref Color color, ref int glowMask, ref Color glowMaskColor) {
        //    base.DrawArmorColor(drawPlayer, shadow, ref color, ref glowMask, ref glowMaskColor);
        //}

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.CrystalShard, 20)
                .AddIngredient(ItemID.SoulofNight, 12)
                .AddIngredient(ItemID.UnicornHorn, 1)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
