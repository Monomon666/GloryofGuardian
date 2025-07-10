using System;
using GloryofGuardian.Common;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles {
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
                    int num  = Dust.NewDust(Projectile.Center + Projectile.velocity * Main.rand.NextFloat(60), 0, 0, DustID.Crimson, 0f, 0f, 50, Color.Red, 1f);
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
}
