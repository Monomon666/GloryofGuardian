using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class ImmortalLotusProj : GOGProj
    {
        public override string Texture => GOGConstant.nulls;

        public override void SetStaticDefaults() {
            //残影机制
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults() {
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.timeLeft = 1200;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 3;//穿透数，1为攻击到第一个敌人就消失
            Projectile.tileCollide = false;

            Projectile.light = 1f;

            Projectile.scale *= 1f;
        }

        Player Owner => Main.player[Projectile.owner];
        Vector2 OwnerPos => Owner.Center;

        public override void OnSpawn(IEntitySource source) {
        }

        int count = 0;
        int mode = 0;
        int rcolor = 0;
        public override void AI() {
            SetStaticDefaults();
            count++;

            if (count > 60) {
                NPC target1 = Projectile.Center.InPosClosestNPC(1200, true, true);
                if (target1 != null && target1.active) Projectile.ChasingBehavior(target1.Center, 24f, 0);
            }

            // 夜光弹幕的AI逻辑
            if (Projectile.ai[0] == 0f) {
                // 初始化弹幕
                rcolor = 0;
                Projectile.ai[0] = 1f;
            }

            if (count > 70) rcolor += 10;
            if (rcolor > 200) rcolor = 200;

            // 弹幕的移动逻辑
            if (count == 2)
                Projectile.velocity *= 0.8f;
            Projectile.rotation = Projectile.velocity.ToRotation();

            // 弹幕的发光效果
            Lighting.AddLight(Projectile.Center, 0.5f, 0.5f, 1f); // 添加淡蓝色光效

            // 弹幕的粒子效果
            //if (Main.rand.NextBool(5)) {
            //    Dust dust = Dust.NewDustPerfect(Projectile.Center, DustID.PinkFairy, Projectile.velocity * 0.5f);
            //    dust.noGravity = true;
            //}
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnKill(int timeLeft) {
            int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Wraith, 0f, 0f, 10, Color.White, 2f);
            Main.dust[num2].noGravity = true;
            Main.dust[num2].velocity *= 10f;
        }

        int drawcount = 0;
        public override bool PreDraw(ref Color lightColor) {
            drawcount++;
            if (drawcount >= 60) drawcount = 0;
            float hue = (drawcount % 60) / 60f; // 色相在 [0, 1) 范围内循环
            Color rainbowColor = HsvToRgb(hue, 1f, 1f); // 饱和度和明度固定为 1

            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "ImmortalLotusProj1").Value;

            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition;
            float rotation = Projectile.rotation;


            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            //残影
            for (int i = 0; i < 1; i++) {
                float scale = 1f - (i / (float)Projectile.oldPos.Length);
                Vector2 drawPosition = Projectile.oldPos[i] - Main.screenPosition + Projectile.Size / 2f;
                Color color = Color.Lerp(Color.White, Color.Pink, scale) * scale * (rcolor / 100f);
                Main.spriteBatch.Draw(texture, drawPosition, null, rainbowColor, Projectile.rotation + MathHelper.PiOver2, texture.Size() * 0.5f, Projectile.scale * scale, SpriteEffects.None, 0f);
            }

            Main.spriteBatch.Draw(texture, drawPosition0, null, rainbowColor * (rcolor / 100f), rotation + MathHelper.PiOver2, texture.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0f);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            //Shader部分//

            //注入
            List<VertexInfo2> vertices = new List<VertexInfo2>();
            for (int i = 0; i < Projectile.oldPos.Length; i++) {
                if (Projectile.oldPos[i] != Vector2.Zero) {
                    vertices.Add(new VertexInfo2(Projectile.oldPos[i] - Main.screenPosition + new Vector2(12, 12) + new Vector2((30 - 1 - i) / 4, 0).RotatedBy(Projectile.oldRot[i] - MathHelper.ToRadians(-90)),
                    new Vector3(i / 8f, 0, 1 - (i / 8f)), rainbowColor * (1 - 0.12f * i)));
                    vertices.Add(new VertexInfo2(Projectile.oldPos[i] - Main.screenPosition + new Vector2(12, 12) + new Vector2((30 - 1 - i) / 4, 0).RotatedBy(Projectile.oldRot[i] - MathHelper.ToRadians(90)),
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

        //生成渐变色
        Color HsvToRgb(float h, float s, float v) {
            h = h % 1f; // 确保色相在 [0, 1) 范围内
            if (h < 0) h += 1f;

            int hi = (int)(h * 6);
            float f = h * 6 - hi;
            float p = v * (1 - s);
            float q = v * (1 - f * s);
            float t = v * (1 - (1 - f) * s);

            switch (hi) {
                case 0: return new Color(v, t, p);
                case 1: return new Color(q, v, p);
                case 2: return new Color(p, v, t);
                case 3: return new Color(p, q, v);
                case 4: return new Color(t, p, v);
                default: return new Color(v, p, q);
            }
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
    }
}
