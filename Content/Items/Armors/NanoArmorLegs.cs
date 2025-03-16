using GloryofGuardian.Common;
using Terraria.ID;

namespace GloryofGuardian.Content.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class NanoArmorLegs : ModItem
    {
        public override string Texture => GOGConstant.Armors + Name;

        public override void SetDefaults() {
            Item.width = 30;
            Item.height = 16;
            Item.value = 10000;
            Item.rare = ItemRarityID.Yellow;

            Item.defense = 14;
        }

        public override void ArmorSetShadows(Player player) {
            player.armorEffectDrawShadow = true;
        }

        public override void UpdateEquip(Player player) {
            Lighting.AddLight(player.Center, 255 * 0.005f, 255 * 0.005f, 255 * 0.005f);
            player.GetModPlayer<GOGModPlayer>().Gslot += 1;
            player.GetCritChance<GenericDamageClass>() += 12;
            player.runAcceleration *= 20f;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.Nanites, 40)
                .AddIngredient(ItemID.HallowedBar, 16)
                .AddTile(TileID.MythrilAnvil)
                .Register();
        }
    }
}
