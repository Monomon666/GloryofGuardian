using GloryofGuardian.Common;
using GloryofGuardian.Content.Projectiles.HeldProj;
using Terraria.ID;

namespace GloryofGuardian.Content.Items.Weapon
{
    public class MaliceFireGun : GOGCalling
    {
        public override string Texture => GOGConstant.Weapons + Name;

        public override void SetStaticDefaults() {
        }

        public override void SetDefaults() {
            Item.damage = 10;
            Item.DamageType = GuardianDamageClass.Instance;
            Item.width = 78;
            Item.height = 32;
            Item.useTime = 20;
            Item.useAnimation = 20;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 0;
            Item.value = Item.buyPrice(platinum: 1, silver: 0, gold: 0, copper: 0);
            Item.rare = ItemRarityID.LightRed;
            Item.autoReuse = false;

            Item.shoot = ModContent.ProjectileType<MaliceFireGunHeld>();
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
    }
}
