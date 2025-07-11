using System;
using GloryofGuardian.Common;
using GloryofGuardian.Content.Buffs;
using GloryofGuardian.Content.Class;
using GloryofGuardian.Content.Projectiles;
using Terraria.ID;

namespace GloryofGuardian.Content.Classes {
    public class GOGGlobalNPC : GlobalNPC {
        public override bool InstancePerEntity => true;

        //变量存储

        //钉入buff
        public bool JavelinDebuffEffect1;
        //腐蚀buff
        public bool CorruptDebuffEffect;
        public int CorruptDebuffCount;
        //腐化苗床buff
        public bool CorruptSeedbedDebuff;

        //buff重设
        public override void ResetEffects(NPC npc) {
            JavelinDebuffEffect1 = false;
            CorruptDebuffEffect = false;
            if (!npc.HasBuff(ModContent.BuffType<CorruptDebuff>())) CorruptDebuffCount = 0;
            npc.defense = npc.defDefense;
        }

        public override void SetDefaults(NPC entity) {
            // 使我们的长矛debuff伤害判定和原版的骨矛一样
            entity.buffImmune[ModContent.BuffType<JavelinDebuff1>()] = entity.buffImmune[BuffID.BoneJavelin];
        }

        int updatecount = 0;//buff生效计时器
        int textcount = 0;
        public override void UpdateLifeRegen(NPC npc, ref int damage) {
            //buff总计时器
            updatecount++;
            //跳字用计数器
            textcount++;

            if (JavelinDebuffEffect1) {
                if (npc.lifeRegen > 0) {
                    npc.lifeRegen = 0;//禁用生命恢复
                }
                // 钉入伤害计算
                int JavelinCount = 0;

                //荒野 //哨弩
                for (int i = 0; i < 1000; i++) {
                    Projectile p = Main.projectile[i];
                    if (p.active
                        && p.ai[0] == 1f
                        && p.ai[1] == npc.whoAmI) {
                        if (p.type == ModContent.ProjectileType<WildProj>()) {
                            JavelinCount += 2;
                        }
                        else if (p.type == ModContent.ProjectileType<GarrisonProj>()) {
                            if (p.ai[2] == 0) JavelinCount += 3;
                            if (p.ai[2] == 1) JavelinCount += 6;
                        }
                    }
                }

                // 生命流逝与伤害不相等，生命流逝的单位是2秒
                // 以原版形式显示生命流失
                npc.lifeRegen -= JavelinCount * 2;
                if (JavelinCount == 0) npc.DelBuff(npc.FindBuffIndex(ModContent.BuffType<JavelinDebuff1>()));
            }

            if (CorruptDebuffEffect) {
                if (CorruptDebuffCount == 0) CorruptDebuffCount = 1;
                npc.defense -= CorruptDebuffCount;
                if (textcount % 60 == 0) {
                    CombatText.NewText(npc.Hitbox,
                            new Color(117, 145, 73),
                            -CorruptDebuffCount,
                            true,
                            false
                            );
                }
            }
        }

        public override void ModifyHitByProjectile(NPC npc, Projectile projectile, ref NPC.HitModifiers modifiers) {
            //腐化1减抗
            if (CorruptDebuffEffect) {
                //特殊射弹叠加减抗,普通及特殊射弹都重置buff时长
                if (projectile.type == ModContent.ProjectileType<CorruptRainProj>()) {
                    if (CorruptDebuffCount < 10) {
                        CorruptDebuffCount += 1;
                        CombatText.NewText(npc.Hitbox,
                                new Color(117, 145, 73),
                                -CorruptDebuffCount,
                                true,
                                false
                                );
                    }
                    npc.AddBuff(ModContent.BuffType<CorruptDebuff>(), 300);
                }

                if (projectile.type == ModContent.ProjectileType<CorruptRainProj>()) {
                    npc.AddBuff(ModContent.BuffType<CorruptDebuff>(), 300);
                }
            }

            //腐化2附伤
            if (CorruptSeedbedDebuff) {
                if (projectile.DamageType == GuardianDamageClass.Instance) {
                    int buffdamage = Math.Min(projectile.damage, 10);
                    npc.life -= buffdamage;
                        CombatText.NewText(npc.Hitbox,
                                new Color(3, 239, 2),
                                -buffdamage,
                                true,
                                false
                                );
                }
            }
        }
    }
}
