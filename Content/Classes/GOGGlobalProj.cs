using System;
using GloryofGuardian.Content.Class;
using GloryofGuardian.Content.Projectiles;
using Terraria.ID;

namespace GloryofGuardian.Content.Classes {
    public class GOGGlobalProj : GlobalProjectile {
        public override bool InstancePerEntity => true;

        //属性
        /// <summary>
        /// 暗影炎印记
        /// </summary>
        public bool ShadowFire = false;
        public Vector2 ShadowVec = Vector2.Zero;
        public int ShadowCount = 0;

        public override void PostAI(Projectile projectile) {
            //暗影炎印记减速
            if (ShadowFire) {
                if (projectile.hostile) {
                    if (ShadowFire) {
                        if (ShadowCount == 0) {
                            ShadowVec = projectile.velocity;//记录初始速度
                            ShadowCount = 3;
                            projectile.velocity *= 0.5f;//减速
                        }
                    }
                    if (!ShadowFire && ShadowCount > 0) ShadowCount--;
                    if (!ShadowFire && ShadowCount == 0) {
                        projectile.velocity = ShadowVec;//重置速度
                    }
                }

                for (int i = 0; i < 4; i++) {
                    int num = Dust.NewDust(projectile.Center, 0, 0, DustID.Shadowflame, 0f, 0f, 10, Color.White, 1f);
                    Main.dust[num].velocity = projectile.velocity / 2;
                    Main.dust[num].noGravity = true;
                }
            }

            //于此重置buff印记
            if (projectile.hostile) ShadowFire = false;//我方弹幕不消除印记

            base.PostAI(projectile);
        }

        //戍卫伤害暴击转超频攻击,目前来说,戍卫伤害一定由弹幕承载
        public override void ModifyHitNPC(Projectile projectile, NPC target, ref NPC.HitModifiers modifiers) {
            if (projectile.DamageType == GuardianDamageClass.Instance) {
                modifiers.DisableCrit();//对指定伤害类型取消暴击
            }
            //暗影炎印记
            if (ShadowFire) target.AddBuff(BuffID.ShadowFlame, 180);
            //秘银弩箭三倍伤害
            if(projectile.type == ModContent.ProjectileType<MythrilProj>() && projectile.ai[2] == 0)
            if (target.boss && target.life <= target.lifeMax * 0.5f) {
                modifiers.FinalDamage *= 3;

                    CombatText.NewText(target.Hitbox,
                                    Color.White,
                                    "×3",
                                    true,
                                    false
                                    );
                }
        }

        public override void ModifyHitPlayer(Projectile projectile, Player target, ref Player.HurtModifiers modifiers) {
            //暗影炎印记
            if (ShadowFire) target.AddBuff(BuffID.ShadowFlame, 60);
        }
    }
}
