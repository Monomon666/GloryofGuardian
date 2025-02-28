using Terraria.DataStructures;

namespace GloryofGuardian.Content.Class
{
    public class GOGGlobaProj : GlobalProjectile
    {
        public override bool InstancePerEntity => true;

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
