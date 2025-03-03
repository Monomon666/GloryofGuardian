using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class FerryLightDT : GOGDT
    {
        public override string Texture => GOGConstant.Projectiles + Name;

        public override void SetStaticDefaults() {
            //残影机制
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults() {
            //这里的尺寸对应的是碰撞体积
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.timeLeft = 36000;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 2;//穿透数，1为攻击到第一个敌人就消失

            Projectile.alpha = 0;

            Projectile.scale *= 1f;
        }

        Player Owner => Main.player[Projectile.owner];

        Vector2 OwnerPos => Owner.Center;

        Vector2 ToMou => Main.MouseWorld - OwnerPos;

        public override void OnSpawn(IEntitySource source) {
            count0 = 12;//默认发射间隔
            Projectile.frame += Main.rand.Next(6);
        }

        // 存储符合条件的 NPC 及其计时器
        private Dictionary<int, int> trackedNPCs = new Dictionary<int, int>();

        int sincount = 0;
        int count = 0;
        int count0 = 0;
        //数据读取
        int Gcount = 0;
        int lastdamage = 0;
        public override void AI() {
            count++;
            sincount++;
            Projectile.Center += new Vector2(0, (float)Math.Sin(sincount / 30f) * 0.5f);
            Calculate();

            //范围粒子
            for (int i = 0; i <= 32; i++) {
                Dust dust1 = Dust.NewDustDirect(Projectile.Center + Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * 600, 8, 8, DustID.SnowSpray, 1f, 1f, 100, Color.Purple, 0.8f);
                dust1.velocity *= 0;
                dust1.noGravity = true;
            }
            for (int i = 0; i <= 32; i++) {
                Dust dust1 = Dust.NewDustDirect(Projectile.Center + Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * 600, 8, 8, DustID.SnowSpray, 1f, 1f, 100, Color.Purple, 0.8f);
                dust1.velocity = dust1.position.Toz(Projectile.Center) * 2f;
                dust1.noGravity = true;
            }

            // 检测范围内的 NPC
            DetectNPCs();

            // 更新已跟踪 NPC 的状态
            UpdateTrackedNPCs();
        }

        /// <summary>
        /// 发动攻击
        /// </summary>
        void Attack(Vector2 position, int damage) {
            //普通
            if (Main.rand.Next(100) >= Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) {
                for (int i = 0; i < 1; i++) {
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), position, new Vector2(0, 0), ModContent.ProjectileType<FerryLightProj>(), damage, 1, Owner.whoAmI);
                    if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                        if (proj1.ModProjectile is GOGProj proj2) {
                            proj2.OrichalcumMarkProj = true;
                            proj2.OrichalcumMarkProjcount = 300;
                        }
                    }
                }
            }

            //过载
            if (Main.rand.Next(100) < Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) {
                for (int i = 0; i < 1; i++) {
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), position, new Vector2(0, 0), ModContent.ProjectileType<FerryLightProj>(), damage, 1, Owner.whoAmI, 1);
                    if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                        if (proj1.ModProjectile is GOGProj proj2) {
                            proj2.OrichalcumMarkProj = true;
                            proj2.OrichalcumMarkProjcount = 300;
                        }
                    }
                }
            }

            //计时重置,通过更改这个值来重置攻击
            count = Owner.GetModPlayer<GOGModPlayer>().GcountEx;
        }

        // 检测范围内的 NPC
        private void DetectNPCs() {
            float detectionRadius = 600f; // 检测范围

            for (int i = 0; i < Main.maxNPCs; i++) {
                NPC npc = Main.npc[i];

                // 检查 NPC 是否存活且血量低于 50%
                if (npc.active && !npc.friendly && !npc.boss && npc.life < npc.lifeMax * 0.5f && npc.life < 2000) {
                    // 检查 NPC 是否在检测范围内
                    if (Vector2.Distance(Projectile.Center, npc.Center) <= detectionRadius) {
                        // 如果 NPC 未被跟踪，则添加到跟踪列表
                        if (!trackedNPCs.ContainsKey(i)) {
                            trackedNPCs[i] = 0; // 初始化计时器
                        }
                    }
                }
            }
        }

        // 更新已跟踪 NPC 的状态
        private void UpdateTrackedNPCs() {
            List<int> toRemove = new List<int>();

            foreach (var pair in trackedNPCs) {
                int npcIndex = pair.Key;
                int timer = pair.Value;

                NPC npc = Main.npc[npcIndex];

                // 检查 NPC 是否脱离范围或血量恢复
                if (!npc.active || npc.friendly || npc.life >= npc.lifeMax * 0.5f || Vector2.Distance(Projectile.Center, npc.Center) > 600f) {
                    toRemove.Add(npcIndex); // 标记为需要移除
                } else {
                    // 更新计时器
                    trackedNPCs[npcIndex]++;
                    // 减速
                    npc.velocity *= 0.95f;

                    for (int i = 0; i <= 4; i++) {
                        Dust dust1 = Dust.NewDustDirect(npc.Center + new Vector2(-2, -4) + Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * Main.rand.NextFloat(0, 8), 0, 0, DustID.PurpleCrystalShard, 1f, 1f, 100, Color.Purple, 1f);
                        dust1.velocity = dust1.position.Toz(Projectile.Center) * 4f;
                        dust1.noGravity = true;
                    }

                    // 如果计时器达到 4 秒（240 帧），召唤弹幕二
                    if (trackedNPCs[npcIndex] >= 240) {
                        int life0 = npc.life;
                        npc.life = 1;
                        Attack(npc.Center, life0);
                        toRemove.Add(npcIndex); // 标记为需要移除
                    }
                }
            }

            // 移除不符合条件的 NPC
            foreach (int npcIndex in toRemove) {
                trackedNPCs.Remove(npcIndex);
            }
        }

        /// <summary>
        /// 重新计算和赋值参数
        /// </summary>
        void Calculate() {
            Gcount = (int)(count0 * Owner.GetModPlayer<GOGModPlayer>().GcountR * Projectile.ai[0]);//攻击间隔因子重新提取
            //伤害修正
            int newDamage = Projectile.originalDamage;
            float rangedOffset = Owner.GetTotalDamage(GuardianDamageClass.Instance).ApplyTo(100) / 100f;
            lastdamage = (int)(newDamage * rangedOffset);
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {

        }

        public override void OnKill(int timeLeft) {
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FerryLightDT").Value;
            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "Shadow").Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0, -8);

            Main.spriteBatch.Draw(
                texture,
                drawPos,
                null,
                lightColor * ((255f - Projectile.alpha) / 255f),
                Projectile.rotation,
                texture.Size() * 0.5f,
                Projectile.scale,
                SpriteEffects.None,
                0);

            foreach (var pair in trackedNPCs) {
                int npcIndex = pair.Key;
                NPC npc = Main.npc[npcIndex];

                // 检查 NPC 是否脱离范围或血量恢复
                if (npc != null && npc.active) {
                    Vector2 drawPos2 = npc.Center - Main.screenPosition;

                    Main.spriteBatch.Draw(
                        texture1,
                        drawPos2,
                        null,
                        lightColor * 0.3f,
                        Projectile.rotation,
                        texture1.Size() * 0.5f,
                        Projectile.scale * 0.8f,
                        SpriteEffects.None,
                        0
                    );
                }
            }

            return false;
        }
    }
}
