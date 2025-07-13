using System;
using GloryofGuardian.Common;
using GloryofGuardian.Content.Class;
using GloryofGuardian.Content.ParentClasses;
using InnoVault;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles {
    public class AncienProj : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.localNPCHitCooldown = 12;
            Projectile.penetrate = -1;

            Projectile.scale *= 0.8f;
        }
        Player Owner => Main.player[Projectile.owner];

        public override void AI() {
            Projectile.rotation += Main.rand.NextFloat(0.1f, 0.3f);
            if (count >= 90) Projectile.rotation += 0.3f;
            if (count >= 30 && count <= 90) Projectile.velocity *= 1.05f;

            Lighting.AddLight(Projectile.Center, new Vector3(2.25f / 5f, 2.25f / 5f, 1 / 5f));

            if (count % 1 == 0) {
                for (int i = 0; i < 6; i++) {
                    int num = Dust.NewDust(Projectile.Center + new Vector2(0, -4) + new Vector2(0, -12).RotatedBy(Main.rand.NextFloat(MathHelper.Pi * 2)), 0, 0, DustID.SandstormInABottle, 0f, 0f, 50, Color.White, 0.8f);
                    Main.dust[num].velocity = Projectile.Center.Toz(Main.dust[num].position).RotatedBy(MathHelper.PiOver2) * 2f;
                    Main.dust[num].noGravity = true;
                }
            }

            if (count % 1 == 0) {
                for (int i = 0; i < 6; i++) {
                    int num = Dust.NewDust(Projectile.Center + new Vector2(0, -4) + new Vector2(0, -24).RotatedBy(Main.rand.NextFloat(MathHelper.Pi * 2)), 0, 0, DustID.SandstormInABottle, 0f, 0f, 50, Color.White, 0.8f);
                    Main.dust[num].velocity = Projectile.Center.Toz(Main.dust[num].position).RotatedBy(MathHelper.PiOver2) * 2f;
                    Main.dust[num].noGravity = true;
                }
            }

            //if (Projectile.ai[0] == 1 && count % 1 == 0) {
            //    for (int i = 0; i < 6; i++) {
            //        int num = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, DustID.SandstormInABottle, 0f, 0f, 50, Color.White, 0.8f);
            //        Main.dust[num].velocity *= 1f;
            //        Main.dust[num].noGravity = true;
            //    }
            //}

            Projectile.alpha -= 5;
            if (Projectile.alpha < 50) {
                Projectile.alpha = 50;
            }

            if (hitcount != 0) Projectile.alpha += 12;
            if (count > 180 ||(count > hitcount && hitcount != 0)) Projectile.Kill();
            base.AI();
        }

        int hitcount = 0;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            if (hitcount == 0) {
                hitcount = count + 20;
                //Projectile.tileCollide = false;
            }
            base.OnHitNPC(target, hit, damageDone);
        }

        public override Color? GetAlpha(Color lightColor) {
            return new Color(225, 225, 100);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            if (hitcount == 0) {
                hitcount = count + 60;
            }
            return false;
        }

        public override void OnKill(int timeLeft) {
            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor) {
            DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);

            return false;
        }

        void DrawAfterimagesCentered(Projectile proj, int mode, Color lightColor, int typeOneIncrement = 1, Texture2D texture = null, bool drawCentered = true) {
            if (texture is null)
                texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "Wind").Value;

            int frameHeight = texture.Height / Main.projFrames[proj.type];
            int frameY = frameHeight * proj.frame;
            float scale = proj.scale;
            float rotation = proj.rotation;

            Rectangle rectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 origin = rectangle.Size() / 2f;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (proj.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Vector2 centerOffset = drawCentered ? proj.Size / 2f : Vector2.Zero;
            Color alphaColor = proj.GetAlpha(Color.LightYellow);

            if (mode == 0) {
                for (int i = 0; i < proj.oldPos.Length; ++i) {
                    Vector2 drawPos = proj.oldPos[i] + centerOffset - Main.screenPosition + new Vector2(0f, proj.gfxOffY);
                    Color color = alphaColor * ((proj.oldPos.Length - i) / (float)proj.oldPos.Length) * 2;
                    Main.spriteBatch.Draw(texture, drawPos, new Rectangle?(rectangle), color, rotation, origin, scale, spriteEffects, 0f);
                }
            }
            else if (mode == 1) {
                int increment = Math.Max(1, typeOneIncrement);
                Color drawColor = alphaColor;
                int afterimageCount = ProjectileID.Sets.TrailCacheLength[proj.type];
                float afterimageColorCount = afterimageCount * 1.5f;
                int k = 0;
                while (k < afterimageCount) {
                    Vector2 drawPos = proj.oldPos[k] + centerOffset - Main.screenPosition + new Vector2(0f, proj.gfxOffY);
                    if (k > 0) {
                        float colorMult = afterimageCount - k;
                        drawColor *= colorMult / afterimageColorCount * 2;
                    }
                    Main.spriteBatch.Draw(texture, drawPos, new Rectangle?(rectangle), drawColor, rotation, origin, scale, spriteEffects, 0f);
                    k += increment;
                }
            }
            else if (mode == 2) {
                for (int i = 0; i < proj.oldPos.Length; ++i) {
                    float afterimageRot = proj.oldRot[i];
                    SpriteEffects sfxForThisAfterimage = proj.oldSpriteDirection[i] == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
                    Vector2 drawPos = proj.oldPos[i] + centerOffset - Main.screenPosition + new Vector2(0f, proj.gfxOffY);
                    Color color = alphaColor * ((proj.oldPos.Length - i) / (float)proj.oldPos.Length) * 2;
                    Main.spriteBatch.Draw(texture, drawPos, new Rectangle?(rectangle), color, afterimageRot, origin, scale, sfxForThisAfterimage, 0f);
                }
            }
        }
    }
}
