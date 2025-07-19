using System;
using System.Collections.Generic;
using GloryofGuardian.Common;
using GloryofGuardian.Content.ParentClasses;
using Humanizer;
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

            if (mode == 1) {
                meleecount = 0;
                shadowcount = 0;
                shadowscale = 0;
            }
            if (mode == 2) {
                meleecount++;
                shadowcount++;
                shadowscale = Math.Min(1f, (shadowcount / 60f * 1.3f) - 0.3f);
            }


            //逐渐产生粒子,直到产生完全的粒子
            if (mode > 0 && drawcount >= 120) {
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
                    for (int i = 0; i < 2; i++) {
                        float angle = Main.rand.NextFloat(0, MathHelper.Pi * 2);
                        int num = Dust.NewDust(Projectile.Center + new Vector2(-6, -16) + new Vector2(0, Main.rand.Next(-180, 0)).RotatedBy(angle), 0, 0, DustID.Crimson, 0f, 0f, 50, Color.White, 1f);
                        Main.dust[num].velocity = new Vector2(0, -1.5f).RotatedBy(angle);
                        Main.dust[num].velocity *= 1f;
                        Main.dust[num].noGravity = true;
                    }
                }

                for (int i = 0; i < 48; i++) {
                    Lighting.AddLight(Projectile.Center + new Vector2(0, -180).RotatedBy(MathHelper.Pi / 24f * i + drawcount / 30f), new Vector3(0.5f, 0, 0));
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

                for (int i = 0; i < 48; i++) {
                    Lighting.AddLight(Projectile.Center + new Vector2(0, -40 - drawcount).RotatedBy(MathHelper.Pi / 24f * i + drawcount / 30f), new Vector3(0.5f, 0, 0));
                }
            }

            if (TileHelper.FindTilesInRectangle(Projectile.Center, 12, 12, 1, 12, true) != null) {
                mode = 1;
                if (target0 != null && target0.active) {
                    if (Vector2.Distance(Projectile.Center, target0.Center) < 240) {
                        mode = 2;
                        if (meleecount >= 120 && meleecount % 90 == 30) {
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
                    else mode = 1;
                }
                else mode = 1;
            }

            //todo 粒子动画
            base.AI();
        }

        protected override List<Projectile> Attack1() {
            List<Projectile> projlist = new List<Projectile>();

            if (mode == 0 || mode == 1) {
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

            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "Rhombus").Value;
            Texture2D texture2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "RhombusBlackShadow").Value;
            Texture2D texture3 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "RhombusShadow1").Value;
            Texture2D texture4 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "RhombusShadow2").Value;

            //if (mode > 0) {
            //    Vector2 drawPosition = AttackPos - Main.screenPosition + new Vector2(4, -0);
            //    for (int i = 0; i < 12; i++) {
            //        Main.EntitySpriteDraw(texture2,
            //            drawPosition + new Vector2(0, 16) + new Vector2(0, (float)(-174 + Math.Sin(drawcount / 20f) * 8f)).RotatedBy(MathHelper.Pi / 6f * i + drawcount / 60f),
            //            null,
            //            new Color(255, 0, 0, 100),
            //            Projectile.rotation + (MathHelper.Pi / 6f * i + drawcount / 60f),
            //            texture2.Size() * 0.5f, Projectile.scale * new Vector2(1.5f, 1), SpriteEffects.None, 0);
            //
            //        Main.EntitySpriteDraw(texture1,
            //            drawPosition + new Vector2(0, 16) + new Vector2(0, (float)(-174 + Math.Sin(drawcount / 20f) * 8f)).RotatedBy(MathHelper.Pi / 6f * i + drawcount / 60f),
            //            null,
            //            new Color(180, 0, 0, 0),
            //            Projectile.rotation + (MathHelper.Pi / 6f * i + drawcount / 60f),
            //            texture1.Size() * 0.5f, Projectile.scale * new Vector2(1.5f, 1), SpriteEffects.None, 0);
            //    }
            //}

            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, -18);

            if (mode == 2) {
                Main.EntitySpriteDraw(texture0bs, drawPosition0, null, lightColor * shadowscale, Projectile.rotation, texture0s.Size() * 0.5f, Projectile.scale * 1.1f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(texture0s, drawPosition0, null, new Color(255, 255, 255, 0) * shadowscale, Projectile.rotation, texture0s.Size() * 0.5f, Projectile.scale * 1.1f, SpriteEffects.None, 0);
            }

            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }

    public class Bloody2Proj1 : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 45;
            Projectile.height = 69;
            Projectile.localNPCHitCooldown = 20;
            Projectile.penetrate = 1;

            Projectile.scale *= 0.7f;
        }

        Player Owner => Main.player[Projectile.owner];

        public override void AI() {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;
            Lighting.AddLight(Projectile.Center, 1f, 0.5f, 0.5f);

            //临时遵循传入的重力的影响
            if (count < 4) {
                float G = Projectile.ai[0];
                Projectile.velocity += new Vector2(0, G);
            }
            //追踪并加速
            if (count >= 4) {
                if (target0 != null && target0.active) {
                    Projectile.velocity *= 0.9f;
                    Projectile.velocity += Projectile.Center.Toz(target0.Center) * Main.rand.NextFloat(2f, 2.5f);
                    if (Vector2.Distance(Projectile.Center, target0.Center) < 64) {
                        Projectile.velocity *= 0.7f;
                        Projectile.velocity += Projectile.Center.Toz(target0.Center) * 3;
                    }
                }
            }

            if (count >= 4) {
                for (int i = 0; i < 6; i++) {
                    int num = Dust.NewDust(Projectile.position, (int)(Projectile.width * Projectile.scale), (int)(Projectile.height * Projectile.scale), Main.rand.NextBool(16) ? DustID.Ichor : DustID.Crimson, 0f, 0f, 50, Color.White, 1.2f);
                    Main.dust[num].velocity *= 0.5f;
                    Main.dust[num].velocity -= Projectile.velocity / 4;
                    Main.dust[num].noGravity = true;

                    if (Main.rand.NextBool(2)) {
                        Main.dust[num].scale = 0.5f;
                        if (count >= 16) Main.dust[num].velocity *= 2f;
                    }
                    if (Main.rand.NextBool(8)) {
                        Main.dust[num].noGravity = false;
                    }

                }
            }

            base.AI();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            //指向的
            Attack(false, target.Center);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            //垂直的
            Vector2 vec = Vector2.Zero;
            //碰撞竖直墙壁
            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon) {
                vec = (oldVelocity.X > 0) ? new Vector2(-1, 0) : new Vector2(1, 0);
            }
            //碰撞水平地面
            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon) {
                vec = (oldVelocity.Y > 0) ? new Vector2(0, -1) : new Vector2(0, 1);
            }

            if (vec != Vector2.Zero) Attack(true, vec);
            return base.OnTileCollide(oldVelocity);
        }

        /// <summary>
        /// 生长血荆棘
        /// </summary>
        /// <param name="velorpos">true为垂直型,直接使用注入的vel;false为导向型,从弹幕尾部指向vek</param>
        /// <param name="vel"></param>
        void Attack(bool velorpos, Vector2 vel) {
            //在目标点范围内寻找最近的实心物块,并返回它们的集合
            Vector2 cen = velorpos ? Projectile.Center : vel;
            List<Vector2> tileCoordsList = TileHelper.FindTilesInRectangle(cen, 6, 6, 0, 8, true);
            if (tileCoordsList != null && tileCoordsList.Count != 0) {
                // 随机确定要返回的元素数量(2~3个)，但不超过列表的总元素数
                Random random = new Random();
                int countToReturn = Math.Min(random.Next(2, 4), tileCoordsList.Count);

                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item60, Projectile.Center);
                for (int i = 0; i < countToReturn; i++) {
                    int randomIndex = random.Next(tileCoordsList.Count);
                    // 获取选中的元素,向下偏移一格多
                    Vector2 selectedTile = tileCoordsList[randomIndex] + new Vector2(0, 16);
                    // 对选中的元素执行A()方法
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), selectedTile,
                    velorpos ? vel : selectedTile.Toz(vel)
                        , ModContent.ProjectileType<Bloody2Proj2>(), Projectile.damage, 8, Owner.whoAmI);
                }
            }
        }

        public override void OnKill(int timeLeft) {
            //爆炸
            for (int j = 0; j < 25; j++) {
                int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Crimson, 0f, 0f, 10, Color.Red, 1.7f);
                Main.dust[num2].noGravity = true;
                Main.dust[num2].velocity *= 1f;
                num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Crimson, 0f, 0f, 10);
                Main.dust[num2].velocity *= 1f;
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "Bloody2Proj1").Value;

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition, null, lightColor, Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }

    public class Bloody2Proj2 : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Main.projFrames[Projectile.type] = 6;//动图帧数

            Projectile.width = 204;
            Projectile.height = 48;
            Projectile.localNPCHitCooldown = 20;
            Projectile.penetrate = -1;

            Projectile.tileCollide = false;
            Projectile.scale *= 1f;
            drawtype = Main.rand.Next(6);
        }

        //比帧大一的数,用于一次性生成随机帧
        int drawtype = 0;
        float grow = 0;
        public override void AI() {
            Lighting.AddLight(Projectile.Center + Projectile.velocity.SafeNormalize(Vector2.Zero) * 96f, 3f, 0.5f, 0.5f);

            if (drawtype == 0) drawtype = Main.rand.Next(6) + 1;
            if (count <= 10) grow = Math.Max(0.4f, count / 10f);
            if (count >= 15) grow = Math.Max(0.4f, (25 - count) / 10f);
            if (count >= 20) Projectile.Kill();

            // 计算点积,速度向量的模,如果点积小于阈值，说明夹角大于45度
            float dotProduct = Vector2.Dot(Projectile.velocity, new Vector2(0, -1));
            float velocityMagnitude = Projectile.velocity.Length();
            float threshold = 0.7071f * velocityMagnitude;
            if (dotProduct < threshold) {
                Projectile.velocity = new Vector2(0, -1).RotatedBy(Main.rand.NextFloat(-MathHelper.PiOver4, MathHelper.PiOver4));
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.Center -= Projectile.velocity;

            if (count < 15) {
                for (int i = 0; i < 4; i++) {
                    int num = Dust.NewDust(Projectile.Center, 0, 0, DustID.Crimson, 0f, 0f, 50, Color.Red, 1f);
                    int num1 = Dust.NewDust(Projectile.Center, 0, 0, DustID.Crimson, 0f, 0f, 50, Color.Red, 1.2f);
                    int num2 = Dust.NewDust(Projectile.Center, 0, 0, DustID.Crimson, 0f, 0f, 50, Color.Black, 1f);
                    //Main.dust[num].velocity = Main.dust[num1].velocity = Main.dust[num2].velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * 4f;
                    Main.dust[num].position += Projectile.velocity.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(-24, 24);

                    Main.dust[num].velocity *= 2f;
                    Main.dust[num].noGravity = true;
                    Main.dust[num1].velocity *= 1f;
                    Main.dust[num1].noGravity = true;
                    Main.dust[num2].velocity *= 1.2f;
                    Main.dust[num2].noGravity = true;
                }
            }

            if (count < 15) {
                for (int i = 0; i < 4; i++) {
                    int num = Dust.NewDust(Projectile.Center + Projectile.velocity * Main.rand.NextFloat(60), 0, 0, DustID.Crimson, 0f, 0f, 50, Color.Red, 1f);
                    int num1 = Dust.NewDust(Projectile.Center + Projectile.velocity * Main.rand.NextFloat(120), 0, 0, DustID.Crimson, 0f, 0f, 50, Color.Red, 1.2f);
                    int num2 = Dust.NewDust(Projectile.Center + Projectile.velocity * Main.rand.NextFloat(180), 0, 0, DustID.Crimson, 0f, 0f, 50, Color.Black, 1f);
                    Main.dust[num].velocity = Main.dust[num1].velocity = Main.dust[num2].velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * 4f;
                    Main.dust[num].position += Projectile.velocity.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(-24, 24);

                    Main.dust[num].velocity *= 2f;
                    Main.dust[num].noGravity = true;
                    Main.dust[num1].velocity *= 1f;
                    Main.dust[num1].noGravity = true;
                    Main.dust[num2].velocity *= 1.2f;
                    Main.dust[num2].noGravity = true;
                }
            }

            base.AI();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            if (count < 30) {
                for (int i = 0; i < 40; i++) {
                    int num = Dust.NewDust(target.Center, 0, 0, DustID.Crimson, 0f, 0f, 50, Color.Red, 1f);
                    Main.dust[num].velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * 2f * Main.rand.NextFloat(2f);
                    Main.dust[num].scale *= Main.rand.NextFloat(1f, 1.5f);
                    Main.dust[num].position += Projectile.velocity.RotatedBy(MathHelper.PiOver2) * Main.rand.NextFloat(-24, 24);
                    Main.dust[num].noGravity = true;
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            return base.OnTileCollide(oldVelocity);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            bool collBool = false;
            float point = 0f;

            int length = Projectile.width;
            Vector2 startPos = Projectile.Center;
            Vector2 endPos = Projectile.Center + Projectile.rotation.ToRotationVector2() * length * Projectile.scale;

            collBool = Collision.CheckAABBvLineCollision(
                targetHitbox.TopLeft(),
                targetHitbox.Size(),
                startPos,
                endPos,
                48,
                ref point
                );

            return collBool;
        }

        public override void OnKill(int timeLeft) {
            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "Bloody2Proj2").Value;
            int singleFrameY = texture.Height / Main.projFrames[Type];

            Main.EntitySpriteDraw(texture, Projectile.Center - Main.screenPosition + Projectile.velocity.SafeNormalize(Vector2.Zero) * 0,
                new Rectangle(0, singleFrameY * (drawtype - 1), texture.Width, singleFrameY),//动图读帧,
                lightColor, Projectile.rotation, new Vector2(0, texture.Height / 2 / 6), new Vector2(1, grow), SpriteEffects.None, 0);

            return false;
        }
    }

    public class Bloody2Proj3 : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Main.projFrames[Projectile.type] = 6;//动图帧数

            Projectile.width = 44;
            Projectile.height = 69;

            Projectile.penetrate = 18;//用于计算攻击次数

            Projectile.tileCollide = false;
            Projectile.friendly = false;
            Projectile.scale *= 1f;

            Attackrange = 2400;
        }

        Player Owner => Main.player[Projectile.owner];

        List<int> ignore = new List<int>();
        NPC target1 = null;
        //mode -1 待机 0 运作 1 特殊攻击
        public override void AI() {
            if (target1 == null || !target1.active) target1 = Projectile.Center.InPosClosestNPC(800, 0, true, false, ignore);

            Projectile.velocity.Y *= 0f;
            Projectile.Center += new Vector2(0, (float)Math.Sin(drawcount / 10f));

            if (mode <= 0) {
                Lighting.AddLight(Projectile.Center, 1f, 0.5f, 0.5f);
                //依据物块上升
                if (TileHelper.TileRectangleDetection(Projectile.Center, 2, 3, 0, 12)) {
                    Projectile.Center += new Vector2(0, -2f);
                }

                if (target0 == null || !target0.active) {
                    Projectile.velocity.X *= 0.9f;
                    mode = -1;//待机
                }
                else {
                    //改变高度
                    if (Projectile.Center.Y - target0.Center.Y > 200) Projectile.Center += new Vector2(0, -2f);
                    if (target0.Center.Y - Projectile.Center.Y > 200) Projectile.Center += new Vector2(0, 2f);

                    if (mode == -1) {
                        mode = 0;
                        Projectile.velocity.X = Math.Max(-2, Math.Min(Projectile.Center.To(
                        oldtargetpos == Vector2.Zero ? target0.Center : oldtargetpos
                        ).X, 2f));
                    }
                    if (drawcount % 120 == 1) oldtargetpos = target0.Center;

                    Projectile.velocity *= 0.97f;
                    Projectile.velocity.X += target0.Center.X - Projectile.Center.X > 0 ? 0.2f : -0.2f;
                    if (target0.Center.X - Projectile.Center.X < 64 && target0.Center.X - Projectile.Center.X > -64) {
                        Projectile.velocity.X *= 1.03f;
                    }

                    if (Vector2.Distance(Projectile.Center, target0.Center) < 400) {
                        if (Projectile.penetrate >= 5 && count > 60) {
                            Projectile.penetrate -= Main.rand.Next(3) + 1;
                            Attack1();
                            count = Main.rand.Next(45);
                        }
                        if (Projectile.penetrate < 5) {
                            count = -120;
                            mode = 1;//进入蓄力模式
                        }
                    }
                }
            }

            if (mode == 1) {
                Lighting.AddLight(Projectile.Center, 1f, 1f, 0.5f);
                Projectile.velocity *= 0;
                if (count >= 0) {
                    if (count % 20 == 19) {

                        if (target1 != null && target1.active) {
                            ignore.Add(target1.whoAmI);
                            Attack2(target1);
                            target1 = null;
                            Projectile.penetrate -= 1;
                        }
                        count = 0;

                        if (Projectile.penetrate <= 0) Projectile.Kill();
                    }
                }

                if (count > 180) Projectile.Kill();
            }

            //帧图
            Projectile.frameCounter++;
            Projectile.frame = (Projectile.frameCounter / 6)
                % 6;//要手动填，不然会出错
            base.AI();
        }

        void Attack1() {
            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item21, Projectile.Center);
            Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), (Projectile.Center + new Vector2(-2, 16)), (Projectile.Center + new Vector2(-2, 16)).Toz(target0.Center) * 12f, ModContent.ProjectileType<Bloody2Proj4>(), Projectile.damage, 8, Owner.whoAmI);
        }

        void Attack2(NPC target1) {
            ignore.Add(target1.whoAmI);
            if (target1 != null && target1.active) {
                Vector2 AttackPos = (Projectile.Center + new Vector2(-2, 16));
                //发射参数计算
                float dx = (target1.Center).X - AttackPos.X;
                float dy = (target1.Center).Y - AttackPos.Y;
                //设置一个相对标准的下落加速度
                float G = 0.3f;
                //设置一个相对标准的初始垂直速度
                float vy = 6;

                float vx = dx / ((vy + (float)Math.Sqrt(vy * vy + 2 * G * dy)) / G);
                Vector2 velfire = new Vector2(vx, -vy);

                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item13, Projectile.Center);
                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), AttackPos, velfire, ModContent.ProjectileType<Bloody2Proj4>(), Projectile.damage, 6, Owner.whoAmI, G);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            return base.OnTileCollide(oldVelocity);
        }

        public override void OnKill(int timeLeft) {
            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "Bloody2Proj3").Value;
            Texture2D textures = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "Bloody2Proj3Shadow").Value;

            int singleFrameY = texture.Height / Main.projFrames[Type];
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0, 0);

            if (mode == 1) {
                Main.spriteBatch.Draw(textures, drawPos,
                new Rectangle(0, singleFrameY * Projectile.frame, textures.Width, singleFrameY),//动图读帧
                new Color(255, 255, 255, 0), Projectile.rotation, new Vector2(Projectile.width / 2, Projectile.height / 2), Projectile.scale * 1.1f, SpriteEffects.None, 0);
            }

            Main.spriteBatch.Draw(texture, drawPos,
            new Rectangle(0, singleFrameY * Projectile.frame, texture.Width, singleFrameY),//动图读帧
            lightColor, Projectile.rotation, new Vector2(Projectile.width / 2, Projectile.height / 2), Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }

    public class Bloody2Proj4 : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {

            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.localNPCHitCooldown = 20;
            Projectile.penetrate = 5;
        }

        public override void AI() {
            if (Projectile.ai[0] != 0) {
                Projectile.extraUpdates = 2;
                //遵循传入的重力的影响
                float G = Projectile.ai[0];
                Projectile.velocity += new Vector2(0, G);
            }

            if (count % 1 == 0) {
                for (int i = 0; i < 4; i++) {
                    int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Ichor, 0f, 0f, 50, Color.White, 1f);
                    Main.dust[num].velocity *= 0f;
                    Main.dust[num].noGravity = true;
                }
            }

            if (count % 1 == 0) {
                for (int i = 0; i < 4; i++) {
                    int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Ichor, 0f, 0f, 50, Color.White, 0.8f);
                    Main.dust[num].velocity *= 0.5f;
                    Main.dust[num].noGravity = true;
                }
            }

            if (count % 1 == 0 && Projectile.ai[1] > 1) {
                for (int i = 0; i < 4; i++) {
                    int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Ichor, 0f, 0f, 50, Color.White, 1f);
                    Main.dust[num].velocity *= 0f;
                    Main.dust[num].noGravity = true;
                }
            }

            base.AI();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            target.AddBuff(BuffID.Ichor, 300);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            return base.OnTileCollide(oldVelocity);
        }

        public override void OnKill(int timeLeft) {
            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor) {
            return base.PreDraw(ref lightColor);
        }
    }
}
