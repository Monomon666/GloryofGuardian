﻿using GloryofGuardian.Common;
using GloryofGuardian.Content.Projectiles;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace GloryofGuardian.Content.Items.Weapon
{
    public class SoulDementorBookCalling : GOGCalling
    {
        public override string Texture => GOGConstant.Weapons + Name;

        public override void SetStaticDefaults() {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
        }

        public override void SetDefaults() {
            Item.damage = 66;
            Item.DamageType = GuardianDamageClass.Instance;
            Item.width = 56;
            Item.height = 56;
            Item.useTime = 60;
            Item.useAnimation = 60;
            Item.useStyle = ItemUseStyleID.RaiseLamp;
            Item.knockBack = 6;
            Item.value = Item.buyPrice(platinum: 1, silver: 0, gold: 0, copper: 0);
            Item.rare = ItemRarityID.Yellow;
            Item.UseSound = SoundID.DD2_DefenseTowerSpawn;
            Item.autoReuse = false;

            Item.shoot = ModContent.ProjectileType<SoulDementorBookProj>();
            Item.shootSpeed = 5f;

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
            if (player.altFunctionUse == 0) {
                if (player.statLife < 40) {
                    CombatText.NewText(player.Hitbox,
                            Color.Red,
                            "鲜血不足",//这里是你需要展示的文字
                            false,
                            true
                            );
                    return false;
                }
            }

            if (player.altFunctionUse == 0) {
                Item.damage = 75;
                Item.shootSpeed = 12f;
                Item.UseSound = null;
                Item.useTime = 60;
                Item.useAnimation = 60;
                Item.noUseGraphic = false;
                return true;
            }
            return false;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
            if (player.altFunctionUse == 0) {//摄魂
                player.statLife -= 40;
                CombatText.NewText(player.Hitbox,
                            Color.Red,
                            "献祭",//这里是你需要展示的文字
                            false,
                            true
                            );
                CombatText.NewText(player.Hitbox,
                            Color.Red,
                            "40",//这里是你需要展示的文字
                            false,
                            true
                            );

                Projectile.NewProjectile(source, position + velocity.SafeNormalize(Vector2.Zero) * 48f, velocity, type, damage, knockback, player.whoAmI);
                return false;
            } else return false;
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
