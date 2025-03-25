using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class FinalSpiralfDT : GOGDT
    {
        //不使用贴图,重写绘制
        public override string Texture => GOGConstant.nulls;
        public override void SetStaticDefaults() {
        }

        public sealed override void SetDefaults() {
            Projectile.width = 132;
            Projectile.height = 70;
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
            count0 = 12;//默认发射间隔
            interval = 5;//多重攻击间隔长度
            Projectile.velocity = new Vector2(0, 8);
            base.OnSpawn(source);
        }

        int count = 0;
        int count0 = 0;
        int interval = 0;
        //重力
        bool drop = true;
        //炮台转动
        float wrotation = 0;
        float projRot = 0;
        //数据读取
        int Gcount = 0;
        int lastdamage = 0;
        //重置判定
        bool firstatk = false;
        //帧
        int frame1 = 0;
        int frame2 = 0;
        //状态机
        int mode = 0;
        public override void AI() {
            count++;
            Projectile.timeLeft = 2;
            Projectile.StickToTiles(false, false);//形成判定
            Drop();
            Calculate();
            //索敌与行动
            NPC target1 = Projectile.Center.InPosClosestNPC(3000, true, true);
            if (target1 != null) {
                if (mode == 0) {
                    Attack(target1);
                }

                Turn(target1);
                Turn(target1);
            }

            //帧图
            Projectile.frameCounter++;
            frame1 = (int)MathHelper.Min(Projectile.frameCounter / 8, 8);
            //if (frame1 <= 15) frame2 = (Projectile.frameCounter / 8) % 17;


            //frame1 = (Projectile.frameCounter / 15) % 8;//要手动填，不然会出错
            //frame2 = (Projectile.frameCounter / 8) % 17;//要手动填，不然会出错

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
            Vector2 tarpos = target1.Center + new Vector2(0, target1.height);
            Vector2 projcen = firepos;
            //发射
            if (count + interval >= Gcount && !firstatk) {
                //普通
                if (Main.rand.Next(100) >= Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) {
                    for (int i = 0; i < 1; i++) {
                        Vector2 nowvel = new Vector2((float)Math.Cos(wrotation), (float)Math.Sin(wrotation));

                        Terraria.Audio.SoundEngine.PlaySound(SoundID.DD2_BallistaTowerShot, Projectile.Center);
                        Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen + nowvel * 68f, nowvel * 48f, ModContent.ProjectileType<FinalSpiralfProj>(), lastdamage, 8, Owner.whoAmI);
                        if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                            if (proj1.ModProjectile is GOGProj proj2) {
                                proj2.OrichalcumMarkProj = true;
                                proj2.OrichalcumMarkProjcount = 300;
                            }
                        }
                    }
                }

                if (Main.rand.Next(100) < Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) {
                    for (int i = 0; i < 1; i++) {
                        Vector2 nowvel = new Vector2((float)Math.Cos(wrotation), (float)Math.Sin(wrotation));

                        Terraria.Audio.SoundEngine.PlaySound(SoundID.DD2_BallistaTowerShot, Projectile.Center);
                        Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen + nowvel * 68f, nowvel * 48f, ModContent.ProjectileType<FinalSpiralfProj>(), lastdamage, 8, Owner.whoAmI, 1);
                        if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                            if (proj1.ModProjectile is GOGProj proj2) {
                                proj2.OrichalcumMarkProj = true;
                                proj2.OrichalcumMarkProjcount = 300;
                            }
                        }
                    }
                }

                //前置攻击完成
                firstatk = true;
            }

            //发射
            if (count >= Gcount) {

                //计时重置,通过更改这个值来重置攻击
                count = Owner.GetModPlayer<GOGModPlayer>().GcountEx;
                firstatk = false;
            }
        }

        /// <summary>
        /// 炮台旋转
        /// </summary>
        void Turn(NPC target1) {
            Vector2 tarpos = target1.Center + new Vector2(0, target1.height / 3);
            Vector2 projcen = firepos;

            Vector2 vector2 = (tarpos - projcen).SafeNormalize(Vector2.Zero) * Projectile.spriteDirection;
            float rot2 = vector2.ToRotation();
            float degree2 = (float)((180 / Math.PI) * rot2);
            float tarrot = MathHelper.ToRadians(projRot + degree2 * Projectile.spriteDirection);
            float rspeed = 0.04f;

            //转头
            if (wrotation != tarrot) {
                if (Math.Abs(wrotation - tarrot) % Math.PI <= rspeed) {//如果方向差小于单次转量则本次旋转将超过目标,取余很重要
                    wrotation = tarrot;//那么直接让方向与到目标方向防止抖动
                                       //tarrot = wrotation;
                    return;
                } else {
                    Vector2 clockwise = (wrotation + rspeed).ToRotationVector2();//这是假设NPC顺时针转动后的单位方向向量
                    Vector2 anticlockwise = (wrotation - rspeed).ToRotationVector2();//这是假设NPC逆时针转动后的单位方向向量
                                                                                     //显然，要比较两个向量哪个与目标夹角更近，就是比较他们与目标向量相减后的长度
                    if ((clockwise - (tarpos - projcen).SafeNormalize(Vector2.Zero)).Length() <= (anticlockwise - (tarpos - projcen).SafeNormalize(Vector2.Zero)).Length())//如果顺时针的差值更小
                    {
                        wrotation += rspeed;
                    } else {
                        wrotation -= rspeed;
                    }
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

        int drawcount = 0;
        Vector2 firepos = Vector2.Zero;
        public override bool PreDraw(ref Color lightColor) {
            drawcount++;
            if (drawcount < 30 || drawcount >= 60) drawcount++;
            if (drawcount >= 80) drawcount = 10;
            float t = (drawcount % 90) / 90f; // t在 [0, 1) 范围内循环
            Color RedColor = WhiteToRedGradient(t); // 生成渐变色

            //不同朝向时翻转贴图
            SpriteEffects spriteEffects = ((wrotation % (2 * Math.PI)) > (Math.PI / 2) || (wrotation % (2 * Math.PI)) < -(Math.PI / 2)) ? SpriteEffects.FlipVertically : SpriteEffects.None;

            Texture2D texture01 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FinalSpiralfDT").Value;
            Texture2D texture012 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FinalSpiralfDTG").Value;
            Texture2D texture013 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FinalSpiralfDTG2").Value;
            Texture2D texture02 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FinalSpiralfDT2").Value;
            Texture2D texture022 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FinalSpiralfDT2G").Value;
            Texture2D texture023 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FinalSpiralfDT2G2").Value;
            Texture2D texture024 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FinalSpiralfDT2G3").Value;

            Texture2D texture4 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FinalSpiralfProj").Value;

            Vector2 drawPosition1 = Projectile.Center - Main.screenPosition;

            int singleFrameY = texture01.Height / 9;
            Vector2 drawPos1 = Projectile.Center - Main.screenPosition + new Vector2(98, 36);

            Main.spriteBatch.Draw(
                texture01,
                drawPos1,
                new Rectangle(0, singleFrameY * frame1, texture01.Width, singleFrameY),//动图读帧
                lightColor,
                Projectile.rotation,
                new Vector2(132, 70),
                Projectile.scale * 1.5f,
                SpriteEffects.None,
                0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            for (int i = 0; i < 2; i++) {
                if (Projectile.frameCounter > 64) {
                    Main.spriteBatch.Draw(
                    texture013,
                    drawPos1,
                    null,
                    RedColor,
                    Projectile.rotation,
                    texture013.Size(),
                    Projectile.scale * 1.5f,
                    SpriteEffects.None,
                    0);
                }
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);

            int singleFrameY2 = texture02.Height / 18;
            Vector2 drawPos2 = Projectile.Center - Main.screenPosition + new Vector2(-12, 48);

            firepos = Projectile.Center + new Vector2(-5, -19);
            Lighting.AddLight(firepos, 255 * 0.05f, 20 * 0.05f, 20 * 0.05f);
            Lighting.AddLight(firepos, 255 * 0.01f, 255 * 0.01f, 255 * 0.01f);

            SpriteEffects spriteEffects2 = ((wrotation % (2 * Math.PI)) > (Math.PI / 2) || (wrotation % (2 * Math.PI)) < -(Math.PI / 2)) ? SpriteEffects.FlipVertically : SpriteEffects.None;
            if (spriteEffects2 == SpriteEffects.FlipVertically) drawPos2 += new Vector2(6, 0);

            Main.spriteBatch.Draw(
                texture02,
                drawPos2 + new Vector2(10, -68),
                new Rectangle(0, singleFrameY2 * frame2, texture02.Width, singleFrameY2),//动图读帧
                lightColor * ((255f - Projectile.alpha) / 255f),
                wrotation,
                new Vector2(28, 18),
                Projectile.scale * 1.5f,
                spriteEffects2,
                0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            for (int i = 0; i < 2; i++) {
                if (Projectile.frameCounter > 64) {
                    Main.spriteBatch.Draw(
                        texture02,
                        drawPos2 + new Vector2(10, -68),
                        new Rectangle(0, singleFrameY2 * frame2, texture02.Width, singleFrameY2),//动图读帧
                        RedColor * 0.4f,
                        wrotation,
                        new Vector2(28, 18),
                        Projectile.scale * 1.5f,
                        spriteEffects2,
                        0);
                }
            }

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, RasterizerState.CullCounterClockwise, null, Main.GameViewMatrix.TransformationMatrix);


            return false;
        }

        // 生成白色和红色之间的渐变色
        Color WhiteToRedGradient(float t) {
            t = t % 1f; // 确保t在 [0, 1) 范围内
            if (t < 0) t += 1f;

            if (t < 0.333f) {
                // 阶段1：黑色(0,0,0) → 红色(1,0,0)
                float r = t * 3f;    // 红色通道从0升到1
                return new Color(r, 0f, 0f);
            } else if (t < 0.666f) {
                // 阶段2：红色(1,0,0) → 白色(1,1,1)
                float gb = (t - 0.333f) * 3f; // 绿蓝通道从0升到1
                return new Color(1f, gb, gb);
            } else {
                // 阶段3：白色(1,1,1) → 黑色(0,0,0)
                float rgb = 1f - (t - 0.666f) * 3f; // 所有通道从1降到0
                return new Color(rgb, rgb, rgb);
            }
        }
    }
}