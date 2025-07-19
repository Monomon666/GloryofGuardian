using System;
using System.Collections.Generic;
using GloryofGuardian.Common;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;
using static GloryofGuardian.Common.GOGUtils;

namespace GloryofGuardian.Content.Projectiles {
    public class HarpyProj2 : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.localNPCHitCooldown = 20;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;

            Projectile.alpha = 255;

            Projectile.scale *= 0.6f;

            base.SetProperty();
        }
        Player Owner => Main.player[Projectile.owner];

        //三种状态:0:飞行 1:钉入 2:收回
        //等一秒后收回
        public override void AI() {
            Lighting.AddLight(Projectile.Center, 0.5f, 0.5f, 0f);

            //注!!!ai1 被占用!!!
            if (count > 60 + Projectile.ai[0] * Projectile.ai[2]) {
                mode = 2;

                Projectile.friendly = true;
                Projectile.penetrate = -1;
                Projectile.localNPCHitCooldown = 10;

                Projectile.velocity = Projectile.Center.To(startpos) / 10f;
            }

            //渐入
            if (Projectile.alpha <= 0) Projectile.alpha = 0;
            else if (Projectile.alpha > 0) Projectile.alpha -= 15;

            if (mode == 0) ai1();
            if (mode == 1) ai2();
            if (mode == 2) ai3();

            base.AI();
        }

        //常态
        void ai1() {
            // 旋转
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            //if (drawcount < 10) drawcount += Main.rand.Next(10, 40);
            //Projectile.velocity = Projectile.velocity.RotatedBy(Math.Sin(drawcount / 2f) / 6f);
        }

        //钉墙
        void ai2() {
            // 旋转
            Projectile.rotation = Projectile.Center.Toz(startpos).ToRotation() - MathHelper.PiOver2;
        }

        //返回
        void ai3() {
            // 旋转
            Projectile.rotation = Projectile.Center.Toz(startpos).ToRotation() + MathHelper.PiOver2;

            if (Vector2.Distance(Projectile.Center, startpos) < 64) Projectile.Kill();
        }

        Vector2 relapos = Vector2.Zero;
        NPC sticknpc = null;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            Projectile.velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f));
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            mode = 2;
            Projectile.velocity *= 0;
            return false;
        }

        public override void OnKill(int timeLeft) {
            for (int i = 0; i < 4; i++) {
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Cloud, 0f, 0f, 10, Color.Yellow, 2f);
                Main.dust[num].velocity *= 0.5f;
                Main.dust[num].noGravity = true;
            }
            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "HarpyProj2").Value;
            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "HarpyProj2Shadow").Value;
            Texture2D texture2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "HarpyProj2BlackShadow").Value;

            Texture2D texture4 = ModContent.Request<Texture2D>(GOGConstant.Effects + "Extra_197").Value;

            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0, -8) + Projectile.velocity.SafeNormalize(Vector2.Zero) * 12f;

            Main.EntitySpriteDraw(texture2, drawPos, null, new Color(255, 255, 0) * ((255f - Projectile.alpha) / 255f), Projectile.rotation, texture2.Size() / 2, Projectile.scale * 1.2f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture1, drawPos, null, new Color(255, 255, 0, 0) * ((255f - Projectile.alpha) / 255f), Projectile.rotation, texture1.Size() / 2, Projectile.scale * 1.5f, SpriteEffects.None, 0);

            //////////////////拖尾
            if (mode == 0 || mode == 2) {
                //切回原画布
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                //注入
                List<VertexInfo2> vertices3 = new List<VertexInfo2>();
                for (int i = 0; i < Projectile.oldPos.Length; i++) {
                    if (Projectile.oldPos[i] != Vector2.Zero) {
                        vertices3.Add(new VertexInfo2(Projectile.oldPos[i] - Main.screenPosition + new Vector2(3, 1) + new Vector2((30 - 1 - i) / 4, 0).RotatedBy(Projectile.oldRot[i] - MathHelper.ToRadians(180)),
                        new Vector3(i / 8f, 0, 1 - (i / 8f)), new Color(255, 255, 0) * (1 - 0.12f * i)));
                        vertices3.Add(new VertexInfo2(Projectile.oldPos[i] - Main.screenPosition + new Vector2(3, 1) + new Vector2((30 - 1 - i) / 4, 0).RotatedBy(Projectile.oldRot[i] - MathHelper.ToRadians(10)),
                        new Vector3(i / 8f, 1, 1 - (i / 8f)), new Color(255, 255, 0) * (1 - 0.12f * i)));
                    }
                }

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

                if (vertices3.Count >= 3) {
                    for (int i = 0; i < 4; i++) {
                        Main.graphics.GraphicsDevice.Textures[0] = texture4;
                        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices3.ToArray(), 0, vertices3.Count - 2);
                    }
                }

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                //切回原画布
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                vertices3.Clear();
            }

            Main.EntitySpriteDraw(texture0, drawPos, null, lightColor * ((255f - Projectile.alpha) / 255f), Projectile.rotation, texture0.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

            return base.PreDraw(ref lightColor);
        }
    }
}
