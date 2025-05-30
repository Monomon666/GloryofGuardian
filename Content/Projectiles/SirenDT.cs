﻿using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class SirenDT : GOGDT
    {
        //不使用贴图,重写绘制
        public override string Texture => GOGConstant.nulls;
        public override void SetStaticDefaults() {
        }

        public sealed override void SetDefaults() {
            Projectile.width = 100;
            Projectile.height = 32;
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
            count0 = 30;//默认发射间隔
            Projectile.velocity = new Vector2(0, 8);
            base.OnSpawn(source);
        }

        int count = 0;
        int count0 = 0;
        //重力
        bool drop = true;
        //数据读取
        int Gcount = 0;
        int lastdamage = 0;
        //合奏
        int allnum = 0;
        int mode = 0;
        public override void AI() {
            count++;
            Projectile.timeLeft = 2;
            Projectile.StickToTiles(false, false);//形成判定
            Drop();
            Calculate();

            for (int i = 0; i <= 8; i++) {
                Dust dust1 = Dust.NewDustDirect(Projectile.Center + Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * 800, 8, 8, DustID.SnowSpray, 1f, 1f, 100, Color.Blue, 0.8f);
                dust1.velocity *= 0;
                dust1.noGravity = true;
            }

            //int dtnum = 0;
            //foreach (var proj in Main.projectile)//遍历所有弹幕
            //{
            //    if (proj.active //活跃状态
            //        && proj.ModProjectile is GOGDT gogdt1//遍历
            //        && proj.type == ModContent.ProjectileType<SirenDT>() //同属附魔贝壳
            //        && Vector2.Distance(Projectile.Center, proj.Center) < 800
            //        ) {
            //
            //        //总数
            //        dtnum++;
            //    }
            //}

            mode = 10;

            //索敌与行动
            NPC target1 = Projectile.Center.InPosClosestNPC(1200, true, true);
            Attack(target1);

            base.AI();
        }

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
                        Projectile.Bottom = (droppos + new Vector2(0, y - 2) * 16);
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

        /// <summary>
        /// 监测与攻击
        /// </summary>
        void Attack(NPC target1) {
            float breath = (float)Math.Sin(drawcount / 18f);
            Vector2 projcen = Projectile.Center + new Vector2(8, -32) + new Vector2(0, -8) * breath;

            //发射,不过载
            if (count >= Gcount) {
                //轴
                if (count == Gcount) {
                    for (int i = 0; i < 4; i++) {
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item26, Projectile.Center);

                        Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen, new Vector2(12, 0).RotatedBy(i * MathHelper.PiOver2), ModContent.ProjectileType<SirenProj>(), lastdamage, 1, Owner.whoAmI, 0);
                        if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                            if (proj1.ModProjectile is GOGProj proj2) {
                                proj2.OrichalcumMarkProj = true;
                                proj2.OrichalcumMarkProjcount = 300;
                            }
                        }
                    }

                    if (Main.rand.Next(100) < Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) count = Owner.GetModPlayer<GOGModPlayer>().GcountEx;
                }//转
                if (count == 2 * Gcount) {
                    for (int i = 0; i < 7; i++) {
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item26, Projectile.Center);

                        Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen, new Vector2(0, -4).RotatedBy(i * 2 * MathHelper.Pi / 7f), ModContent.ProjectileType<SirenProj>(), lastdamage, 1, Owner.whoAmI, 2);
                        if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                            if (proj1.ModProjectile is GOGProj proj2) {
                                proj2.OrichalcumMarkProj = true;
                                proj2.OrichalcumMarkProjcount = 300;
                            }
                        }
                    }

                    if (Main.rand.Next(100) < Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) count = Owner.GetModPlayer<GOGModPlayer>().GcountEx;
                }//吸
                if (count == 3 * Gcount && target1 != null) {
                    for (int i = 0; i < 1; i++) {
                        Vector2 vel = projcen.Toz(target1.Center) * 4f;

                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item26, Projectile.Center);

                        Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen, vel, ModContent.ProjectileType<SirenProj>(), lastdamage, 1, Owner.whoAmI, 3);
                        if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                            if (proj1.ModProjectile is GOGProj proj2) {
                                proj2.OrichalcumMarkProj = true;
                                proj2.OrichalcumMarkProjcount = 300;
                            }
                        }
                    }

                    if (Main.rand.Next(100) < Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) count = Owner.GetModPlayer<GOGModPlayer>().GcountEx;
                }//散
                if (count == 4 * Gcount && target1 != null) {
                    for (int i = -1; i < 2; i++) {
                        Vector2 vel = projcen.Toz(target1.Center) * 6f;

                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item26, Projectile.Center);

                        Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen, vel.RotatedBy(i * 0.3f - 0.3f), ModContent.ProjectileType<SirenProj>(), lastdamage, 1, Owner.whoAmI, 1);
                        if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                            if (proj1.ModProjectile is GOGProj proj2) {
                                proj2.OrichalcumMarkProj = true;
                                proj2.OrichalcumMarkProjcount = 300;
                            }
                        }
                    }

                    if (Main.rand.Next(100) < Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) count = Owner.GetModPlayer<GOGModPlayer>().GcountEx;
                }//合奏
                if (count == 5 * Gcount) {
                    for (int i = 0; i < mode; i++) {
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item26, Projectile.Center);

                        Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen, new Vector2(0, -1).RotatedByRandom(0.5f), ModContent.ProjectileType<SirenProj>(), lastdamage, 1, Owner.whoAmI, 4);
                        if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                            if (proj1.ModProjectile is GOGProj proj2) {
                                proj2.OrichalcumMarkProj = true;
                                proj2.OrichalcumMarkProjcount = 300;
                            }
                        }
                    }

                    if (Main.rand.Next(100) < Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) count = Owner.GetModPlayer<GOGModPlayer>().GcountEx;
                }

                //计时重置
                if (count > 5 * Gcount) count = Owner.GetModPlayer<GOGModPlayer>().GcountEx;
            }
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
                int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Wraith, 0f, 0f, 10, Color.White, 0.8f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].velocity *= 2f;
            }
            for (int j = 0; j < 15; j++) {
                int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Wraith, 0f, 0f, 10, Color.White, 0.4f);
                Main.dust[num2].noGravity = true;
                Main.dust[num2].velocity *= 1f;
            }
        }

        int drawcount = 0;
        public override bool PreDraw(ref Color lightColor) {
            drawcount++;
            float breath = (float)Math.Sin(drawcount / 18f);

            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "SirenDT").Value;
            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "SirenDT2").Value;

            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, -32);
            Vector2 drawPosition1 = Projectile.Center - Main.screenPosition + new Vector2(38, -8) + new Vector2(0, -8) * breath;

            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture1, drawPosition1, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}