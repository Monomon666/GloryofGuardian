using GloryofGuardian.Common;
using GloryofGuardian.Content.Projectiles;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace GloryofGuardian.Content.Items.Weapon
{
    public class PinkSlimeCalling : GOGCalling
    {
        public override string Texture => GOGConstant.Weapons + Name;

        public override void SetStaticDefaults() {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
            ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
            ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
        }

        public override void SetDefaults() {
            Item.damage = 50;
            Item.DamageType = GuardianDamageClass.Instance;
            Item.width = 80;
            Item.height = 80;
            Item.useTime = 45;
            Item.useAnimation = 45;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 6;
            Item.value = Item.buyPrice(platinum: 1, silver: 0, gold: 0, copper: 0);
            Item.rare = ItemRarityID.LightRed;
            Item.UseSound = SoundID.Item54;
            Item.autoReuse = true;

            Item.shoot = ModContent.ProjectileType<PinkSlimeCallProj>();
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
            if (player.altFunctionUse == 0) Projectile.NewProjectile(source, position + velocity.SafeNormalize(Vector2.Zero) * 48f, velocity, type, damage, knockback, player.whoAmI, 0, PrefixCrit(Item));
            if (player.altFunctionUse == 2) Projectile.NewProjectile(source, position + velocity.SafeNormalize(Vector2.Zero) * 48f, velocity, type, damage, knockback, player.whoAmI, 1, PrefixCrit(Item));
            return false;
        }

        public override Color? GetAlpha(Color lightColor) {
            //return Color.White;
            return null;
        }

        public override void AddRecipes() {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<SlimeCalling>(), 1);
            recipe.AddIngredient(ItemID.GelBalloon, 20);
            recipe.AddIngredient(ItemID.SoulofLight, 10);
            recipe.AddIngredient(ItemID.CrystalShard, 10);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
    }
}
