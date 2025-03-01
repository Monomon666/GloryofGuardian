using GloryofGuardian.Common;
using GloryofGuardian.Content.Buffs;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class NanoGMissileDT : GOGDT
    {
        //不使用贴图,重写绘制
        public override string Texture => GOGConstant.nulls;
        public override void SetStaticDefaults() {
        }

        public sealed override void SetDefaults() {
            Projectile.width = 118;
            Projectile.height = 80;
            Projectile.tileCollide = true;

            Projectile.friendly = true;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.penetrate = -1;
            Projectile.scale *= 1f;
            Projectile.timeLeft = 36000;

            Projectile.scale *= 1f;
        }

        Player Owner => Main.player[Projectile.owner];

        //防止破坏地图道具
        public override bool? CanCutTiles() {
            return false;
        }

        //生成时自由下坠
        public override void OnSpawn(IEntitySource source) {
            count0 = 60;//默认发射间隔
            interval = 5;//多重攻击间隔长度
            Projectile.velocity = new Vector2(0, 8);
            base.OnSpawn(source);
        }

        int count = 0;
        int count0 = 0;
        int interval = 0;
        //重力
        bool drop = true;
        //数据读取
        int Gcount = 0;
        int lastdamage = 0;
        //重置判定
        bool firstatk = false;
        //状态机
        int mode = 0;
        int modecount = 0;
        int anicount = 0;
        int addcount = 0;//延迟
        List<int> ignore = new List<int>();
        float mindistance = 0;
        NPC tarnpc = null;
        NPC target0 = null;
        bool hadatk = false;
        bool closedoor = false;
        public override void AI() {
            count++;
            Projectile.timeLeft = 2;
            Lighting.AddLight(Projectile.Center, 7 * 0.01f, 255 * 0.01f, 255 * 0.01f);//战斗状态发亮
            Projectile.StickToTiles(false, false);//形成判定
            Drop();
            Calculate();

            //充能
            if (mode == 0) {//冷却阶段
                modecount++;
                if (modecount > Gcount) {
                    mode = 1;
                    modecount = 0;
                }
            }
            if (mode == 1) {//准备完成/索敌/展开
                anicount++;
                if (anicount > 60) {
                    mode = 2;
                    anicount = 60;
                }
            }
            if (mode == 2) {//攻击发动/关闭
                if (closedoor) anicount--;
                //索敌
                if (!hadatk) {
                    mindistance = 3600;
                    ignore.Clear();
                    for (int i = 0; i < Main.maxNPCs; i++) {
                        if (target0 == null) {
                            NPC target1 = Projectile.Center.InPosClosestNPC(3600, true, true, ignore);
                            //标记检索
                            if (target1 != null && !target1.HasBuff<NanoMarkDebuff1>() && !target1.HasBuff<NanoMarkDebuff2>()) {
                                ignore.Add(target1.whoAmI);
                                continue;
                            }
                            if (target1 != null && (target1.HasBuff<NanoMarkDebuff1>() || target1.HasBuff<NanoMarkDebuff2>())) {
                                if (Vector2.Distance(Projectile.Center, target1.Center) < mindistance) {
                                    target0 = target1;
                                    mindistance = Vector2.Distance(Projectile.Center, target1.Center);
                                }
                            }
                        }
                    }
                    if (target0 == null || !target0.active) addcount = 0;
                    if (target0 != null) {
                        tarnpc = target0;
                        //锁定
                        if (tarnpc.HasBuff<NanoMarkDebuff1>()) tarnpc.AddBuff(ModContent.BuffType<NanoMarkDebuff2>(), 300);
                        if (tarnpc.HasBuff<NanoMarkDebuff2>()) {
                            hadatk = true;
                            Attack(target0);
                            target0 = null;
                            addcount = 0;
                        }
                    }
                }
                //延迟关门
                if (hadatk) {
                    addcount++;
                }
                if (addcount > 60) {
                    closedoor = true;
                }

                if (anicount < 0) {
                    mode = 0;
                    hadatk = false;
                    anicount = 0;
                    addcount = 0;
                    closedoor = false;
                    hadfire = false;
                }
            }

            base.AI();
        }

        /// <summary>
        /// 坠落
        /// </summary>
        void Drop() {
            Projectile.velocity.Y += 8f;
            if (Projectile.velocity.Y > 8f) {
                Projectile.velocity.Y = 8f;
            }

            Vector2 droppos = Projectile.Bottom;
            if (drop) {
                int maxdropdis = 5000;
                for (int y = 0; y < maxdropdis; y++) {
                    Tile tile0 = TileHelper.GetTile(GOGUtils.WEPosToTilePos(droppos + new Vector2(0, y) * 16));
                    if (tile0.HasTile) {
                        Projectile.Bottom = (droppos + new Vector2(0, y - 6) * 16);
                        break;
                    }
                }
                drop = false;
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

        bool hadfire = false;
        /// <summary>
        /// 监测与攻击
        /// </summary>
        void Attack(NPC target1) {
            Vector2 tarpos = target1.Center + new Vector2(0, target1.height / 2);
            Vector2 projcen = Projectile.Center + new Vector2(0, 16);

            //发射
            //普通
            if (Main.rand.Next(100) >= Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) {
                for (int i = 0; i < 1; i++) {
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Item11);
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen, new Vector2(0, -2), ModContent.ProjectileType<NanoGMissileProj>(), lastdamage, 0, Owner.whoAmI, target1.whoAmI);
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
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Item11);
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen, new Vector2(0, -2), ModContent.ProjectileType<NanoGMissileProj>(), lastdamage, 0, Owner.whoAmI, target1.whoAmI);
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
            modecount = Owner.GetModPlayer<GOGModPlayer>().GcountEx;
            hadfire = true;
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
            fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            //Projectile.velocity *= 0;
            return false;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            return false;
        }

        public override void OnKill(int timeLeft) {
            //爆炸粒子
            for (int j = 0; j < 15; j++) {
                int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, 0f, 0f, 10, Color.White, 0.8f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].velocity *= 2f;
            }
            for (int j = 0; j < 15; j++) {
                int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric, 0f, 0f, 10, Color.White, 0.4f);
                Main.dust[num2].noGravity = true;
                Main.dust[num2].velocity *= 1f;
            }
        }

        int doorcount = 0;
        public override bool PreDraw(ref Color lightColor) {
            doorcount++;
            float breath01 = (float)Math.Sin(doorcount * 0.05f) + 1;
            float breath02 = (float)Math.Sin(doorcount * 0.2f) + 1;
            float breath03 = (float)Math.Sin(doorcount * 0.02f) + 1.5f;

            float breathm = (float)Math.Cos(doorcount * 0.2f) + 1;

            float breath1 = 0;
            float breath2 = 0;
            if (mode == 0) {
                breath1 = breath01;
                breath2 = breath01;
            }
            if (mode == 1) {
                breath1 = breath02;
                breath2 = breath01;
            }
            if (mode == 2) {
                if (closedoor) {
                    breath1 = 1;
                    breath2 = breath01;
                } else {
                    breath1 = breath02;
                    breath2 = breath03;
                }
            }

            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "NanoGMissileDT").Value;
            Texture2D texture0g = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "NanoGMissileDTGlow").Value;
            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "NanoGMissileDT2").Value;
            Texture2D texture1g = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "NanoGMissileDT2Glow").Value;
            Texture2D texture2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "NanoGMissileDT3").Value;
            Texture2D texture2g = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "NanoGMissileDT3Glow").Value;

            Texture2D texturem = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "NanoGMissileProj2").Value;
            Texture2D texturemg = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "NanoGMissileProj2Glow").Value;

            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, 0);
            //门位移
            Vector2 doorvec = new Vector2(-16, 0);
            doorvec *= (anicount / 120f);
            //导弹位移
            Vector2 missilepos = new Vector2(0, -27);
            missilepos *= (anicount / 120f);

            //背景绘制
            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            //背景光效
            Main.EntitySpriteDraw(texture0g, drawPosition0, null, lightColor * breath1, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            //伪导弹绘制
            if (!hadfire) {
                //导弹绘制
                Main.EntitySpriteDraw(texturem, drawPosition0 + new Vector2(0, 16), null, lightColor, Projectile.rotation, texturem.Size() * 0.5f, Projectile.scale * 1f, SpriteEffects.None, 0);
                //导弹光效
                Main.EntitySpriteDraw(texturemg, drawPosition0 + new Vector2(0, 16), null, lightColor * breathm, Projectile.rotation, texturemg.Size() * 0.5f, Projectile.scale * 1f, SpriteEffects.None, 0);
            }

            //左门绘制
            Main.EntitySpriteDraw(texture1, drawPosition0 + doorvec, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            //左门绘制
            Main.EntitySpriteDraw(texture1g, drawPosition0 + doorvec, null, lightColor * breath2, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            //右门绘制
            Main.EntitySpriteDraw(texture2, drawPosition0 - doorvec, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            //右门绘制
            Main.EntitySpriteDraw(texture2g, drawPosition0 - doorvec, null, lightColor * breath2, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}