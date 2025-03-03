using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Class
{
    public class GOGGlobalProj : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

        //暗影卷轴偏移角
        public float ShadowAngleOffset = 0;

        //戍卫伤害暴击转过载攻击
        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers) {
            if (projectile.DamageType == GuardianDamageClass.Instance) {
                modifiers.DisableCrit();//对指定伤害类型取消暴击
            }
        }

        public override void OnSpawn(Projectile projectile, IEntitySource source) {
            base.OnSpawn(projectile, source);
        }

        public override void AI(Projectile projectile) {
            if (ShadowAngleOffset != 0) {
                for (int j = 0; j < 2; j++) {
                    int num1 = Dust.NewDust(projectile.position, projectile.width, projectile.height, DustID.PurpleTorch, 0f, 0f, 10, Color.White, 1f);
                    Main.dust[num1].noGravity = true;
                    Main.dust[num1].velocity *= 3f;
                }
            }

            base.AI(projectile);
        }

        public override void PostAI(Projectile projectile) {
            base.PostAI(projectile);
        }

        public override void OnHitNPC(Projectile projectile, NPC target, NPC.HitInfo hit, int damageDone) {
            base.OnHitNPC(projectile, target, hit, damageDone);
        }
    }
}
