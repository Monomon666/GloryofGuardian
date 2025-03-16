using GloryofGuardian.Common;
using Terraria.ID;

namespace GloryofGuardian.Content.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class UndeadWoodBody : ModItem
    {
        public override string Texture => GOGConstant.Armors + Name;

        public override void SetDefaults() {
            Item.width = 44;
            Item.height = 28;
            Item.value = 10000;
            Item.rare = 0;

            Item.defense = 5;
        }

        public override void ArmorSetShadows(Player player) {
        }

        public override void UpdateEquip(Player player) {
            player.statLifeMax2 += 50;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.CrystalShard, 40)
                .AddIngredient(ItemID.SoulofNight, 24)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
