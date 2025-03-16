using GloryofGuardian.Common;
using Terraria.ID;

namespace GloryofGuardian.Content.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class NanoArmorBody : ModItem
    {
        public override string Texture => GOGConstant.Armors + Name;

        public override void SetDefaults() {
            Item.width = 44;
            Item.height = 28;
            Item.value = 10000;
            Item.rare = ItemRarityID.Yellow;

            Item.defense = 24;
        }

        public override void ArmorSetShadows(Player player) {
            player.armorEffectDrawShadow = true;
            player.endurance += 0.3f;
        }

        public override void UpdateEquip(Player player) {
            Lighting.AddLight(player.Center, 255 * 0.005f, 255 * 0.005f, 255 * 0.005f);
            player.GetAttackSpeed(DamageClass.Generic) -= 0.25f;
            player.GetAttackSpeed(GuardianDamageClass.Instance) -= 0.25f;
            player.statLifeMax2 += 200;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.Nanites, 50)
                .AddIngredient(ItemID.HallowedBar, 20)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
