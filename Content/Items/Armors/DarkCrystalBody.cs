using GloryofGuardian.Common;
using Terraria.ID;

namespace GloryofGuardian.Content.Items.Armor
{
    [AutoloadEquip(EquipType.Body)]
    public class DarkCrystalBody : ModItem
    {
        public override string Texture => GOGConstant.Armors + Name;

        public override void SetDefaults() {
            Item.width = 44;
            Item.height = 28;
            Item.value = 10000;
            Item.rare = ItemRarityID.LightRed;

            Item.defense = 7;
        }

        public override void ArmorSetShadows(Player player) {
            player.endurance += 0.3f;
        }

        public override void UpdateEquip(Player player) {
            Lighting.AddLight(player.Center, 120 * 0.01f, 66 * 0.01f, 181 * 0.01f);
            player.GetAttackSpeed(DamageClass.Generic) -= 0.25f;
            player.GetAttackSpeed(GuardianDamageClass.Instance) -= 0.25f;
            player.statLifeMax2 += 100;
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
