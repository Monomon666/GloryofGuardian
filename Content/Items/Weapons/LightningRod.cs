using GloryofGuardian.Common;
using GloryofGuardian.Content.Projectiles.HeldProj;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Items.Weapon
{
    public class LightningRod : GOGCalling
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

            Item.shoot = ModContent.ProjectileType<LightningRodHeld>();
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

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback) {
            SetDefaults();
            int p = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, PrefixCD(Item), PrefixCrit(Item));
            return false;
        }
    }
}
