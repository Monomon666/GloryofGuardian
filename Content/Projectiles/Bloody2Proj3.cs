using System;
using System.Collections.Generic;
using System.Linq;
using GloryofGuardian.Common;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles {
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
}
