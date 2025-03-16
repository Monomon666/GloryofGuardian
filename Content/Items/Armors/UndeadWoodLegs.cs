using GloryofGuardian.Common;
using Terraria.ID;

namespace GloryofGuardian.Content.Items.Armor
{
    [AutoloadEquip(EquipType.Legs)]
    public class UndeadWoodLegs : ModItem
    {
        public override string Texture => GOGConstant.Armors + Name;

        public override void SetDefaults() {
            Item.width = 30;
            Item.height = 16;
            Item.value = 10000;
            Item.rare = ItemRarityID.White;

            Item.defense = 3;
        }

        public override void ArmorSetShadows(Player player) {
        }

        public override void UpdateEquip(Player player) {
            player.GetDamage<GuardianDamageClass>() += 0.06f;
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
