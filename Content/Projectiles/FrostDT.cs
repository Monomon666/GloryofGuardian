using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class FrostDT : GOGDT
    {
        //不使用贴图,重写绘制
        public override string Texture => GOGConstant.nulls;
        public override void SetStaticDefaults() {
        }

        public sealed override void SetDefaults() {
            Projectile.width = 52;
            Projectile.height = 32;
            Projectile.tileCollide = true;

            Projectile.friendly = true;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.penetrate = -1;
            Projectile.scale *= 1f;
            Projectile.timeLeft = 36000;

            Projectile.scale *= 1f;

            Projectile.light = 2f;
        }

        Player Owner => Main.player[Projectile.owner];

        //防止破坏地图道具
        public override bool? CanCutTiles() {
            return false;
        }

        //生成时自由下坠
        public override void OnSpawn(IEntitySource source) {
            count0 = 45;//默认发射间隔
            Projectile.velocity = new Vector2(0, 8);
            base.OnSpawn(source);
        }

        int count = 0;
        int count0 = 0;
        int countdust = 0;
        //重力
        bool drop = true;
        //数据读取
        int Gcount = 0;
        int lastdamage = 0;
        Projectile projcenter = null;
        //
        int frostburnnum = 0;
        bool frost = false;
        public override void AI() {
            countdust++;

            count++;
            Projectile.timeLeft = 2;
            Projectile.StickToTiles(false, false);//形成判定
            Drop();
            Calculate();

            float breath = (float)Math.Sin(drawcount / 8f);

            if (countdust % 1 == 0) {
                for (int j = 0; j < 1; j++) {
                    int num1 = Dust.NewDust(Projectile.Center + new Vector2(12, -56) + new Vector2(0, -1) * breath + new Vector2(-28, -20), 32, 16, DustID.SnowflakeIce, 0f, 0f, 10, Color.White, 1f);
                    Main.dust[num1].noGravity = true;
                    Main.dust[num1].velocity *= 1f;
                }
            }

            //仆从判定
            frostburnnum = 0;
            for (int i = 0; i < Main.maxProjectiles; i++) {
                Projectile p = Main.projectile[i];
                if (p.active) {//安全性检测
                    if (p.type == ModContent.ProjectileType<SRFrostDT>()) {//判戍卫弹幕
                        if (Vector2.Distance(Projectile.Center, p.Center) < 1600) {//判距离
                            frostburnnum++;
                        }
                    }
                }
            }

            //雪地特判
            frost = false;
            if (Owner.ZoneSnow || frostburnnum > 0) {
                frost = true;
                if (countdust % 1 == 0) {
                    for (int j = 0; j < 1; j++) {
                        int num1 = Dust.NewDust(Projectile.Center + new Vector2(-24, -72), 42, 24, DustID.SnowSpray, 0f, 0f, 10, Color.White, 0.4f);
                        Main.dust[num1].noGravity = false;
                        Main.dust[num1].velocity = new Vector2(0, 1f) * Main.rand.NextFloat(1, 1.5f);
                    }
                }
            }

            //索敌与行动
            NPC target1 = Projectile.Center.InPosClosestNPC(800, true, true);
            if (target1 != null) {
                Attack(target1);
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
                        Projectile.Bottom = (droppos + new Vector2(0, y - 2) * 16);
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
            Vector2 tarpos = target1.Center;
            int snow = 0;
            if (!frost) snow = 0;
            if (frost) snow = 25;

            if (count >= Gcount) {
                if (Main.rand.Next(100) >= Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1] + snow) {
                    for (int i = 0; i < 1; i++) {
                        float breath = (float)Math.Sin(drawcount / 18f);
                        Vector2 projcen = Projectile.Center + new Vector2(0, -76) + new Vector2(0, -1) * breath;
                        Vector2 velfire = (tarpos - projcen).SafeNormalize(Vector2.Zero) * 6f;

                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);

                        Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen, velfire, ModContent.ProjectileType<FrostProj>(), lastdamage, 2, Owner.whoAmI);
                        if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                            if (proj1.ModProjectile is GOGProj proj2) {
                                proj2.OrichalcumMarkProj = true;
                                proj2.OrichalcumMarkProjcount = 300;
                            }
                        }
                    }

                    if (projcenter != null) {
                        Vector2 p1 = Projectile.Center + new Vector2(0, -32);
                        Vector2 p2 = projcenter.Center + new Vector2(0, -8);

                        for (int j = 0; j < 128; j++) {
                            int num1 = Dust.NewDust(p1 + (p2 - p1) * Main.rand.NextFloat(1), 2, 2, DustID.Flare, 0f, 0f, 10, Color.White, 2f);
                            Main.dust[num1].noGravity = true;
                            Main.dust[num1].velocity *= 0;
                        }
                    }

                    //计时重置
                    count = Owner.GetModPlayer<GOGModPlayer>().GcountEx;
                }
                if (Main.rand.Next(100) < Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1] + snow) {
                    for (int i = 0; i < 1; i++) {
                        float breath = (float)Math.Sin(drawcount / 18f);
                        Vector2 projcen = Projectile.Center + new Vector2(0, -76) + new Vector2(0, -1) * breath;
                        Vector2 velfire = (tarpos - projcen).SafeNormalize(Vector2.Zero) * 12f;

                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
                        int crit = (Main.rand.Next(100) >= (Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1])) ? 0 : 1;

                        Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen, velfire, ModContent.ProjectileType<FrostProj>(), lastdamage, 2, Owner.whoAmI, 1);
                        if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                            if (proj1.ModProjectile is GOGProj proj2) {
                                proj2.OrichalcumMarkProj = true;
                                proj2.OrichalcumMarkProjcount = 300;
                            }
                        }
                    }

                    if (projcenter != null) {
                        Vector2 p1 = Projectile.Center + new Vector2(0, -32);
                        Vector2 p2 = projcenter.Center + new Vector2(0, -8);

                        for (int j = 0; j < 128; j++) {
                            int num1 = Dust.NewDust(p1 + (p2 - p1) * Main.rand.NextFloat(1), 2, 2, DustID.Flare, 0f, 0f, 10, Color.White, 2f);
                            Main.dust[num1].noGravity = true;
                            Main.dust[num1].velocity *= 0;
                        }
                    }

                    //计时重置
                    count = Owner.GetModPlayer<GOGModPlayer>().GcountEx;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor) {
            return lightColor;
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
        Vector2 drawPosition1 = new Vector2(0, 0);
        public override bool PreDraw(ref Color lightColor) {
            drawcount++;
            float breath = (float)Math.Sin(drawcount / 8f);

            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FrostDT").Value;
            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FrostDT2").Value;
            Texture2D texture2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "Frost2DT").Value;

            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, -14);
            drawPosition1 = Projectile.Center - Main.screenPosition + new Vector2(12, -56) + new Vector2(0, -1) * breath;

            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture1, drawPosition1, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}