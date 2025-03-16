using GloryofGuardian.Common;
using GloryofGuardian.Content.Projectiles.HeldProj;
using Terraria.ID;

namespace GloryofGuardian.Content.Items.Weapon
{
    public class GoldTrident : GOGCalling
    {
        public override string Texture => GOGConstant.Weapons + Name;

        public override void SetStaticDefaults() {
        }

        public override void SetDefaults() {
            Item.damage = 10;
            Item.DamageType = GuardianDamageClass.Instance;
            Item.width = 78;
            Item.height = 32;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 0;
            Item.value = Item.buyPrice(platinum: 1, silver: 0, gold: 0, copper: 0);
            Item.rare = ItemRarityID.Blue;
            Item.autoReuse = false;

            Item.shoot = ModContent.ProjectileType<GoldTridentHeld>();
            Item.shootSpeed = 5f;

            Item.channel = true;
            Item.noUseGraphic = true;
            Item.noMelee = true;
            Item.mana = 0;
            Item.scale = 1f;

            Item.staff[Item.type] = true;
        }

        public override Color? GetAlpha(Color lightColor) {
            //return Color.White;
            return null;
        }



        //物品合成表
        public override void AddRecipes() {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Diamond, 2);
            recipe.AddIngredient(ItemID.SunplateBlock, 10);
            recipe.AddRecipeGroup("GloryofGuardian.AnyGoldBar", 5);
            recipe.AddTile(TileID.SkyMill);
            recipe.Register();
        }
    }
}
