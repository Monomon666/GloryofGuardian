using GloryofGuardian.Common;
using GloryofGuardian.Content.Items.Armor;
using Terraria.ID;
using Terraria.Localization;

namespace GloryofGuardian.Content.Items.Armors
{
    [AutoloadEquip(EquipType.Head)]
    public class NanoArmorHead : ModItem
    {
        public override string Texture => GOGConstant.Armors + Name;

        public override void SetDefaults() {
            Item.width = 22;
            Item.height = 22;
            Item.value = 10000;
            Item.rare = ItemRarityID.Yellow;

            Item.defense = 12;
        }

        public override void ArmorSetShadows(Player player) {
            player.armorEffectDrawShadow = true;

            //player.armorEffectDrawShadowBasilisk = true;
            //player.armorEffectDrawShadowEOCShield = true;
            //player.armorEffectDrawShadowLokis = true;
            player.armorEffectDrawShadowSubtle = true;
            player.armorEffectDrawOutlines = true;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs) {
            if (body.type == ModContent.ItemType<NanoArmorBody>()) {
                return legs.type == ModContent.ItemType<NanoArmorLegs>();
            }
            return false;
        }

        public override void UpdateEquip(Player player) {
            Lighting.AddLight(player.Center, 255 * 0.005f, 255 * 0.005f, 255 * 0.005f);
            player.GetModPlayer<GOGModPlayer>().Gslot += 1;
            player.GetDamage<GuardianDamageClass>() += 0.16f;
            player.GetArmorPenetration(DamageClass.Generic) += 10;
        }

        public override void UpdateArmorSet(Player player) {
            player.setBonus = Language.GetTextValue("Mods.GloryofGuardian.ArmorSetBonuses.Nano.Description");

            if (player.statLife < player.statLifeMax2 / 2) player.endurance = 0.15f;
            if (player.statLife > player.statLifeMax2 / 2) player.GetDamage<GuardianDamageClass>() += 0.15f;
            if (player.statLife > player.statLifeMax2 / 2) player.GetDamage<GenericDamageClass>() += 0.15f;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.Nanites, 30)
                .AddIngredient(ItemID.HallowedBar, 12)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
