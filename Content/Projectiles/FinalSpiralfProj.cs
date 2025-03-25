using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class FinalSpiralfProj : GOGProj
    {
        public override string Texture => GOGConstant.Projectiles + Name;

        public override void SetStaticDefaults() {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults() {
            Projectile.width = 14;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.timeLeft = 1200;
            //Projectile.light = 1.0f;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 4;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 50;

            Projectile.extraUpdates = 0;
            Projectile.light = 1.5f;
            Projectile.scale *= 1f;

            Projectile.tileCollide = false;
        }

        Player Owner => Main.player[Projectile.owner];
        Vector2 OwnerPos => Owner.Center;
        Vector2 ToMou => Main.MouseWorld - OwnerPos;

        public override void OnSpawn(IEntitySource source) {
        }

        int countsin = 0;
        int count = 0;
        int mode = 0;
        Vector2 orivel = Vector2.Zero;
        Vector2 oldpos = Vector2.Zero;
        public override void AI() {
            countsin++;
            count++;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }


        public override bool ShouldUpdatePosition() {
            return true;//禁止速度影响弹幕位置
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnKill(int timeLeft) {
            if (Projectile.timeLeft > 10) Terraria.Audio.SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
        }

        int drawcount = 0;
        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FinalSpiralfProj").Value;

            drawcount++;
            if (drawcount >= 10) drawcount = 0;
            float t = (drawcount % 10) / 10f; // t在 [0, 1) 范围内循环
            Color rainbowColor = WhiteToRedGradient(t); // 生成渐变色

            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition;
            float rotation = Projectile.rotation;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            //残影
            for (int i = 0; i < 1; i++) {
                float scale = 1f - (i / (float)Projectile.oldPos.Length);
                Vector2 drawPosition = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;
                Main.spriteBatch.Draw(texture, drawPosition, null, rainbowColor, Projectile.rotation + -MathHelper.PiOver2, texture.Size() * 0.5f, Projectile.scale * scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(texture, drawPosition0, null, rainbowColor, rotation + -MathHelper.PiOver2, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            //Shader部分//

            //注入
            List<VertexInfo2> vertices = new List<VertexInfo2>();
            for (int i = 0; i < Projectile.oldPos.Length; i++) {
                if (Projectile.oldPos[i] != Vector2.Zero) {
                    vertices.Add(new VertexInfo2(Projectile.oldPos[i] - Main.screenPosition + new Vector2(9, 9) + new Vector2((30 - 1 - i) / 4, 0).RotatedBy(Projectile.oldRot[i] - MathHelper.ToRadians(-90)),
                    new Vector3(i / 8f, 0, 1 - (i / 8f)), rainbowColor * (1 - 0.12f * i)));
                    vertices.Add(new VertexInfo2(Projectile.oldPos[i] - Main.screenPosition + new Vector2(9, 9) + new Vector2((30 - 1 - i) / 4, 0).RotatedBy(Projectile.oldRot[i] - MathHelper.ToRadians(90)),
                    new Vector3(i / 8f, 1, 1 - (i / 8f)), rainbowColor * (1 - 0.12f * i)));
                }

                int num1 = Dust.NewDust(Projectile.oldPos[i] - Main.screenPosition + new Vector2(30 - i, 0).RotatedBy(Projectile.oldRot[i] - MathHelper.ToRadians(-90)), 0, 0, DustID.Wraith, 0f, 0f, 10, Color.White, 0.8f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].velocity *= 50f;
            }

            //绘制
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp,
            DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            Main.graphics.GraphicsDevice.Textures[0] = ModContent.Request<Texture2D>(GOGConstant.Effects + "Extra_197").Value;

            if (vertices.Count >= 3) {
                for (int i = 0; i < 4; i++) {
                    Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
                }
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
            DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }

        //Shader结构体
        public struct VertexInfo2 : IVertexType
        {
            private static VertexDeclaration _vertexDeclaration = new VertexDeclaration(new VertexElement[3]
            {
            new VertexElement(0,VertexElementFormat.Vector2,VertexElementUsage.Position,0),
            new VertexElement(8,VertexElementFormat.Color,VertexElementUsage.Color,0),
            new VertexElement(12,VertexElementFormat.Vector3,VertexElementUsage.TextureCoordinate,0)
            });
            public Vector2 Position;
            public Color Color;
            public Vector3 TexCoord;
            public VertexInfo2(Vector2 position, Vector3 texCoord, Color color) {
                Position = position;
                TexCoord = texCoord;
                Color = color;
            }
            public VertexDeclaration VertexDeclaration {
                get => _vertexDeclaration;
            }
        }


        // 生成白色和红色之间的渐变色
        Color WhiteToRedGradient(float t) {
            t = t % 1f; // 确保t在 [0, 1) 范围内
            if (t < 0) t += 1f;

            // 在白色 (1,1,1) 和红色 (1,0,0) 之间插值
            float r = 1f;          // 红色通道始终为1
            float g = 1f - t;      // 绿色通道从1降到0
            float b = 1f - t;      // 蓝色通道从1降到0

            return new Color(r, g, b);
        }
    }
}
