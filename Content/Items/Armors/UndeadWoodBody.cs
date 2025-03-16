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
            Item.rare = ItemRarityID.White;

            Item.defense = 5;
        }

        public override void ArmorSetShadows(Player player) {
        }

        public override void UpdateEquip(Player player) {
            player.statLifeMax2 += 50;
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.Wood, 24)
                .AddTile(TileID.LivingLoom)
                .Register();
        }
    }
}
