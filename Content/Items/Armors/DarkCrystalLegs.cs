using GloryofGuardian.Common;
using Terraria.ID;

namespace GloryofGuardian.Content.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class DarkCrystalLegs : ModItem
    {
        public override string Texture => GOGConstant.Armors + Name;

        public override void SetDefaults() {
            Item.width = 30;
            Item.height = 16;
            Item.value = 10000;
            Item.rare = ItemRarityID.LightRed;

            Item.defense = 7;
        }

        public override void ArmorSetShadows(Player player) {
        }

        public override void UpdateEquip(Player player) {
            Lighting.AddLight(player.Center, 120 * 0.01f, 66 * 0.01f, 181 * 0.01f);
            player.GetModPlayer<GOGModPlayer>().Gslot += 1;
            player.GetDamage<GuardianDamageClass>() += 0.06f;
            player.GetCritChance<GenericDamageClass>() += 6;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.CrystalShard, 30)
                .AddIngredient(ItemID.SoulofNight, 18)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
