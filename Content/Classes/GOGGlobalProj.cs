using GloryofGuardian.Content.Class;

namespace GloryofGuardian.Content.Classes {
    public class GOGGlobalProj : GlobalProjectile {
        //戍卫伤害暴击转超频攻击,目前来说,戍卫伤害一定由弹幕承载
        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers) {
            if (projectile.DamageType == GuardianDamageClass.Instance) {
                modifiers.DisableCrit();//对指定伤害类型取消暴击
            }
        }
    }
}
