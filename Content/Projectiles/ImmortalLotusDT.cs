using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class ImmortalLotusDT : GOGDT
    {
        //不使用贴图,重写绘制
        public override string Texture => GOGConstant.nulls;
        public override void SetStaticDefaults() {
            Main.projFrames[Projectile.type] = 6;//动图帧数
        }

        public sealed override void SetDefaults() {
            Projectile.width = 23;
            Projectile.height = 32;
            Projectile.tileCollide = true;

            Projectile.friendly = true;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.penetrate = -1;
            Projectile.scale *= 1f;
            Projectile.timeLeft = 36000;

            Projectile.scale *= 1f;
        }

        Player Owner => Main.player[Projectile.owner];

        //防止破坏地图道具
        public override bool? CanCutTiles() {
            return false;
        }

        //生成时自由下坠
        public override void OnSpawn(IEntitySource source) {
            count0 = 60;//默认发射间隔
            count00 = 60;//多重攻击间隔

            interval = 600;//恢复弹间隔长度
            countleft = -120;//修整时长

            Projectile.velocity = new Vector2(0, 8);
            base.OnSpawn(source);
        }

        int interval = 0;
        int countin = 0;
        int count = 0;
        int count0 = 0;
        int count00 = 0;
        int countleft = 0;
        public int countp = 0;
        public bool earliest = false;
        //重力
        bool drop = true;
        //数据读取
        int Gcount = 0;
        int lastdamage = 0;
        //状态机
        int allnum = 0;
        int mode = 0;
        int dtnum = 0;

        int drawcircount = 0;
        //帧
        int frame1 = 0;
        int frame2 = 0;
        //位置标定
        Vector2 firepos = Vector2.Zero;
        public override void AI() {
            count++;
            countin++;

            countp = count;
            Projectile.timeLeft = 2;
            Projectile.StickToTiles(false, false);//形成判定
            Drop();
            Calculate();

            int dtnum0 = 0;
            int index = 0;//定义序号为0
            foreach (var proj in Main.projectile)//遍历所有弹幕
            {
                if (proj.active //活跃状态
                    && proj.ModProjectile is GOGDT gogdt1//遍历
                    && proj.type == ModContent.ProjectileType<ImmortalLotusDT>() //同类
                    && Vector2.Distance(Projectile.Center, proj.Center) < 3000
                    ) {

                    //编号
                    //序列号
                    if (gogdt1.globalcount < globalcount) {
                        //检测是否有比自己先生成的炮塔,如果有说明自己的序号要后移一位
                        index++;
                        earliest = false;
                    }

                    //调率同步

                    //总数
                    dtnum0++;
                }
            }

            dtnum = dtnum0;

            mode = 0;
            if (dtnum >= 3) mode = 1;
            if (dtnum >= 6) mode = 2;

            //水晶位置标定
            firepos = Projectile.Center + new Vector2(2, -96) + new Vector2(0, -8) * breath;

            if (mode >= 1) {
                for (int j = 0; j < 1; j++) {
                    int num1 = Dust.NewDust(Projectile.Center + new Vector2(-24, -72), 48, 48, DustID.SnowSpray, 0f, 0f, 10, Color.White, 0.4f);
                    Main.dust[num1].noGravity = false;
                    Main.dust[num1].velocity = new Vector2(0, -1f) * Main.rand.NextFloat(1, 2f);
                }
            }

            //索敌与行动
            NPC target1 = Projectile.Center.InPosClosestNPC(3000, true, true);
            if (target1 != null && target1.active) Attack(target1);
            if ((target1 == null || !target1.active) && count < 180) count = 0;

            //治愈
            if (countin >= interval) {
                for (int i = 0; i < 1; i++) {
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);

                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), firepos, new Vector2(0, -12f), ModContent.ProjectileType<ImmortalLotusProj>(), lastdamage, 2, Owner.whoAmI, 120);
                    if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                        if (proj1.ModProjectile is GOGProj proj2) {
                            proj2.OrichalcumMarkProj = true;
                            proj2.OrichalcumMarkProjcount = 300;
                        }
                    }
                }
                //计时重置
                countin = 0;
            }

            //帧图
            Projectile.frameCounter++;
            frame1 = (Projectile.frameCounter / 4) % 6;//要手动填，不然会出错
            frame2 = (Projectile.frameCounter / 8) % 4;//要手动填，不然会出错

            base.AI();
        }

        /// <summary>
        /// 坠落
        /// </summary>
        void Drop() {
            Projectile.velocity.Y += 0.2f;
            if (Projectile.velocity.Y > 8f) {
                Projectile.velocity.Y = 8f;
            }

            Vector2 droppos = Projectile.Bottom;
            if (drop) {
                int maxdropdis = 5000;
                for (int y = 0; y < maxdropdis; y++) {
                    Tile tile0 = TileHelper.GetTile(GOGUtils.WEPosToTilePos(droppos + new Vector2(0, y) * 16));
                    if (tile0.HasTile) {
                        Projectile.Bottom = (droppos + new Vector2(0, y - 6) * 16);
                        break;
                    }
                }
                drop = false;
            }
        }

        /// <summary>
        /// 重新计算和赋值参数
        /// </summary>
        void Calculate() {
            Gcount = (int)(count0 * Owner.GetModPlayer<GOGModPlayer>().GcountR * Projectile.ai[0]);//攻击间隔因子重新提取
            //伤害修正
            int newDamage = Projectile.originalDamage;
            float rangedOffset = Owner.GetTotalDamage(GuardianDamageClass.Instance).ApplyTo(100) / 100f;
            lastdamage = (int)(newDamage * rangedOffset);
        }

        /// <summary>
        /// 监测与攻击
        /// </summary>
        void Attack(NPC target1) {
            Vector2 projcen = firepos;
            Vector2 vel = projcen.Toz(target1.Center) * 12f;
            int firenum = Math.Min(6, dtnum);

            //发射,不过载
            if (count >= Gcount) {
                if (count == Gcount) {
                    for (int i = 0; i < firenum; i++) {
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);

                        Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), firepos, new Vector2(0, -12f).RotatedBy(MathHelper.Pi * 2 / firenum * i), ModContent.ProjectileType<ImmortalLotusProj>(), lastdamage, 2, Owner.whoAmI);
                        if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                            if (proj1.ModProjectile is GOGProj proj2) {
                                proj2.OrichalcumMarkProj = true;
                                proj2.OrichalcumMarkProjcount = 300;
                            }
                        }

                        if (mode == 0) count = Owner.GetModPlayer<GOGModPlayer>().GcountEx;
                    }
                }

                if (count == Gcount + count00) {
                    for (int i = 0; i < 3; i++) {
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);

                        Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), firepos, new Vector2(0, -12f).RotatedBy(MathHelper.Pi * 2 / 3 * i), ModContent.ProjectileType<ImmortalLotusProj>(), lastdamage, 2, Owner.whoAmI, 1);
                        if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                            if (proj1.ModProjectile is GOGProj proj2) {
                                proj2.OrichalcumMarkProj = true;
                                proj2.OrichalcumMarkProjcount = 300;
                            }
                        }
                    }

                    if (mode == 1) count = Owner.GetModPlayer<GOGModPlayer>().GcountEx + countleft;
                }

                if (count == Gcount + count00) {
                    for (int i = 0; i < 3; i++) {
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);

                        Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), firepos, new Vector2(0, -12f).RotatedBy(MathHelper.Pi * 2 / 3 * i), ModContent.ProjectileType<ImmortalLotusProj>(), lastdamage, 2, Owner.whoAmI, 2);
                        if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                            if (proj1.ModProjectile is GOGProj proj2) {
                                proj2.OrichalcumMarkProj = true;
                                proj2.OrichalcumMarkProjcount = 300;
                            }
                        }
                    }

                    //计时重置
                    count = Owner.GetModPlayer<GOGModPlayer>().GcountEx + countleft;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
            fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            //Projectile.velocity *= 0;
            return false;
        }


        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            return false;
        }

        public override void OnKill(int timeLeft) {
            //爆炸粒子
            for (int j = 0; j < 15; j++) {
                int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Wraith, 0f, 0f, 10, Color.White, 0.8f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].velocity *= 2f;
            }
            for (int j = 0; j < 15; j++) {
                int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Wraith, 0f, 0f, 10, Color.White, 0.4f);
                Main.dust[num2].noGravity = true;
                Main.dust[num2].velocity *= 1f;
            }
        }

        public int drawcountp = 0;
        int drawcount = 0;
        float breath = 0;
        public override bool PreDraw(ref Color lightColor) {
            breath = (float)Math.Sin((int)Main.GameUpdateCount / 18f);
            drawcount = (int)Main.GameUpdateCount;

            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "ImmortalLotusDT").Value;
            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "ImmortalLotusDT2").Value;
            Texture2D texture2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "ImmortalLotusProj01").Value;
            Texture2D texture3 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "ImmortalLotusProj02").Value;

            Vector2 drawPosition1 = Projectile.Center - Main.screenPosition + new Vector2(38, -8) + new Vector2(0, -8) * breath;
            int singleFrameY = texture0.Height / 4;
            Vector2 drawPos1 = Projectile.Center - Main.screenPosition + new Vector2(0, -46);

            Main.spriteBatch.Draw(
                texture0,
                drawPos1,
                new Rectangle(0, singleFrameY * frame2, texture0.Width, singleFrameY),//动图读帧
                lightColor * ((255f - Projectile.alpha) / 255f),
                Projectile.rotation,
                new Vector2(23, 62),
                Projectile.scale,
                SpriteEffects.None,
                0);

            if (mode < 2) drawcircount = 0;
            if (mode >= 2) {
                if (drawcircount < 99) drawcircount++;

                //Main.spriteBatch.End();
                //Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
                //
                //Main.EntitySpriteDraw(
                //        texture2,
                //        Projectile.Center - Main.screenPosition + new Vector2(0, -96),
                //        null,
                //        lightColor * 0.8f * (drawcircount / 100f),
                //        Projectile.rotation + (int)Main.GameUpdateCount / 60f,
                //        texture2.Size() / 2,
                //        Projectile.scale * 0.4f,
                //        SpriteEffects.None,
                //        0);
                //
                //Main.spriteBatch.End();
                //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                float rs = drawcount / 30f;
                List<VertexInfo2> vertices = new List<VertexInfo2>();
                float FlipRotation = 0f;
                float rotcount = drawcount / 30f;

                if (0 == 0) {
                    //Vector2 v1 = new Vector2(-300, -300).RotatedBy(rotcount);
                    //vertices.Add(new VertexInfo2(Drwapos1(v1, FlipRotation), new Vector3(0, 0, 1), Color.White * (drawcircount / 100f)));
                    ////记录纹理左上角的顶点坐标
                    //v1 = new Vector2(300, -300).RotatedBy(rotcount);
                    //vertices.Add(new VertexInfo2(Drwapos1(v1, FlipRotation), new Vector3(1, 0, 1), Color.White * (drawcircount / 100f)));
                    ////记录纹理右上角的顶点坐标
                    //v1 = new Vector2(-300, 300).RotatedBy(rotcount);
                    //vertices.Add(new VertexInfo2(Drwapos1(v1, FlipRotation), new Vector3(0, 1, 1), Color.White * (drawcircount / 100f)));
                    ////记录纹理左下角的顶点坐标
                    //v1 = new Vector2(300, 300).RotatedBy(rotcount);
                    //vertices.Add(new VertexInfo2(Drwapos1(v1, FlipRotation), new Vector3(1, 1, 1), Color.White * (drawcircount / 100f)));
                    //
                    //Main.spriteBatch.End();
                    //Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
                    //if (vertices.Count >= 3) {
                    //    for (int j = 0; j < 3; j++) {
                    //        Main.graphics.GraphicsDevice.Textures[0] = texture2;
                    //        Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
                    //    }
                    //}
                    //vertices.Clear();
                    //Main.spriteBatch.End();
                    //Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

                    //大环
                    Vector2 v2 = new Vector2(-300, -300).RotatedBy(rotcount * 2);
                    vertices.Add(new VertexInfo2(Drwapos2(v2, FlipRotation), new Vector3(0, 0, 1), Color.White * (drawcircount / 100f)));
                    //记录纹理左上角的顶点坐标
                    v2 = new Vector2(300, -300).RotatedBy(rotcount * 2);
                    vertices.Add(new VertexInfo2(Drwapos2(v2, FlipRotation), new Vector3(1, 0, 1), Color.White * (drawcircount / 100f)));
                    //记录纹理右上角的顶点坐标
                    v2 = new Vector2(-300, 300).RotatedBy(rotcount * 2);
                    vertices.Add(new VertexInfo2(Drwapos2(v2, FlipRotation), new Vector3(0, 1, 1), Color.White * (drawcircount / 100f)));
                    //记录纹理左下角的顶点坐标
                    v2 = new Vector2(300, 300).RotatedBy(rotcount * 2);
                    vertices.Add(new VertexInfo2(Drwapos2(v2, FlipRotation), new Vector3(1, 1, 1), Color.White * (drawcircount / 100f)));

                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.NonPremultiplied, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
                    if (vertices.Count >= 3) {
                        for (int j = 0; j < 3; j++) {
                            Main.graphics.GraphicsDevice.Textures[0] = texture2;
                            Main.graphics.GraphicsDevice.DrawUserPrimitives(PrimitiveType.TriangleStrip, vertices.ToArray(), 0, vertices.Count - 2);
                        }
                    }
                    vertices.Clear();
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                }
            }

            //宝石绘制
            int singleFrameY2 = texture1.Height / (Main.projFrames[Type]);
            Vector2 drawPos2 = Projectile.Center - Main.screenPosition + new Vector2(0, -52) + new Vector2(0, -8) * breath;

            Lighting.AddLight(firepos, 255 * 0.01f, 255 * 0.01f, 255 * 0.01f);

            Main.spriteBatch.Draw(
                texture1,
                drawPos2,
                new Rectangle(0, singleFrameY2 * frame1, texture1.Width, singleFrameY2),//动图读帧
                lightColor * ((255f - Projectile.alpha) / 255f),
                Projectile.rotation,
                new Vector2(23, 62),
                Projectile.scale,
                SpriteEffects.None,
                0);

            return false;
        }

        Vector2 Drwapos1(Vector2 v1, float FlipRotation) {
            //return Projectile.Center - Main.screenPosition + new Vector2(0, -64) + new Vector2(v1.X / 4, v1.Y / 12).RotatedBy(FlipRotation);
            return Projectile.Center - Main.screenPosition + new Vector2(0, -80) + new Vector2(0, -6) * breath + new Vector2(v1.X / 8, v1.Y / 24).RotatedBy(FlipRotation);
        }

        Vector2 Drwapos2(Vector2 v1, float FlipRotation) {
            return Projectile.Center - Main.screenPosition + new Vector2(0, -64) + new Vector2(0, -12) * breath + new Vector2(v1.X / 6, v1.Y / 16).RotatedBy(FlipRotation);
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