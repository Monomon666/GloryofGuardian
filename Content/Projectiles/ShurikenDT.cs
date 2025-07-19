using System;
using System.Collections.Generic;
using GloryofGuardian.Common;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;
using static GloryofGuardian.Common.GOGUtils;

namespace GloryofGuardian.Content.Projectiles {
    public class ShurikenDT : GOGDT {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 40;
            Projectile.friendly = false;
            Projectile.penetrate = -1;

            OtherHeight = 46;

            count0 = 25;

            findboss = true;
            exdust = DustID.Wraith;
            frame0 = Main.rand.Next(2);

            Attackrange = 1200;
        }

        Player Owner => Main.player[Projectile.owner];
        Vector2 AttackPos2 = Vector2.Zero;

        public override void AI() {
            AttackPos = Projectile.Center + new Vector2(-3, -38);
            
            base.AI();
        }

        protected override List<Projectile> Attack1() {
            List<Projectile> projlist = new List<Projectile>();

            for (int i = 0; i < 1; i++) {
                Vector2 vel = AttackPos.Toz(target0.Center) * 18f;

                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item39, Projectile.Center);
                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), AttackPos, vel.RotatedBy(Main.rand.NextFloat(-0.6f, 0.6f)), ModContent.ProjectileType<ShurikenProj>(), lastdamage, 8, Owner.whoAmI);

                frame0 += 1;
                projlist.Add(proj1);
            }

            FinishAttack = true;
            return projlist;
        }

        protected override List<Projectile> Attack2() {
            List<Projectile> projlist = new List<Projectile>();

            for (int i = 0; i < 1; i++) {
                Vector2 vel = AttackPos.Toz(target0.Center) * 18f;

                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item39, Projectile.Center);
                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), AttackPos, vel.RotatedBy(Main.rand.NextFloat(-0.8f, 0.8f)), ModContent.ProjectileType<ShurikenProj>(), lastdamage, 8, Owner.whoAmI, 1);

                frame0 += 1;
                projlist.Add(proj1);
            }

            FinishAttack = true;
            return projlist;
        }

        int frame0 = 0;
        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "ShurikenDT").Value;
            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, -15);
            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "ShurikenDT2").Value;
            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "ShurikenDT3").Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(0, -43);
            if (frame0 >= 2) frame0 = 0;
            if (frame0 == 0) Main.EntitySpriteDraw(texture, drawPosition, null, lightColor, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 1.2f, SpriteEffects.None, 0);
            if (frame0 == 1) Main.EntitySpriteDraw(texture1, drawPosition + new Vector2(2.5f, 0), null, lightColor, Projectile.rotation, texture.Size() * 0.5f, Projectile.scale * 1.2f, SpriteEffects.None, 0);

            return false;
        }
    }

    public class ShurikenProj : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.localNPCHitCooldown = 12;
            Projectile.penetrate = 3;

            Projectile.tileCollide = false;

            throughtile = true;
        }

        Player Owner => Main.player[Projectile.owner];


        Vector2 startpos = Vector2.Zero;
        Vector2 atkpos = Vector2.Zero;
        bool dash = false;
        Vector2 dashvec = Vector2.Zero;
        Vector2 dashvel = Vector2.Zero;

        int dashover = 0;
        float dias = 0;

        int returncount = 0;
        public override void AI() {
            drawcount++;
            Projectile.rotation = Projectile.velocity.ToRotation();
            if (count == 1) startpos = Projectile.Center;
            if (target0 != null && target0.active && count == 1) atkpos = target0.Center;

            if (Projectile.ai[0] == 0) Normal();
            if (Projectile.ai[0] == 1) Shadow();

            if (target0 == null || !target0.active) {
                returncount++;
                if (!hadhit) Projectile.velocity *= 0.97f;
            }
            if (returncount > 120) hadhit = true;

            base.AI();
        }

        void Normal() {
            if (!hadhit) {
                if (target0 != null && target0.active) {
                    float dis = Vector2.Distance(Projectile.Center, atkpos);

                    Projectile.velocity *= 0.95f;
                    if (dis > 400) {
                        Projectile.velocity += Projectile.Center.Toz(atkpos) * 1.2f;
                    }
                    if (dis > 200 && dis <= 400) {
                        Projectile.velocity *= 0.95f;
                        Projectile.velocity += Projectile.Center.Toz(atkpos) * (1.2f + (400 - dis) * 0.004f);
                    }
                    if (dis <= 200) {
                        Projectile.velocity *= 0.95f;
                        Projectile.velocity += Projectile.Center.Toz(atkpos) * 2f;
                    }

                    if (dis <= 16 && !hadhit) {
                        hadhit = true;
                        Projectile.penetrate = 3;
                    }
                }
            }

            if (hadhit) {
                float dis = Vector2.Distance(Projectile.Center, startpos);

                Projectile.velocity *= 0.95f;
                if (dis > 400) {
                    Projectile.velocity += Projectile.Center.Toz(startpos) * 1.2f;
                }
                if (dis > 200 && dis <= 400) {
                    Projectile.velocity *= 0.95f;
                    Projectile.velocity += Projectile.Center.Toz(startpos) * (1.2f + (400 - dis) * 0.004f);
                }
                if (dis <= 200) {
                    Projectile.velocity *= 0.95f;
                    Projectile.velocity += Projectile.Center.Toz(startpos) * 2f;
                }

                if (dis <= 16) Projectile.Kill();
            }
        }

        void Shadow() {
            if (!hadhit && !dash) {
                if (target0 != null && target0.active) {
                    float dis = Vector2.Distance(Projectile.Center, target0.Center);

                    Projectile.velocity *= 0.95f;
                    if (dis > 600) {
                        Projectile.velocity += Projectile.Center.Toz(target0.Center) * 1.2f;
                    }
                    if (dis > 400 && dis <= 600) {
                        Projectile.velocity *= 0.95f;
                        Projectile.velocity += Projectile.Center.Toz(target0.Center) * (1.2f + (600 - dis) * 0.004f);
                    }
                    if (dis <= 400) {
                        Projectile.velocity *= 0.9f;
                        Projectile.velocity += Projectile.Center.Toz(target0.Center) * 2f;
                    }
                    if (dis <= 200 &&
                        Projectile.velocity.SafeNormalize(Vector2.Zero).ToRotation()
                        - Projectile.Center.Toz(target0.Center).ToRotation() < 0.05f) {
                        //dash
                        dash = true;
                        dashvec = Projectile.Center;
                        dashvel = Projectile.velocity;

                        Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 40;
                    }
                }
            }

            if (hadhit && !dash) {
                float dis = Vector2.Distance(Projectile.Center, startpos);

                if (dashover > 0) {
                    if (dashover == 60) dias = Main.rand.NextFloat(-0.03f, 0.03f);
                    dashover--;
                    if (dashover < 40) Projectile.velocity *= 0.9f;
                    if (dashover > 40) Projectile.velocity = Projectile.velocity.RotatedBy(dias);
                }

                if (dashover == 0) {
                    Projectile.velocity *= 0.95f;
                    if (dis > 400) {
                        Projectile.velocity += Projectile.Center.Toz(startpos) * 1.2f * 1.5f;
                    }
                    if (dis > 200 && dis <= 400) {
                        Projectile.velocity *= 0.95f;
                        Projectile.velocity += Projectile.Center.Toz(startpos) * (1.2f + (400 - dis) * 0.004f) * 1.5f;
                    }
                    if (dis <= 200) {
                        Projectile.velocity *= 0.95f;
                        Projectile.velocity += Projectile.Center.Toz(startpos) * 2f * 1.5f;
                    }
                }

                if (dis <= 16) Projectile.Kill();
            }

            //冲刺:加速穿过目标400
            if (dash) {
                float dis2 = Vector2.Distance(Projectile.Center, dashvec);

                Projectile.penetrate = -1;
                hadhit = true;

                if (dis2 >= 400) {
                    dash = false;
                    Projectile.velocity = dashvel;
                    Projectile.penetrate = 3;
                    dashover = 60;
                }
            }
        }

        bool hadhit = false;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            target.AddBuff(BuffID.Bleeding, 300);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            return base.OnTileCollide(oldVelocity);
        }

        public override void OnKill(int timeLeft) {
            //基本爆炸粒子
            for (int i = 0; i < 4; i++) {
                int num = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, DustID.Wraith, 0f, 0f, 50, Color.White, 1.5f);
                Main.dust[num].velocity *= 1f;
                Main.dust[num].noGravity = true;
                if (Main.rand.NextBool(2)) {
                    Main.dust[num].scale = 0.5f;
                    Main.dust[num].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
            for (int i = 0; i < 4; i++) {
                int num = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, DustID.CrimsonTorch, 0f, 0f, 50, Color.White, 1.5f);
                Main.dust[num].velocity *= 1f;
                if (Main.rand.NextBool(2)) {
                    Main.dust[num].scale = 0.5f;
                    Main.dust[num].noGravity = true;
                    Main.dust[num].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
        }


        int drawcount = 0;
        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "ShurikenProj").Value;
            Texture2D texture2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "ShurikenProjShadow").Value;
            Texture2D texture3 = ModContent.Request<Texture2D>(GOGConstant.Effects + "Extra_197").Value;
            Texture2D texture4 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "ShurikenProjShadow0").Value;

            Vector2 drawPos = Projectile.Center - Main.screenPosition;

            {
                //防止没有正确切回原画布
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                //////////////////黑色衬底
                List<VertexInfo2> vertices4 = new List<VertexInfo2>();

                float FlipRotation = Projectile.rotation;//倾斜
                float rotcount = drawcount / 3f;//转速
                if (hadhit) rotcount *= -1;
                //缩放比例在Drwapos2里调

                //大环
                Vector2 v2 = new Vector2(-120, -120).RotatedBy(rotcount);

                //记录纹理左上角的顶点坐标
                v2 = new Vector2(-64, -64).RotatedBy(rotcount);
                vertices4.Add(new VertexInfo2(Drwapos2(v2, FlipRotation, 1.5f), new Vector3(0, 0, 1), lightColor * 0.5f));
                //记录纹理右上角的顶点坐标
                v2 = new Vector2(64, -64).RotatedBy(rotcount);
                vertices4.Add(new VertexInfo2(Drwapos2(v2, FlipRotation, 1.5f), new Vector3(1, 0, 1), lightColor * 0.5f));
                //记录纹理左下角的顶点坐标
                v2 = new Vector2(-64, 64).RotatedBy(rotcount);
                vertices4.Add(new VertexInfo2(Drwapos2(v2, FlipRotation, 1.5f), new Vector3(0, 1, 1), lightColor * 0.5f));
                //记录纹理右下角的顶点坐标
                v2 = new Vector2(64, 64).RotatedBy(rotcount);
                vertices4.Add(new VertexInfo2(Drwapos2(v2, FlipRotation, 1.5f), new Vector3(1, 1, 1), lightColor * 0.5f));

                if (1 == 1) {
                    if (vertices4.Count >= 3) {
                        for (int j = 0; j < 2; j++) {
                            Main.graphics.GraphicsDevice.Textures[0] = texture4;
                            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices4.ToArray(), 0, vertices4.Count - 2);
                        }
                    }
                }

                vertices4.Clear();

                //////////////////红色渲染
                List<VertexInfo2> vertices = new List<VertexInfo2>();

                //大环
                v2 = new Vector2(-120, -120).RotatedBy(rotcount);

                //记录纹理左上角的顶点坐标
                v2 = new Vector2(-64, -64).RotatedBy(rotcount);
                vertices.Add(new VertexInfo2(Drwapos2(v2, FlipRotation, 1.5f), new Vector3(0, 0, 1), new Color(255, 255, 255, 0)));
                //记录纹理右上角的顶点坐标
                v2 = new Vector2(64, -64).RotatedBy(rotcount);
                vertices.Add(new VertexInfo2(Drwapos2(v2, FlipRotation, 1.5f), new Vector3(1, 0, 1), new Color(255, 255, 255, 0)));
                //记录纹理左下角的顶点坐标
                v2 = new Vector2(-64, 64).RotatedBy(rotcount);
                vertices.Add(new VertexInfo2(Drwapos2(v2, FlipRotation, 1.5f), new Vector3(0, 1, 1), new Color(255, 255, 255, 0)));
                //记录纹理右下角的顶点坐标
                v2 = new Vector2(64, 64).RotatedBy(rotcount);
                vertices.Add(new VertexInfo2(Drwapos2(v2, FlipRotation, 1.5f), new Vector3(1, 1, 1), new Color(255, 255, 255, 0)));

                if (1 == 1) {
                    if (vertices.Count >= 3) {
                        for (int j = 0; j < 2; j++) {
                            Main.graphics.GraphicsDevice.Textures[0] = texture2;
                            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
                        }
                    }
                }

                vertices.Clear();

                //////////////////贴图
                List<VertexInfo2> vertices2 = new List<VertexInfo2>();

                //记录纹理左上角的顶点坐标
                v2 = new Vector2(-64, -64).RotatedBy(rotcount);
                vertices2.Add(new VertexInfo2(Drwapos2(v2, FlipRotation, 1), new Vector3(0, 0, 1), Color.White));
                //记录纹理右上角的顶点坐标
                v2 = new Vector2(64, -64).RotatedBy(rotcount);
                vertices2.Add(new VertexInfo2(Drwapos2(v2, FlipRotation, 1), new Vector3(1, 0, 1), Color.White));
                //记录纹理左下角的顶点坐标
                v2 = new Vector2(-64, 64).RotatedBy(rotcount);
                vertices2.Add(new VertexInfo2(Drwapos2(v2, FlipRotation, 1), new Vector3(0, 1, 1), Color.White));
                //记录纹理右下角的顶点坐标
                v2 = new Vector2(64, 64).RotatedBy(rotcount);
                vertices2.Add(new VertexInfo2(Drwapos2(v2, FlipRotation, 1), new Vector3(1, 1, 1), Color.White));

                if (1 == 1) {
                    if (vertices2.Count >= 3) {
                        for (int j = 0; j < 3; j++) {
                            Main.graphics.GraphicsDevice.Textures[0] = texture;
                            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices2.ToArray(), 0, vertices2.Count - 2);
                        }
                    }
                }

                vertices2.Clear();
            }

            //////////////////拖尾
            {
                //注入
                List<VertexInfo2> vertices3 = new List<VertexInfo2>();
                for (int i = 0; i < Projectile.oldPos.Length; i++) {
                    if (Projectile.oldPos[i] != Vector2.Zero) {
                        vertices3.Add(new VertexInfo2(Projectile.oldPos[i] - Main.screenPosition + new Vector2(18, 16) + new Vector2((30 - 1 - i) / 4, 0).RotatedBy(Projectile.oldRot[i] - MathHelper.ToRadians(-90)),
                        new Vector3(i / 8f, 0, 1 - (i / 8f)), new Color(148, 0, 0) * (1 - 0.12f * i)));
                        vertices3.Add(new VertexInfo2(Projectile.oldPos[i] - Main.screenPosition + new Vector2(18, 16) + new Vector2((30 - 1 - i) / 4, 0).RotatedBy(Projectile.oldRot[i] - MathHelper.ToRadians(90)),
                        new Vector3(i / 8f, 1, 1 - (i / 8f)), new Color(148, 0, 0) * (1 - 0.12f * i)));
                    }
                }

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

                if (vertices3.Count >= 3) {
                    for (int i = 0; i < 4; i++) {
                        Main.graphics.GraphicsDevice.Textures[0] = texture3;
                        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices3.ToArray(), 0, vertices3.Count - 2);
                    }
                }

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);


                ////绘制
                //Main.spriteBatch.End();
                //Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicClamp,
                //DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);
                //
                //if (vertices3.Count >= 3) {
                //    for (int i = 0; i < 4; i++) {
                //        Main.graphics.GraphicsDevice.Textures[0] = texture3;
                //        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices3.ToArray(), 0, vertices3.Count - 2);
                //    }
                //}
                //
                //Main.spriteBatch.End();
                //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
                //DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.TransformationMatrix);

                //切回原画布
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                vertices3.Clear();
            }

            return false;
        }

        //扁平化旋转的顶点绘制
        Vector2 Drwapos2(Vector2 v1, float FlipRotation, float scale) {
            return Projectile.Center - Main.screenPosition + new Vector2(scale * v1.X / 4f, scale * v1.Y / 7f).RotatedBy(FlipRotation);
        }
    }
}
