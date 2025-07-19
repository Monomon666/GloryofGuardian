using System;
using System.Collections.Generic;
using GloryofGuardian.Common;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles {
    public class BloodyDT : GOGDT {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 64;
            Projectile.friendly = false;
            Projectile.penetrate = -1;

            OtherHeight = 34;

            count0 = 60;

            exdust = DustID.Crimson;
        }

        Player Owner => Main.player[Projectile.owner];

        public override void AI() {
            AttackPos = Projectile.Center + new Vector2(-4, -30);

            //todo 粒子动画
            base.AI();
        }

        protected override List<Projectile> Attack1() {
            List<Projectile> projlist = new List<Projectile>();

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

                Terraria.Audio.SoundEngine.PlaySound(SoundID.NPCDeath13, Projectile.Center);
                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), AttackPos, velfire, ModContent.ProjectileType<BloodyProj>(), lastdamage, 6, Owner.whoAmI, G);

                projlist.Add(proj1);
            }

            FinishAttack = true;
            return projlist;
        }

        protected override List<Projectile> Attack2() {
            List<Projectile> projlist = new List<Projectile>();

            for (int i = 0; i < 1; i++) {
                Vector2 vel = new Vector2(0, -16);

                for (int j = 0; j < 25; j++) {
                    int num = Dust.NewDust(AttackPos + new Vector2(0, 16), 0, 0, DustID.Crimson, 0f, 0f, 50, Color.Red, 1.5f);
                    int num1 = Dust.NewDust(AttackPos + new Vector2(0, 16), 0, 0, DustID.Crimson, 0f, 0f, 50, Color.Red, 0.6f);
                    int num2 = Dust.NewDust(AttackPos + new Vector2(0, 16), 0, 0, DustID.Crimson, 0f, 0f, 50, Color.Black, 1f);

                    if (Main.rand.NextBool(2)) Main.dust[num].velocity += Main.dust[num1].velocity = Main.dust[num2].velocity
                        = new Vector2(0, -2).RotatedBy(MathHelper.PiOver4 * 0.8f) * 1.5f;
                    else Main.dust[num].velocity += Main.dust[num1].velocity = Main.dust[num2].velocity
                        = new Vector2(0, -2).RotatedBy(-MathHelper.PiOver4 * 0.8f) * 1.5f;

                    Main.dust[num].velocity *= 2f;
                    Main.dust[num].noGravity = true;
                    Main.dust[num1].velocity *= 1f;
                    Main.dust[num1].noGravity = false;
                    Main.dust[num2].velocity *= 1.2f;
                    Main.dust[num2].noGravity = true;
                }

                Terraria.Audio.SoundEngine.PlaySound(SoundID.NPCDeath13, Projectile.Center);
                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), AttackPos + new Vector2(0, 16), vel, ModContent.ProjectileType<BloodyProj2>(), lastdamage, 8, Owner.whoAmI);

                projlist.Add(proj1);
            }

            FinishAttack = true;
            return projlist;
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "BloodyDT").Value;

            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, -16);
            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }

    public class BloodyProj : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.localNPCHitCooldown = 20;
            Projectile.penetrate = 1;

            if (Projectile.ai[1] == 0) Projectile.extraUpdates = 1;

            Projectile.scale *= Main.rand.NextFloat(1, 1.5f);
            if (Projectile.ai[1] == 2) Projectile.tileCollide = false;
        }

        Player Owner => Main.player[Projectile.owner];

        public override void AI() {
            //遵循传入的重力的影响
            float G = Projectile.ai[0];
            Projectile.velocity += new Vector2(0, G);

            if (Projectile.ai[1] == 1 && count < 20) Projectile.friendly = false;
            if (Projectile.ai[1] == 1 && count >= 20) Projectile.friendly = true;


            if (count % 1 == 0) {
                for (int i = 0; i < 4; i++) {
                    int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Crimson, 0f, 0f, 50, Color.Red, 1f);
                    Main.dust[num].velocity *= 0f;
                    Main.dust[num].noGravity = true;
                }
            }

            if (count % 1 == 0) {
                for (int i = 0; i < 4; i++) {
                    int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Crimson, 0f, 0f, 50, Color.Red, 0.8f);
                    Main.dust[num].velocity *= 0.5f;
                    Main.dust[num].noGravity = true;
                }
            }

            if (count % 1 == 0 && Projectile.ai[1] > 1) {
                for (int i = 0; i < 4; i++) {
                    int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Crimson, 0f, 0f, 50, Color.Black, 1f);
                    Main.dust[num].velocity *= 0f;
                    Main.dust[num].noGravity = true;
                }
            }

            if (Projectile.ai[1] == 2 && count > 15) Projectile.Kill();

            base.AI();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            int dustnum = 0;
            if (Projectile.ai[1] == 0) dustnum = 25;
            if (Projectile.ai[1] == 1) dustnum = 10;
            if (Projectile.ai[1] == 2) dustnum = 5;

            if (Projectile.ai[1] == 2) target.velocity /= 2;

            for (int i = 0; i < dustnum; i++) {
                int num = Dust.NewDust(Projectile.Center, 0, 0, DustID.Crimson, 0f, 0f, 50, Color.Red, 1f);
                int num1 = Dust.NewDust(Projectile.Center, 0, 0, DustID.Crimson, 0f, 0f, 50, Color.Red, 0.6f);
                int num2 = Dust.NewDust(Projectile.Center, 0, 0, DustID.Crimson, 0f, 0f, 50, Color.Black, 1f);
                Main.dust[num].velocity *= 2f;
                Main.dust[num].noGravity = true;
                Main.dust[num1].velocity *= 1f;
                Main.dust[num1].noGravity = false;
                Main.dust[num2].velocity *= 1.2f;
                Main.dust[num2].noGravity = true;
            }

            if (Projectile.ai[1] == 0) {
                for (int i = 0; i < Main.rand.Next(2) + 1; i++) {
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center, new Vector2(0, -4).RotatedByRandom(MathHelper.Pi * 2), ModContent.ProjectileType<BloodyProj>(), Projectile.damage / 2, 6, Owner.whoAmI, Projectile.ai[0], 1);
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            if (Projectile.ai[1] == 0) {
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);

                //碰撞竖直墙壁
                if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon) {
                    for (int i = 0; i < 25; i++) {
                        int num = Dust.NewDust(Projectile.Center, 0, 0, DustID.Crimson, 0f, 0f, 50, Color.Red, 1.5f);
                        int num1 = Dust.NewDust(Projectile.Center, 0, 0, DustID.Crimson, 0f, 0f, 50, Color.Red, 0.6f);
                        int num2 = Dust.NewDust(Projectile.Center, 0, 0, DustID.Crimson, 0f, 0f, 50, Color.Black, 1f);

                        if (Main.rand.NextBool(2)) Main.dust[num].velocity += Main.dust[num1].velocity = Main.dust[num2].velocity
                            = new Vector2(-oldVelocity.X, 0).RotatedBy(MathHelper.PiOver4) * 1.5f;
                        else Main.dust[num].velocity += Main.dust[num1].velocity = Main.dust[num2].velocity
                            = new Vector2(-oldVelocity.X, 0).RotatedBy(-MathHelper.PiOver4) * 1.5f;

                        Main.dust[num].velocity *= 2f;
                        Main.dust[num].noGravity = true;
                        Main.dust[num1].velocity *= 1f;
                        Main.dust[num1].noGravity = false;
                        Main.dust[num2].velocity *= 1.2f;
                        Main.dust[num2].noGravity = true;
                    }
                }

                //碰撞水平地面
                if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon) {
                    for (int i = 0; i < 25; i++) {
                        int num = Dust.NewDust(Projectile.Center, 0, 0, DustID.Crimson, 0f, 0f, 50, Color.Red, 1.5f);
                        int num1 = Dust.NewDust(Projectile.Center, 0, 0, DustID.Crimson, 0f, 0f, 50, Color.Red, 0.6f);
                        int num2 = Dust.NewDust(Projectile.Center, 0, 0, DustID.Crimson, 0f, 0f, 50, Color.Black, 1f);

                        if (Main.rand.NextBool(2)) Main.dust[num].velocity += Main.dust[num1].velocity = Main.dust[num2].velocity
                            = new Vector2(0, -oldVelocity.Y).RotatedBy(MathHelper.PiOver4) / 4;
                        else Main.dust[num].velocity += Main.dust[num1].velocity = Main.dust[num2].velocity
                            = new Vector2(0, -oldVelocity.Y).RotatedBy(-MathHelper.PiOver4) / 4;

                        Main.dust[num].velocity *= 2f;
                        Main.dust[num].noGravity = true;
                        Main.dust[num1].velocity *= 1f;
                        Main.dust[num1].noGravity = false;
                        Main.dust[num2].velocity *= 1.2f;
                        Main.dust[num2].noGravity = true;
                    }
                    //Projectile.velocity.Y = -oldVelocity.Y * 0.5f;
                }

                for (int i = 0; i < Main.rand.Next(2) + 1; i++) {
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BloodyProj>(), Projectile.damage / 2, 6, Owner.whoAmI, Projectile.ai[0], 1);

                    if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon) proj1.velocity = new Vector2(-oldVelocity.X / 2f, Main.rand.NextFloat(-2, 2));
                    if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon) proj1.velocity = new Vector2(Main.rand.NextFloat(-2, 2), -oldVelocity.Y / 2f);
                }

                return true;
            }
            else return true;
        }

        public override void OnKill(int timeLeft) {
            //爆炸
            if (Projectile.ai[1] == 1) {
                for (int j = 0; j < 5; j++) {
                    int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Crimson, 0f, 0f, 10, Color.Black, 1.7f);
                    Main.dust[num2].noGravity = true;
                    Main.dust[num2].velocity *= 1f;
                    num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Crimson, 0f, 0f, 10);
                    Main.dust[num2].velocity *= 1f;
                }
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

        public override bool PreDraw(ref Color lightColor) {
            return false;
        }
    }

    public class BloodyProj2 : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Main.projFrames[Projectile.type] = 6;//动图帧数

            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.localNPCHitCooldown = 0;
            Projectile.penetrate = 4;

            Projectile.scale = 0.6f;

            // 旋转90度
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(180f);
            framevel = Main.rand.Next(3, 7);
        }

        int framevel = 0;
        bool hungry = false;

        Player Owner => Main.player[Projectile.owner];

        public override void AI() {
            if (!hadbit) normal();
            else biting();

            //帧图
            Projectile.frameCounter++;
            Projectile.frame = (Projectile.frameCounter / framevel)
                % 6;//要手动填，不然会出错
        }

        void normal() {
            // 旋转90度
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(180f);
            if (count >= Main.rand.Next(5, 20)) hungry = true;

            if (hungry) {
                //追踪,路径释放粒子,咬两口,击中墙壁爆炸
                if (target0 != null && target0.active) {
                    Projectile.velocity *= 0.95f;
                    Projectile.velocity += Projectile.Center.Toz(target0.Center) * Main.rand.NextFloat(0.5f, 1.5f);
                    if (Vector2.Distance(Projectile.Center, target0.Center) < 64) {
                        Projectile.velocity *= 0.7f;
                        Projectile.velocity += Projectile.Center.Toz(target0.Center) * Main.rand.NextFloat(1f, 1.5f);
                    }
                }

                if (count % 1 == 0) {
                    for (int i = 0; i < 12; i++) {
                        int num = Dust.NewDust(Projectile.position, (int)(Projectile.width * Projectile.scale), (int)(Projectile.height * Projectile.scale), DustID.Crimson, 0f, 0f, 50, Color.Red, 0.8f);
                        Main.dust[num].velocity *= 0.5f;
                        Main.dust[num].velocity -= Projectile.velocity / 4;
                        Main.dust[num].noGravity = true;

                        if (Main.rand.NextBool(2)) {
                            Main.dust[num].scale = 0.5f;
                        }
                        if (Main.rand.NextBool(8)) {
                            Main.dust[num].noGravity = false;
                        }
                    }
                }
            }
        }

        void biting() {
            if (Main.npc[bittennpc] != null && Main.npc[bittennpc].active) {
                Projectile.Center = Main.npc[bittennpc].Center + bitepos;
                if (count % 60 == 0) {
                    //释放次级弹幕
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center - Projectile.rotation.ToRotationVector2() * 16, Vector2.Zero, ModContent.ProjectileType<BloodyProj>(), Projectile.damage / 2, 6, Owner.whoAmI, Projectile.ai[0], 2);
                    proj1.velocity = -Projectile.rotation.ToRotationVector2() * 8;

                    for (int i = 0; i < 24; i++) {
                        int num = Dust.NewDust(Projectile.position, (int)(Projectile.width * Projectile.scale), (int)(Projectile.height * Projectile.scale), DustID.Crimson, 0f, 0f, 50, Color.Red, 1.5f);
                        Main.dust[num].velocity = -Projectile.rotation.ToRotationVector2() * 8;
                        Main.dust[num].noGravity = true;

                        if (Main.rand.NextBool(2)) {
                            Main.dust[num].scale *= 1f;
                            Main.dust[num].velocity *= 2;
                        }
                        if (Main.rand.NextBool(2)) {
                            Main.dust[num].scale *= 0.8f;
                            Main.dust[num].velocity *= 1.5f;
                        }
                    }

                    if (Projectile.penetrate > 1) Projectile.penetrate -= 1;
                    if (Projectile.penetrate == 1) Projectile.Kill();
                }
            }
            else {
                hadbit = false;
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 8f;
            }
        }

        bool hadbit = false;
        int bittennpc = 0;
        Vector2 bitepos = Vector2.Zero;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            hadbit = true;
            Projectile.friendly = false;
            bittennpc = target.whoAmI;
            bitepos = target.Center.To(Projectile.Center);

            if (Owner.statLifeMax2 != Owner.statLife) {
                int addlife = Math.Min(Owner.statLifeMax2 - Owner.statLife, 3);
                Owner.statLife += addlife;
                CombatText.NewText(Owner.Hitbox,
                                Color.LightGreen,
                                addlife,
                                false,
                                false
                                );
            }

            //如果满血就整点儿活
            if (Owner.statLifeMax2 == Owner.statLife) {
                CombatText.NewText(Owner.Hitbox,
                                Color.LightGreen,
                                GOGUtils.Full(),
                                false,
                                false
                                );
            }

            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            return base.OnTileCollide(oldVelocity);
        }

        public override void OnKill(int timeLeft) {
            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "BloodyProj2").Value;
            int singleFrameY = texture.Height / Main.projFrames[Type];
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0, 0);

            Main.spriteBatch.Draw(
                texture,
                drawPos,
                new Rectangle(0, singleFrameY * Projectile.frame, texture.Width, singleFrameY),//动图读帧
                lightColor,
                Projectile.rotation,
                new Vector2(34, 18) / Projectile.scale,
                Projectile.scale * 1.2f,
                SpriteEffects.None,
                0);
            return true;
        }
    }
}
