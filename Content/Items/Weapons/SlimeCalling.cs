﻿using GloryofGuardian.Common;
using GloryofGuardian.Content.Projectiles;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace GloryofGuardian.Content.Items.Weapon
{
    public class SlimeCalling : GOGCalling
    {
        public override string Texture => GOGConstant.Weapons + Name;

        public override void SetStaticDefaults() {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
        }

        public override void SetDefaults() {
            Item.damage = 10;
            Item.DamageType = GuardianDamageClass.Instance;
            Item.width = 64;
            Item.height = 66;
            Item.useTime = 75;
            Item.useAnimation = 75;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 9;
            Item.value = Item.buyPrice(platinum: 1, silver: 0, gold: 0, copper: 0);
            Item.rare = ItemRarityID.Blue;
            Item.UseSound = SoundID.Item54;
            Item.autoReuse = true;

            Item.shoot = ModContent.ProjectileType<SlimeCallProj>();
            Item.shootSpeed = 12f;

            Item.channel = true;
            Item.noUseGraphic = false;
            Item.noMelee = true;
            Item.mana = 0;
            Item.scale = 1f;

            Item.staff[Item.type] = true;
        }

        public override bool AltFunctionUse(Player player) {
            return true;
        }

        public override bool CanUseItem(Player player) {
            SetDefaults();
            if (player.altFunctionUse == 2) {
                Item.UseSound = null;
                return false;
            }

            Item.UseSound = SoundID.Item155;
            if (player.altFunctionUse == 0) {
                if (player.GetModPlayer<GOGModPlayer>().Gslot == 0) {
                    CombatText.NewText(player.Hitbox,
                            Color.Red,
                            "戍卫栏不足",
                            false,
                            true
                            );
                    return false;
                }
            }

            Item.noUseGraphic = false;

            if (player.altFunctionUse == 0) {
                Item.shootSpeed = 12f;
            }

            if (player.altFunctionUse == 2) {
                Item.shootSpeed = 8f;
            }

            return base.CanUseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
            if (player.altFunctionUse == 0) Projectile.NewProjectile(source, position + velocity.SafeNormalize(Vector2.Zero) * 48f, velocity, type, damage, knockback, player.whoAmI, 1, PrefixCrit(Item));
            return false;
        }

        public override Color? GetAlpha(Color lightColor) {
            //return Color.White;
            return null;
        }

        public override void AddRecipes() {
            Recipe recipe = CreateRecipe();
            recipe.AddRecipeGroup("GloryofGuardian.AnyGoldBar", 5);
            recipe.AddIngredient(ItemID.Gel, 20);
            recipe.AddIngredient(ItemID.RoyalGel, 1);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
