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
            Item.rare = -12;
            Item.defense = 100;
        }

        public override void ArmorSetShadows(Player player) {
            player.armorEffectDrawShadow = true;
            player.endurance += 0.3f;
        }

        public override void UpdateEquip(Player player) {
        }

        public override void AddRecipes() {
            CreateRecipe()
                .AddIngredient(ItemID.DirtBlock, 1)
                .AddTile(TileID.WorkBenches)
                .Register();
        }
    }
}
