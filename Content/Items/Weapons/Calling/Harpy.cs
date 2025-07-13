using GloryofGuardian.Common;
using GloryofGuardian.Content.Projectiles;
using GloryofGuardian.Content.WeaponClasses;
using Terraria.ID;

namespace GloryofGuardian.Content.Items.Weapons.Calling {
    public class Harpy : GOGCalling {
        public override string Texture => GOGConstant.Weapons + Name;

        protected override int ProjType => ModContent.ProjectileType<BloodyDT>();

        protected override int ProjSlot => 1;

        protected override int ProjlNumLimit => -1;

        public override void SetProperty() {
            Item.damage = 20;
            Item.width = 56;
            Item.height = 56;
            Item.knockBack = 0;
            Item.value = Item.buyPrice(platinum: 1, silver: 0, gold: 0, copper: 0);
            Item.rare = ItemRarityID.LightPurple;
        }

        public override void AddRecipes() {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Cloud, 5);
            recipe.AddIngredient(ItemID.RainCloud, 5);
            recipe.AddIngredient(ItemID.SoulofFlight, 10);
            recipe.AddIngredient(ItemID.Feather, 10);
            recipe.AddIngredient(ItemID.GiantHarpyFeather, 1);
            recipe.AddTile(TileID.SkyMill);
            recipe.Register();
        }
    }
}
