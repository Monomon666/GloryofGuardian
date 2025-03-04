using GloryofGuardian.Common;
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
            Item.useTime = 90;
            Item.useAnimation = 90;
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
            if (player.altFunctionUse == 2) {
                Item.UseSound = null;
                return false;
            }

            Item.UseSound = SoundID.Item155;
            if (player.altFunctionUse == 0) {
                if (player.GetModPlayer<GOGModPlayer>().Gslot == 0) {
                    CombatText.NewText(player.Hitbox,//跳字生成的矩形范围
                            Color.Red,//跳字的颜色
                            "戍卫栏不足",//这里是你需要展示的文字
                            false,//dramatic为true可以使得字体闪烁，
                            true //dot为true可以使得字体略小，跳动方式也不同(原版debuff扣血格式)
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
            if (player.altFunctionUse == 0) Projectile.NewProjectile(source, position + velocity.SafeNormalize(Vector2.Zero) * 48f, velocity, type, damage, knockback, player.whoAmI, 1, PrefixCrit());
            return false;
        }

        /// <summary>
        /// 用来特判和传递来自武器前缀的暴击加成
        /// </summary>
        float PrefixCrit() {
            if (Item.prefix == ModContent.PrefixType<Excellent0>()
                || Item.prefix == ModContent.PrefixType<Sensitive0>()
                ) return 0f;
            if (Item.prefix == ModContent.PrefixType<Peerless0>()
                || Item.prefix == ModContent.PrefixType<Classic0>()
                || Item.prefix == ModContent.PrefixType<Silent0>()
                ) return 5f;
            if (Item.prefix == ModContent.PrefixType<Overclocked0>()
                || Item.prefix == ModContent.PrefixType<Burdened0>()
                ) return 10f;
            if (Item.prefix == ModContent.PrefixType<Blooey0>()
                || Item.prefix == ModContent.PrefixType<ShortCircuited0>()
                ) return 20f;
            if (Item.prefix == ModContent.PrefixType<Scrapped0>()
                ) return -5f;
            if (Item.prefix == ModContent.PrefixType<Precise0>()
                || Item.prefix == ModContent.PrefixType<Damaged0>()
                ) return -10f;
            if (Item.prefix == ModContent.PrefixType<Stainless0>()
                ) return -20f;
            return 0;
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
