using System;
using System.Collections.Generic;
using GloryofGuardian.Common;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles {
    public class Bloody2DT : GOGDT {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 80;
            Projectile.friendly = false;
            Projectile.penetrate = -1;

            OtherHeight = 38;

            count0 = 60;

            exdust = DustID.Crimson;
        }

        int meleecount = 0;
        int shadowcount = 0;
        float shadowscale = 0;
        Player Owner => Main.player[Projectile.owner];
        public override void AI() {
            AttackPos = Projectile.Center + new Vector2(-4, -30);
            Lighting.AddLight(Projectile.Center, 2f, 0.5f, 0.5f);

            if (mode == 0) {
                meleecount = 0;
                shadowcount = 0;
                shadowscale = 0;
            }
            if (mode == 1) {
                meleecount++;
                shadowcount++;
                shadowscale = Math.Min(1f, (shadowcount / 60f * 1.3f) - 0.3f);
            }

            //逐渐产生粒子,直到产生完全的粒子
            if (drawcount >= 120) {
                //包围圈和内部粒子
                if (drawcount % 1 == 0) {
                    for (int i = 0; i < 4; i++) {
                        float angle = Main.rand.NextFloat(0, MathHelper.Pi * 2);
                        int num = Dust.NewDust(Projectile.Center + new Vector2(-6, -16) + new Vector2(0, -180).RotatedBy(angle), 0, 0, DustID.Crimson, 0f, 0f, 50, Color.White, 3f);
                        Main.dust[num].velocity = new Vector2(0, -1.5f).RotatedBy(angle + MathHelper.PiOver2);
                        Main.dust[num].velocity *= 1f;
                        Main.dust[num].noGravity = true;
                        Main.dust[num].scale *= Main.rand.NextFloat(0.5f, 1);
                        if (Main.rand.NextBool(60)) Main.dust[num].noGravity = false;
                    }
                }
                if (drawcount % 1 == 0) {
                    for (int i = 0; i < 3; i++) {
                        float angle = Main.rand.NextFloat(0, MathHelper.Pi * 2);
                        int num = Dust.NewDust(Projectile.Center + new Vector2(-6, -16) + new Vector2(0, Main.rand.Next(-180, 0)).RotatedBy(angle), 0, 0, DustID.Crimson, 0f, 0f, 50, Color.White, 1f);
                        Main.dust[num].velocity = new Vector2(0, -1.5f).RotatedBy(angle);
                        Main.dust[num].velocity *= 1f;
                        Main.dust[num].noGravity = true;
                    }
                }
            }
            else if (drawcount < 120) {
                //包围圈和内部粒子
                if (drawcount % 1 == 0) {
                    for (int i = 0; i < (count / 40) + 1; i++) {
                        float angle = Main.rand.NextFloat(0, MathHelper.Pi * 2);
                        int num = Dust.NewDust(Projectile.Center + new Vector2(-6, -16) + new Vector2(0, drawcount * 1.5f).RotatedBy(angle), 0, 0, DustID.Crimson, 0f, 0f, 50, Color.White, 3f);
                        Main.dust[num].velocity = new Vector2(0, -1.5f).RotatedBy(angle + MathHelper.PiOver2);
                        Main.dust[num].velocity *= 1f;
                        Main.dust[num].noGravity = true;
                        Main.dust[num].scale *= Main.rand.NextFloat(0.5f, 1);
                    }
                }
                if (drawcount % 1 == 0) {
                    for (int i = 0; i < 3; i++) {
                        float angle = Main.rand.NextFloat(0, MathHelper.Pi * 2);
                        int num = Dust.NewDust(Projectile.Center + new Vector2(-6, -16) + new Vector2(0, Main.rand.Next(-40 - drawcount, 0)).RotatedBy(angle), 0, 0, DustID.Crimson, 0f, 0f, 50, Color.White, 1f);
                        Main.dust[num].velocity = new Vector2(0, -1.5f).RotatedBy(angle);
                        Main.dust[num].velocity *= 1f;
                        Main.dust[num].noGravity = true;
                    }
                }
            }

            if (target0 != null && target0.active) {
                if (Vector2.Distance(Projectile.Center, target0.Center) < 240) {
                    mode = 1;
                    if (meleecount >= 120 && meleecount % 90  == 30) {
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item60, Projectile.Center);

                        //在目标点范围内寻找最近的实心物块,并返回它们的集合
                        List<Vector2> tileCoordsList = TileHelper.FindTilesInRectangle(Projectile.Center, 12, 12, 1, 12, true);
                        List<Vector2> tileCoordsList2 = TileHelper.FindTilesInRectangle(target0.Center, 3, 3, 0, 8, true);

                        if (tileCoordsList != null && tileCoordsList.Count != 0) {
                            // 随机确定要返回的元素数量(2~3个)，但不超过列表的总元素数
                            Random random = new Random();
                            int countToReturn = Math.Min(random.Next(4, 7), tileCoordsList.Count);

                            for (int i = 0; i < countToReturn; i++) {
                                int randomIndex = random.Next(tileCoordsList.Count);
                                // 获取选中的元素,向下偏移一格多
                                Vector2 selectedTile = tileCoordsList[randomIndex] + new Vector2(0, 16);
                                // 对选中的元素执行A()方法
                                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), selectedTile,
                                selectedTile.Toz(target0.Center)
                                    , ModContent.ProjectileType<Bloody2Proj2>(), Projectile.damage, 8, Owner.whoAmI);
                            }
                        }
                        if (tileCoordsList2 != null && tileCoordsList2.Count != 0) {
                            // 随机确定要返回的元素数量(2~3个)，但不超过列表的总元素数
                            Random random = new Random();
                            int countToReturn = Math.Min(random.Next(1, 3), tileCoordsList2.Count);

                            for (int i = 0; i < countToReturn; i++) {
                                int randomIndex = random.Next(tileCoordsList2.Count);
                                // 获取选中的元素,向下偏移一格多
                                Vector2 selectedTile = tileCoordsList2[randomIndex] + new Vector2(0, 16);
                                // 对选中的元素执行A()方法
                                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), selectedTile,
                                selectedTile.Toz(target0.Center)
                                    , ModContent.ProjectileType<Bloody2Proj2>(), Projectile.damage, 8, Owner.whoAmI);
                            }
                        }
                    }
                }
                else mode = 0;
            }
            else mode = 0;

            //todo 粒子动画
            base.AI();
        }

        protected override List<Projectile> Attack1() {
            List<Projectile> projlist = new List<Projectile>();

            if (mode == 0) {
                //发射参数计算
                float dx = target0.Center.X - AttackPos.X;
                float dy = target0.Center.Y - AttackPos.Y;
                //设置一个相对标准的下落加速度
                float G = 0.3f;
                //设置一个相对标准的初始垂直速度
                float vy = 16;
                for (int i = 0; i < 1; i++) {
                    G *= Main.rand.NextFloat(0.8f, 1.2f);
                    vy *= Main.rand.NextFloat(0.9f, 1.1f);
                    vy *= (dy >= 0 ? 0.75f : 1.2f);
                    float vx = dx / ((vy + (float)Math.Sqrt(vy * vy + 2 * G * dy)) / G);
                    Vector2 velfire = new Vector2(vx * Main.rand.NextFloat(0.9f, 1.1f), -vy * Main.rand.NextFloat(0.98f, 1.02f));//降低精度

                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), AttackPos, velfire, ModContent.ProjectileType<Bloody2Proj1>(), lastdamage, 6, Owner.whoAmI, G);

                    projlist.Add(proj1);
                }
            }

            FinishAttack = true;
            return projlist;
        }

        protected override List<Projectile> Attack2() {
            List<Projectile> projlist = new List<Projectile>();

            if (Owner.ownedProjectileCounts[ModContent.ProjectileType<Bloody2Proj3>()] < 2) {
                for (int i = 0; i < 1; i++) {
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.NPCDeath13, Projectile.Center);
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), AttackPos, new Vector2(0, -2), ModContent.ProjectileType<Bloody2Proj3>(), lastdamage, 6, Owner.whoAmI);

                    projlist.Add(proj1);
                }
            }else {
                Attack1();
            }

            FinishAttack = true;
            return projlist;
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "Bloody2DT").Value;
            Texture2D texture0s = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "Bloody2DTShadow").Value;
            Texture2D texture0bs = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "Bloody2DTBlackShadow").Value;

            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, -18);

            if (mode == 1) {
                Main.EntitySpriteDraw(texture0bs, drawPosition0, null, lightColor * shadowscale, Projectile.rotation, texture0s.Size() * 0.5f, Projectile.scale * 1.1f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(texture0s, drawPosition0, null, new Color(255, 255, 255, 0) * shadowscale, Projectile.rotation, texture0s.Size() * 0.5f, Projectile.scale * 1.1f, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
