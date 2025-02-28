using GloryofGuardian.Common;
using GloryofGuardian.Content.Buffs;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class OrichalcumDT : GOGDT
    {
        //不使用贴图,重写绘制
        public override string Texture => GOGConstant.nulls;
        public override void SetStaticDefaults() {
        }

        public sealed override void SetDefaults() {
            Projectile.width = 36;
            Projectile.height = 46;
            Projectile.tileCollide = true;

            Projectile.friendly = true;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.penetrate = -1;
            Projectile.scale *= 1f;
            Projectile.timeLeft = 36000;

            Projectile.scale *= 1f;

            //本体具有伤害的炮塔，需要设置无敌帧
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 45;
            Projectile.light = 3f;
        }

        Player Owner => Main.player[Projectile.owner];

        //防止破坏地图道具
        public override bool? CanCutTiles() {
            return false;
        }

        //生成时自由下坠
        public override void OnSpawn(IEntitySource source) {
            count0 = 60;//默认发射间隔
            Projectile.velocity = new Vector2(0, 8);
            base.OnSpawn(source);
        }

        int count = 0;
        int count0 = 0;
        int adpcount = 0;
        int adpronum = 0;
        //重力
        bool drop = true;
        //数据读取
        int Gcount = 0;
        int Acount = 0;
        int lastdamage = 0;
        public override void AI() {
            count++;
            adpcount++;
            Projectile.timeLeft = 2;
            Projectile.StickToTiles(false, false);//形成判定
            Drop();
            Calculate();
            //常态下即攻击
            Attack();
            if (adpcount % 60 == 0) {
                adpronum = 2;
            }

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
            int newDamage = (int)(Projectile.originalDamage);
            float rangedOffset = Owner.GetTotalDamage(GuardianDamageClass.Instance).ApplyTo(100) / 100f;
            lastdamage = (int)(newDamage * rangedOffset);
        }

        Projectile p1 = null;
        /// <summary>
        /// 监测与攻击
        /// </summary>
        void Attack() {
            Vector2 projcen = Projectile.Center + new Vector2(0, -12);

            p1 = null;
            float mindistance = 540 * ringscale;
            float mindistance2 = 540 * ringscale;

            for (int i = 0; i < 1000; i++) {
                Projectile p = Main.projectile[i];
                if (p.active && p.type != ModContent.ProjectileType<OrichalcumDT>()
                    && Projectile.type != ModContent.ProjectileType<SlimeProj0>() //不加成史莱姆地雷
                    ) {//安全性检测,不判自己
                    if (p.ModProjectile is GOGDT) {//判炮台
                        if (Vector2.Distance(Projectile.Center, p.Center) < mindistance) {//判距离
                             mindistance = Vector2.Distance(Projectile.Center, p.Center);
                             p1 = p;
                        }else {
                             continue;
                        }
                    }
                }
            }

            if (p1 != null && p1.ModProjectile is GOGDT proj) {
                proj.OrichalcumMarkDT = true;
                proj.OrichalcumMarkDT2 = true;
            }

            if (adpronum > 0) {
                for (int i = 0; i < 1000; i++) {
                    Projectile p = Main.projectile[i];
                    if (p.active) {//安全性检测
                        if (p.ModProjectile is GOGProj && p.damage > 0) {//判戍卫弹幕
                            if (Vector2.Distance(Projectile.Center, p.Center) < mindistance2) {//判距离
                                if (p.ModProjectile is GOGProj proj3) {
                                    proj3.OrichalcumMarkProj = true;
                                    proj3.OrichalcumMarkProjcount = 300;
                                    adpronum -= 1;
                                }
                            } else {
                                continue;
                            }
                        }
                    }
                }
            }

            //计时重置
            count = Owner.GetModPlayer<GOGModPlayer>().GcountEx;
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
                int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Honey, 0f, 0f, 10, Color.White, 0.8f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].velocity *= 2f;
            }
            for (int j = 0; j < 15; j++) {
                int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Honey, 0f, 0f, 10, Color.White, 0.4f);
                Main.dust[num2].noGravity = true;
                Main.dust[num2].velocity *= 1f;
            }
        }

        //浮动因子
        int floatcount = 0;
        float float1 = 0;
        float float2 = 0;
        float float3 = 0;
        float ringscale = 0;
        float ringlastscale = 0;

        Vector2 DTcenter = Vector2.Zero;
        public override bool PreDraw(ref Color lightColor) {
            floatcount++;
            float1 = (float)Math.Sin(floatcount / 12f) + 1;
            float2 = (float)Math.Sin(floatcount / 12f + MathHelper.Pi) + 1;
            float3 = (float)Math.Sin(floatcount / 12f + MathHelper.PiOver2) + 1;

            if (floatcount == 0) ringscale = 0.02f;
            if (floatcount >= 0 && floatcount <= 120) ringscale += 0.003f;
            if (floatcount >= 360 && ringscale < 0.4f) ringscale += 0.0001f;

            ringlastscale = 0.3f;

            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "OrichalcumDT").Value;
            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "OrichalcumDT2").Value;
            Texture2D texture2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "OrichalcumDT3").Value;
            Texture2D texture3 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "OrichalcumDT4").Value;
            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture1, drawPosition0 + new Vector2(-4, -16) + new Vector2(0, -2 * float2), null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture2, drawPosition0 + new Vector2(6, -48) + new Vector2(0, -2 * float1), null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            if (p1 != null) {
                for (int i = 0; i <= 4; i++) {
                    Dust dust1 = Dust.NewDustDirect(Projectile.Center + new Vector2(-10, -72) + new Vector2(0, -2 * float1), texture0.Width / 2, texture0.Height / 2, DustID.UndergroundHallowedEnemies, 1f, 1f, 100, Color.Purple, 0.8f);
                    dust1.velocity = new Vector2(0, -2);
                    dust1.noGravity = true;

                    Dust dust2 = Dust.NewDustDirect(Projectile.Center + new Vector2(-6, -72) + new Vector2(0, -2 * float1), (int)(texture0.Width * 0.25f), (int)(texture0.Height * 0.25f), DustID.UndergroundHallowedEnemies, 1f, 1f, 100, Color.Purple, 0.8f);
                    dust2.velocity = new Vector2(0, -4);
                    dust2.noGravity = true;
                }
            } 

            {  
                Main.EntitySpriteDraw(texture2, drawPosition0 + new Vector2(36, -20) + new Vector2(0, -4 * float2), null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale * 0.5f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(texture2, drawPosition0 + new Vector2(-32, -20) + new Vector2(0, -4 * float3), null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale * 0.5f, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(texture3, drawPosition0 + new Vector2(-8, -16) + new Vector2(0, -4 * float3), null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            Texture2D textures = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "GradientRing3000").Value;
            Texture2D textures2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "GradientRing3000P").Value;
            DTcenter = drawPosition0 + new Vector2(-0, -48) + new Vector2(0, -4 * float1);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            Main.EntitySpriteDraw(textures, DTcenter, null, new Color(163, 22, 158) * 0.8f, Projectile.rotation, textures.Size() * 0.5f, Projectile.scale * ringscale * ringlastscale * (float1 * 0.01f + 1), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(textures2, DTcenter, null, new Color(163, 22, 158) * 0.7f, Projectile.rotation, textures2.Size() * 0.5f, Projectile.scale * ringscale * ringlastscale * (float1 * 0.005f + 1), SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            //强化附带球
            if (p1 != null) {
                Vector2 ballpos = (p1.Center - Main.screenPosition) + new Vector2(4, -64) + new Vector2(0, -2 * float1);

                Main.EntitySpriteDraw(texture2, ballpos, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale * 0.5f, SpriteEffects.None, 0);

                for (int i = 0; i <= 4; i++) {
                    Dust dust1 = Dust.NewDustDirect(ballpos + Main.screenPosition - texture0.Size() * 0.25f, (int)(texture0.Width * 0.25f), (int)(texture0.Height * 0.25f), DustID.UndergroundHallowedEnemies, 1f, 1f, 100, Color.Purple, 0.8f);
                    dust1.velocity = new Vector2(0, -3);
                    dust1.noGravity = true;
                }
            }

            return false;
        }
    }
}