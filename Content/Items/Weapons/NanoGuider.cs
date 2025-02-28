﻿using GloryofGuardian.Common;
using GloryofGuardian.Content.Projectiles.HeldProj;
using Terraria.ID;

namespace GloryofGuardian.Content.Items.Weapon
{
    public class NanoGuider : GOGCalling
    {
        public override string Texture => GOGConstant.Weapons + "NanoGuiderItem";

        public override void SetStaticDefaults() {
        }

        public override void SetDefaults() {
            Item.damage = 50;
            Item.DamageType = GuardianDamageClass.Instance;
            Item.width = 78;
            Item.height = 32;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = Item.buyPrice(platinum: 1, silver: 0, gold: 0, copper: 0);
            Item.rare = -13;
            Item.autoReuse = false;

            Item.shoot = ModContent.ProjectileType<NanoGuiderHeld>();
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

        public override void AddRecipes() {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.DirtBlock, 10);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}
