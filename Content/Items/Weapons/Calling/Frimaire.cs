using GloryofGuardian.Common;
using GloryofGuardian.Content.Projectiles;
using GloryofGuardian.Content.WeaponClasses;
using Terraria.ID;

namespace GloryofGuardian.Content.Items.Weapons.Calling {
    public class Frimaire : GOGCalling {
        public override string Texture => GOGConstant.Weapons + Name;

        protected override int ProjType => ModContent.ProjectileType<FrimaireDT>();

        protected override int ProjSlot => 1;

        protected override int ProjlNumLimit => -1;

        public override void SetProperty() {
            Item.damage = 120;
            Item.width = 56;
            Item.height = 56;
            Item.knockBack = 6;
            Item.value = Item.buyPrice(platinum: 1, silver: 0, gold: 0, copper: 0);
            Item.rare = ItemRarityID.Yellow;
        }

        public override void AddRecipes() {
            Recipe recipe = CreateRecipe();

            recipe.Register();
        }
    }
}
