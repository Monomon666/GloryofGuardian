using GloryofGuardian.Content.Projectiles;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Class
{
    public abstract class GOGProj : ModProjectile
    {
        //倒钩箭头倒钩标记
        public bool ReverseHookArrow = false;

        public override void OnSpawn(IEntitySource source) {
            base.OnSpawn(source);
        }

        //山铜强化标记
        public bool OrichalcumMarkProj;
        public int OrichalcumMarkProjcount;
        public override void AI() {
            base.AI();
        }

        public override void PostAI() {
            if (OrichalcumMarkProjcount > 0) OrichalcumMarkProjcount -= 0;//我们目前决定不清除强化
            if (OrichalcumMarkProjcount == 0) OrichalcumMarkProj = false;

            //山铜强化粒子特效
            if (OrichalcumMarkProj
                //黑名单，钉入弩箭不能享受
                && Projectile.type != ModContent.ProjectileType<WildProj>()
                && Projectile.type != ModContent.ProjectileType<GarrisonProj>()
                && Projectile.type != ModContent.ProjectileType<MythrilProj2>()
                ) {
                for (int i = 0; i <= 1 && Projectile.width > 2 && Projectile.height > 2; i++) {
                    Dust dust1 = Dust.NewDustDirect(Projectile.position, Projectile.width, Projectile.height, DustID.TeleportationPotion, 1f, 1f, 100, new Color(239, 113, 248), 1.2f);
                    dust1.velocity = Projectile.velocity;
                    dust1.noGravity = true;
                }
                for (int i = 0; i <= 1 && Projectile.width <= 2 && Projectile.height <= 2; i++) {
                    Dust dust1 = Dust.NewDustDirect(Projectile.position + new Vector2(-2, -2), 2, 2, DustID.TeleportationPotion, 1f, 1f, 100, new Color(239, 113, 248), 1.2f);
                    dust1.velocity = Projectile.velocity;
                    dust1.noGravity = true;
                }
            }
            base.PostAI();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            //山铜强化追加伤害
            if (OrichalcumMarkProj) {
                int Oriexdamage = (int)MathHelper.Max(1, (Projectile.originalDamage * Main.player[Projectile.owner].GetTotalDamage(GuardianDamageClass.Instance).ApplyTo(100) / 100f * 0.1f));
                if (target.life >= Oriexdamage) target.life -= Oriexdamage;
                if (target.life < Oriexdamage) target.life = 1;
                CombatText.NewText(target.Hitbox,
                                    new Color(239, 113, 248),
                                    Oriexdamage,
                                    false,
                                    false 
                                    );
            }
            base.OnHitNPC(target, hit, damageDone);
        }
    }
}