using GloryofGuardian.Common;
using GloryofGuardian.Content.Projectiles;
using GloryofGuardian.Content.WeaponClasses;
using Terraria.ID;

namespace GloryofGuardian.Content.Items.Weapons.Calling {
    public class BeeHive : GOGCalling {
        public override string Texture => GOGConstant.Weapons + Name;

        protected override int ProjType => ModContent.ProjectileType<BeeHiveDT>();

        protected override int ProjSlot => 1;

        protected override int ProjlNumLimit => 1;

        public override void SetProperty() {
            Item.damage = 0;
            Item.width = 56;
            Item.height = 56;
            Item.knockBack = 1;
            Item.value = Item.buyPrice(platinum: 1, silver: 0, gold: 0, copper: 0);
            Item.rare = ItemRarityID.Orange;

            //独一炮塔召唤速度减缓
            Item.useTime = 60;
            Item.useAnimation = 60;
        }

        public override void AddRecipes() {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.BeeWax, 8);
            recipe.AddIngredient(ItemID.HoneyBlock, 2);
            recipe.AddIngredient(ItemID.Hive, 4);
            recipe.AddIngredient(ItemID.HoneyBucket, 1);
            recipe.AddTile(TileID.HoneyDispenser);
            recipe.Register();
        }
    }
}
