using System;
using System.Collections.Generic;
using System.Diagnostics;
using GloryofGuardian.Common;
using GloryofGuardian.Content.Buffs;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Localization;

namespace GloryofGuardian.Content.Projectiles {
    public class Corrupt2DT : GOGDT {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 64;
            Projectile.friendly = false;
            Projectile.penetrate = -1;

            OtherHeight = 34;

            count0 = 60;

            exdust = DustID.CorruptGibs;

            ignoretile = true;
            Attackrange = 1900;
        }

        Player Owner => Main.player[Projectile.owner];

        bool rained = false;
        float dark = 0;
        float darkmax = 0;
        public override void AI() {
            AttackPos = Projectile.Center + new Vector2(-4, -36);

            //13为绿色
            //if (drawcount % 400 < 100 || drawcount % 200 > 300) "13".WL();
            //else "14".WL();

            if (drawcount == 1) {
                if (Main.raining || Main.cloudAlpha > 0) rained = true;

                if (rained) {
                    //如果本来有雨,则重新设定目标雨量
                    dark = Main.cloudAlpha;
                    darkmax = Math.Max(0.5f, Main.cloudAlpha);
                }
                else {
                    //如果本来没有雨,则使用0.5f
                    darkmax = 0.5f;
                    Main.rainTime = 180.0;
                    Main.raining = true;

                    if (Main.netMode == NetmodeID.Server) {
                        NetMessage.SendData(MessageID.WorldData, -1, -1, (NetworkText)null, 0, 0f, 0f, 0f, 0, 0, 0);
                        Main.SyncRain();
                    }
                }
            }

            if (drawcount > 1) {
                Main.rainTime = 180.0;
                Main.raining = true;
                if (dark < darkmax) {
                    dark = Math.Max(dark, Math.Min(0.5f, (drawcount / 360f)));
                    Main.maxRaining = Main.cloudAlpha = dark;
                }
            }

            //常态攻击
            if (drawcount % 6 == 0) {
                for (int i = 0; i < 1 + Main.rand.Next(4); i++) {
                    Vector2 firepos = AttackPos + new Vector2(1900 * Main.rand.NextFloat(-1, 1), -900 * Main.rand.NextFloat(0.9f, 1.1f));
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), firepos,
                        new Vector2(0, 18 + Main.rand.NextFloat(6)).RotatedBy(-Main.windSpeedCurrent),
                        ModContent.ProjectileType<Corrupt2RainProj>(), lastdamage / 3, 6, Owner.whoAmI,
                        ((drawcount % 400 < 200) && Main.rand.Next(300) < Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1] + othercrit) ? 2 : 0
                        );
                }
            }

            //近战
            for (int i = 0; i < Main.maxNPCs; i++) {
                NPC tarnext = Main.npc[i];
                if (tarnext != null && tarnext.active) {
                    if (Vector2.Distance(Projectile.Center, tarnext.Center) < 180) {
                        tarnext.AddBuff(BuffID.CursedInferno, 60);
                        if(tarnext.knockBackResist > 0) tarnext.velocity *= 0.8f;

                        if (drawcount % 1 == 0) {
                            for (int j = 0; j < 1; j++) {
                                int num = Dust.NewDust(AttackPos, 0, 0, DustID.CursedTorch, 0f, 0f, 50, Color.White, 2f);
                                Main.dust[num].velocity = new Vector2(0, -4f).RotatedBy(Main.rand.NextFloat(-1f, 1f));
                                Main.dust[num].velocity *= Main.rand.NextFloat(0.8f, 1.2f);
                                Main.dust[num].noGravity = true;
                                if (Main.rand.NextBool(4)) {
                                    Main.dust[num].noGravity = false;
                                    Main.dust[num].velocity *= Main.rand.NextFloat(0.2f, 0.6f);
                                }
                            }
                        }
                    }
                }
            }

            //todo 粒子动画
            base.AI();
        }

        //穿墙多索敌,自锁,一个按远近顺序排序的list,一个最终攻击的list

        List<NPC> tarlist = new List<NPC>();

        List<NPC> _nearbyNPCs = new List<NPC>();
        List<float> _npcDistances = new List<float>();
        List<NPC> _finalTargets = new List<NPC>();
        protected override List<Projectile> Attack1() {
            return Attack3(1);
        }

        protected override List<Projectile> Attack2() {
            if (drawcount % 400 < 100 || drawcount % 400 > 300) return Attack3(1);
            else return Attack3(2);
        }

        List<Projectile> Attack3(int m) {
            List<Projectile> projlist = new List<Projectile>();

            tarlist.Clear();
            for (int i = 0; i < Main.maxNPCs; i++) {
                NPC tar = Main.npc[i];
                if (tarlist.Count >=
                    (m == 1 ? 3 : 5)
                    ) break;
                if (tar != null && tar.active) {
                    if (Vector2.Distance(Projectile.Center, tar.Center) < 1900) {
                        tarlist.Add(tar);
                    }
                }
            }

            for (int i = 0; i < tarlist.Count; i++) {
                NPC nowtar = tarlist[Main.rand.Next(0, Math.Min((m == 1 ? 3 : 5), tarlist.Count))];
                Vector2 firepos = nowtar.Center + new Vector2(0, -1900 * Main.rand.NextFloat(0.9f, 1)).RotatedBy(-Main.windSpeedCurrent);
                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), firepos,
                    new Vector2(0, 24 + Main.rand.NextFloat(6)).RotatedBy(-Main.windSpeedCurrent),
                    ModContent.ProjectileType<Corrupt2RainProj>(), lastdamage, 6, Owner.whoAmI, 
                    (m == 1 ? 1 : 2));

                projlist.Add(proj1);
            }

            FinishAttack = true;
            return projlist;
        }

        public override void OnKill(int timeLeft) {
            if (rained) {
                Main.StopRain();
                if (Main.netMode == NetmodeID.Server) {
                    Main.SyncRain();
                }
            }
            
            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "Corrupt2DT").Value;

            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "Rhombus").Value;
            Texture2D texture2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "RhombusBlackShadow").Value;
            Texture2D texture3 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "RhombusShadow1").Value;
            Texture2D texture4 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "RhombusShadow2").Value;

            if (1 == 1) {
                Vector2 drawPosition = AttackPos - Main.screenPosition + new Vector2(4, -0);
                for (int i = 0; i < 8; i++) {
                    Main.EntitySpriteDraw(texture2,
                        drawPosition + new Vector2(0, 16) + new Vector2(0, (float)(-174 * Math.Min(1, (drawcount / 180f)) + Math.Sin(drawcount / 20f) * 8f)).RotatedBy(MathHelper.Pi / 4f * i + drawcount / 60f),
                        null,
                        WeightedRoundTripTransition(new Color(140, 220, 40, 180), new Color(109, 53, 152, 180), drawcount, 200, 0.5f, 0.5f)
                        * Math.Min(1, (drawcount / 180f)),
                        Projectile.rotation + (MathHelper.Pi / 4f * i + drawcount / 60f),
                        texture2.Size() * 0.5f, Projectile.scale * new Vector2(1.5f, 1f), SpriteEffects.None, 0);
                    for (int j = 0; j < 1; j++) {
                        Main.EntitySpriteDraw(texture1,
                        drawPosition + new Vector2(0, 16) + new Vector2(0, (float)(-174 * Math.Min(1, (drawcount / 180f)) + Math.Sin(drawcount / 20f) * 8f * Math.Min(1, (drawcount / 180f)))).RotatedBy(MathHelper.Pi / 4f * i + drawcount / 60f),
                        null,
                        //new Color(0, 180, 0, 0) * Math.Min(1, (drawcount / 180f)),
                        WeightedRoundTripTransition(new Color(140, 220, 40, 0), new Color(109, 53, 152, 0), drawcount, 400, 0.5f, 0.5f)
                        * Math.Min(1, (drawcount / 180f)),
                        Projectile.rotation + (MathHelper.Pi / 4f * i + drawcount / 60f),
                        texture1.Size() * 0.5f, Projectile.scale * new Vector2(1.5f, 1), SpriteEffects.None, 0);
                    }
                }
            }

            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, -24);
            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }

        /// <summary>
        /// 可调时间权重的1-2-1平滑颜色循环
        /// </summary>
        /// <param name="color1">第一种颜色</param>
        /// <param name="color2">第二种颜色</param>
        /// <param name="timer">当前计时器值</param>
        /// <param name="duration">完整循环周期</param>
        /// <param name="toColor2Weight">到color2的过渡权重</param>
        /// <param name="holdColor2Weight">保持color2的权重</param>
        /// <returns>过渡后的颜色</returns>
        public static Color WeightedRoundTripTransition(
            Color color1,
            Color color2,
            int timer,
            int duration,
            float toColor2Weight,
            float holdColor2Weight) {
            if (duration <= 0) return color2;

            // 计算归一化权重
            float totalWeight = toColor2Weight + holdColor2Weight + toColor2Weight; // 去+保持+回
            float normToWeight = toColor2Weight / totalWeight;
            float normHoldWeight = holdColor2Weight / totalWeight;
            float normBackWeight = toColor2Weight / totalWeight;

            // 当前循环进度(0-1)
            float cyclePos = (float)(timer % duration) / duration;

            // 计算混合因子
            float blendFactor;

            if (cyclePos < normToWeight) {
                // 第一阶段：color1 → color2
                float phasePos = cyclePos / normToWeight;
                blendFactor = SmoothPhase(phasePos);
            }
            else if (cyclePos < normToWeight + normHoldWeight) {
                // 第二阶段：保持color2
                blendFactor = 1f;
            }
            else {
                // 第三阶段：color2 → color1
                float phasePos = (cyclePos - normToWeight - normHoldWeight) / normBackWeight;
                blendFactor = 1f - SmoothPhase(phasePos);
            }

            return Color.Lerp(color1, color2, blendFactor);
        }

        /// <summary>
        /// 平滑过渡阶段计算(0-1)
        /// </summary>
        private static float SmoothPhase(float t) {
            // 使用三次缓动函数实现超平滑过渡
            t = MathHelper.Clamp(t, 0f, 1f);
            return t * t * (3f - 2f * t);

            // 可选：使用正弦函数实现更柔和的过渡
            // return (float)Math.Sin(t * MathHelper.PiOver2);
        }
    }

    public class Corrupt2RainProj : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.localNPCHitCooldown = 0;
            Projectile.penetrate = 1;
        }

        Player Owner => Main.player[Projectile.owner];

        public override void AI() {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;

            if (Projectile.ai[0] == 2) {
                if (count % 1 == 0) {
                    for (int i = 0; i < 1; i++) {
                        int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GreenFairy, 0f, 0f, 50, Color.White, 1f);
                        Main.dust[num].velocity = Projectile.velocity;
                        Main.dust[num].velocity *= 1f;
                        Main.dust[num].noGravity = true;
                    }
                }
            }

            //腐化伏行
            if (Projectile.ai[0] == 3 || Projectile.ai[0] == 3.5f) {
                Projectile.width = Projectile.height = 8;

                Drop();

                if (count % 1 == 0) {
                    for (int i = 0; i < 4; i++) {
                        int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GreenFairy, 0f, 0f, 50, Color.White, 1f);
                        Main.dust[num].velocity *= 0f;
                        Main.dust[num].noGravity = true;
                    }
                }

                if (Projectile.ai[0] == 3 && count % 5 == 0) {
                    int num = count / 30;
                    int damage1 = (int)(Projectile.damage * ((13 - num) / 12f));

                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center + new Vector2(0, 16), new Vector2(0, -4), ModContent.ProjectileType<Corrupt2RainProj>(), damage1, 6, Owner.whoAmI, 4, num);
                    proj1.penetrate = -1;
                    proj1.localNPCHitCooldown = 30;
                }

                if (count > 40) Projectile.Kill();
            }

            //腐化柱子
            if (Projectile.ai[0] == 4) {
                Projectile.width = Projectile.height = 16;

                height0 = (int)(80 - 40 * Projectile.ai[1]);
                Projectile.velocity *= 0;

                float dying = count / 20f;

                if (1 == 1) {
                    if (count % 1 == 0) {
                        for (int i = 0; i < 4; i++) {
                            int num = Dust.NewDust(Projectile.position + new Vector2(0, Projectile.height - height0 * dying - 16), 16, 32, DustID.GreenFairy, 0f, 0f, 50, Color.Yellow, 1f);
                            Main.dust[num].velocity = new Vector2(0, -4);
                            Main.dust[num].noGravity = true;
                        }
                    }

                    if (count % 1 == 0) {
                        for (int i = 0; i < 1; i++) {
                            int num = Dust.NewDust(Projectile.position + new Vector2(0, Projectile.height - height0 * dying - 16), 16, 32, DustID.CursedTorch, 0f, 0f, 50, Color.White, 2f);
                            Main.dust[num].velocity = new Vector2(0, -2);
                            Main.dust[num].noGravity = true;
                        }
                    }
                }

                if (count > 20) Projectile.Kill();
            }

            base.AI();
        }

        bool drop = true;
        int height0 = 0;
        /// <summary>
        /// 坠落
        /// </summary>
        void Drop() {
            Projectile.velocity.Y += 0.2f;
            if (Projectile.velocity.Y > 8f) {
                Projectile.velocity.Y = 8f;
            }

            Vector2 droppos = Projectile.Bottom;
            if (drop) {
                int maxdropdis = 5000;
                for (int y = 0; y < maxdropdis; y++) {
                    Tile tile0 = TileHelper.GetTile(GOGUtils.WEPosToTilePos(droppos + new Vector2(0, y) * 16));
                    if (tile0.HasTile) {
                        Projectile.Bottom = (droppos + new Vector2(0, y - 1) * 16);
                        break;
                    }
                }
            }
        }

        bool boom = false;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            target.AddBuff(ModContent.BuffType<CorruptSeedbedDebuff>(), 180);
            if (Projectile.ai[0] == 2) target.AddBuff(BuffID.CursedInferno, 180);

            if (Projectile.ai[0] == 2 &&
                TileHelper.FindTilesInRectangle(target.Center + new Vector2(target.width / 2, target.height), 3, 3, 1, 2, true) != null) {

                for (int i = 0; i < 1; i++) {
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.NPCHit9, Projectile.Center);
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), target.Center + new Vector2(target.width / 2, target.height), new Vector2(4, 0), ModContent.ProjectileType<Corrupt2RainProj>(), Projectile.damage / 2, 4, Owner.whoAmI, 3);
                    Projectile proj3 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), target.Center + new Vector2(target.width / 2, target.height), new Vector2(-4, 0), ModContent.ProjectileType<Corrupt2RainProj>(), Projectile.damage / 2, 4, Owner.whoAmI, 3);
                    proj1.tileCollide = proj3.tileCollide = false;
                    proj1.localNPCHitCooldown = proj3.localNPCHitCooldown = -1;
                }
            }

            if (Projectile.ai[0] == 2 &&
                TileHelper.FindTilesInRectangle(target.Center + new Vector2(target.width / 2, target.height), 3, 3, 1, 2, true) == null) {
                boom = true;
            }

            if (Projectile.ai[0] == 3) {
                for (int i = 0; i < 25; i++) {
                    int num = Dust.NewDust(target.position, target.width, target.height, DustID.GreenFairy, 0f, 0f, 50, Color.Yellow, 1.5f);
                    Main.dust[num].velocity = new Vector2(Main.dust[num].position.Toz(target.Center).X, -4);
                    Main.dust[num].noGravity = true;
                }
            }

            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            if (Projectile.ai[0] == 2) {
                for (int i = 0; i < 1; i++) {
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.NPCHit9, Projectile.Center);
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center, new Vector2(2, 0), ModContent.ProjectileType<Corrupt2RainProj>(), Projectile.damage / 2, 4, Owner.whoAmI, 3.5f);
                    Projectile proj3 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center, new Vector2(-2, 0), ModContent.ProjectileType<Corrupt2RainProj>(), Projectile.damage / 2, 4, Owner.whoAmI, 3.5f);
                    proj1.tileCollide = proj3.tileCollide = false;
                    proj1.localNPCHitCooldown = proj3.localNPCHitCooldown = -1;
                }
            }

            return base.OnTileCollide(oldVelocity);
        }

        public override void OnKill(int timeLeft) {
            if (boom) {
                //爆炸
                for (int j = 0; j < 60; j++) {
                    int num1 = Dust.NewDust(Projectile.Center, 0, 0, DustID.GreenFairy, 0f, 0f, 10, Color.Yellow, 1.7f);
                    Main.dust[num1].velocity = new Vector2(0, -1 * Main.rand.NextFloat(0.8f, 1.2f)).RotatedBy(MathHelper.Pi / 30f * j);
                    Main.dust[num1].noGravity = true;
                    Main.dust[num1].velocity *= 8f;
                }

                SoundEngine.PlaySound(in SoundID.NPCHit9, Projectile.Center);
                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = (int)(120 * Projectile.scale);
                Projectile.position.X = Projectile.position.X - Projectile.width / 2;
                Projectile.position.Y = Projectile.position.Y - Projectile.height / 2;
                Projectile.maxPenetrate = -1;
                Projectile.penetrate = -1;
                Projectile.usesLocalNPCImmunity = false;
                Projectile.usesIDStaticNPCImmunity = true;
                Projectile.idStaticNPCHitCooldown = 0;
                Projectile.Damage();
            }
            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "Corrupt2RainProj0").Value;
            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "Corrupt2RainProj").Value;
            Texture2D texture2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "Corrupt2RainProj2").Value;

            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "RainShadow").Value;

            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition - Projectile.velocity.SafeNormalize(Vector2.Zero) * texture1.Width / 2;

            if (Projectile.ai[0] == 0 || Projectile.ai[0] == 1) {
                if (Main.dayTime) {
                    Main.EntitySpriteDraw(texture, drawPosition0, null, new Color(0, 0, 0), Projectile.rotation,
                    texture.Size() * 0.5f,
                    Projectile.scale, SpriteEffects.None, 0);
                }

                Main.EntitySpriteDraw(texture1, drawPosition0, null, new Color(70, 0, 161, 0) * 2, Projectile.rotation,
                    texture1.Size() * 0.5f,
                    Projectile.scale, SpriteEffects.None, 0);
            }

            if (Projectile.ai[0] == 2) {
                if (Main.dayTime) {
                    Main.EntitySpriteDraw(texture, drawPosition0, null, new Color(0, 0, 0), Projectile.rotation,
                    texture.Size() * 0.5f,
                    Projectile.scale, SpriteEffects.None, 0);
                }

                Main.EntitySpriteDraw(texture1, drawPosition0, null, new Color(3, 239, 2, 0) * 20, Projectile.rotation,
                    texture1.Size() * 0.5f,
                    Projectile.scale, SpriteEffects.None, 0);
            }

            if (Projectile.ai[0] == 3 || Projectile.ai[0] == 4) {
                Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation,
                    texture0.Size() * 0.5f,
                    Projectile.scale * 0.5f, SpriteEffects.None, 0);
            }

            return base.PreDraw(ref lightColor);
        }
    }

    public class CorruptCloudProj : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Main.projFrames[Projectile.type] = 6;//动图帧数

            Projectile.width = 54;
            Projectile.height = 24;
            Projectile.localNPCHitCooldown = 20;
            Projectile.penetrate = 1;

            Projectile.friendly = false;
            Projectile.tileCollide = false;

            if (Projectile.ai[1] == 1) Projectile.Size *= 1.5f;
        }

        Player Owner => Main.player[Projectile.owner];

        NPC target1 = null;
        bool startspeed = true;
        Vector2 chasingpos = Vector2.Zero;
        public override void AI() {
            //目标继承
            if (count == 1) target1 = Main.npc[(int)Projectile.ai[0]];

            //初始的加速
            if (count > 60) startspeed = false;

            //默认的摇晃
            chasingpos += new Vector2((float)Math.Cos(count / 40f), 0);
            if (Projectile.ai[1] == 1) chasingpos += new Vector2((float)Math.Cos(count / 40f), 0);

            if (target1 != null && target1.active && target1.whoAmI != 0) {
                if (count == 1) chasingpos = target1.Center + new Vector2(0, -240) + new Vector2(0, Main.rand.Next(-8, 8));

                if (startspeed) Projectile.ChasingBehavior(chasingpos, 12, 48);
                if (!startspeed) Projectile.ChasingBehavior(chasingpos, 4, 48);
            }
            else {
                target1 = Projectile.Center.InPosClosestNPC(400, 0, false, true);
            }

            if (Projectile.ai[1] == 0) {
                if (count >= 90) Projectile.alpha += 1;
                if (Projectile.alpha >= 240) Projectile.Kill();
            }
            if (Projectile.ai[1] == 1) {
                if (count >= 150) Projectile.alpha += 1;
                if (Projectile.alpha >= 240) Projectile.Kill();
            }

            //发射雨滴
            if (Projectile.ai[1] == 0) {
                if (count > 60 && count % 20 == 0) {
                    int randx = Main.rand.Next(-6, 6);
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center + new Vector2(randx, 6), new Vector2(0, 12), ModContent.ProjectileType<CorruptRainProj>(), Projectile.damage, 1, Owner.whoAmI);
                    count += Main.rand.Next(10);
                }
            }
            if (Projectile.ai[1] == 1) {
                if (count > 60 && count % 12 == 0) {
                    int randx = Main.rand.Next(-6, 6);
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center + new Vector2(randx, 6), new Vector2(0, 12), ModContent.ProjectileType<CorruptRainProj>(), Projectile.damage, 1, Owner.whoAmI, 1);
                    count += Main.rand.Next(10);
                }
            }


            //帧图
            Projectile.frameCounter++;
            Projectile.frame = (Projectile.frameCounter / 6)
                % 6;//要手动填，不然会出错

            base.AI();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {

        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            return base.OnTileCollide(oldVelocity);
        }

        public override void OnKill(int timeLeft) {

        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "CorruptCloudProj").Value;

            int singleFrameY = texture.Height / Main.projFrames[Type];
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0, -8);
            //Main.EntitySpriteDraw(texture, drawPos, new Rectangle(0, singleFrameY * Projectile.frame, texture.Width, singleFrameY), lightColor * ((255f - Projectile.alpha) / 255f), Projectile.rotation, new Vector2(27, 23), Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(
                texture,
                drawPos,
                new Rectangle(0, singleFrameY * Projectile.frame, texture.Width, singleFrameY),//动图读帧
                lightColor * ((255f - Projectile.alpha) / 255f),
                Projectile.rotation,
                new Vector2(27, 23),
                Projectile.scale,
                SpriteEffects.None,
                0);
            return false;
        }
    }
}
