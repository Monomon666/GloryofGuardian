using GloryofGuardian.Common;
using GloryofGuardian.Content.Projectiles;
using GloryofGuardian.Content.WeaponClasses;
using Terraria.ID;

namespace GloryofGuardian.Content.Items.Weapons.Calling {
    public class PaleClockworkGun : GOGCalling {
        public override string Texture => GOGConstant.Weapons + Name;

        protected override int ProjType => ModContent.ProjectileType<PaleClockworkGunDT>();

        protected override int ProjSlot => 1;

        protected override int ProjlNumLimit => -1;

        public override void SetProperty() {
            Item.damage = 40;
            Item.width = 60;
            Item.height = 64;
            Item.knockBack = 2;
            Item.value = Item.buyPrice(platinum: 1, silver: 0, gold: 0, copper: 0);
            Item.rare = ItemRarityID.LightRed;
        }

        public override void AddRecipes() {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.DirtBlock, 10);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}
