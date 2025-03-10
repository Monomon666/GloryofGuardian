using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class PaleClockworkGunDT : GOGDT
    {
        //不使用贴图,重写绘制
        public override string Texture => GOGConstant.nulls;
        public override void SetStaticDefaults() {
        }

        public sealed override void SetDefaults() {
            Projectile.width = 76;
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
            count0 = 30;//默认发射间隔
            interval = 30;
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
        int numatk = 0;
        public override void AI() {
            count++;
            Projectile.timeLeft = 2;
            Projectile.StickToTiles(false, false);//形成判定
            Drop();
            Calculate();
            //索敌与行动
            NPC target1;
            target1 = ((Projectile.Center + new Vector2(0, 6)).InDir2ClosestNPC(800, false, true, 1));
            if (target1 == null) target1 = ((Projectile.Center + new Vector2(-4, -48)).InDir2ClosestNPC(800, false, true, 1));
            if (target1 != null) {
                Attack(target1);
                Turn(target1);
            }

            if (target1 == null) {
                count = 0;
                interval = Gcount;
                firstatk = false;
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
            int newDamage = Projectile.originalDamage;
            float rangedOffset = Owner.GetTotalDamage(GuardianDamageClass.Instance).ApplyTo(100) / 100f;
            lastdamage = (int)(newDamage * rangedOffset);
        }

        /// <summary>
        /// 监测与攻击
        /// </summary>
        void Attack(NPC target1) {
            Vector2 tarpos = target1.Center + new Vector2(0, 4);
            Vector2 projcen = Projectile.Center + new Vector2(-4, -48);

            //发射
            if (count >= interval) {
                //普通
                if (Main.rand.Next(100) >= Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) {
                    for (int i = 0; i < 1; i++) {
                        float vel = Main.rand.NextFloat(0.99f, 1.01f) * 24f;
                        Vector2 nowvel = new Vector2((float)Math.Cos(wrotation), (float)Math.Sin(wrotation));

                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item11, Projectile.Center);
                        Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen + new Vector2(0, 2) + nowvel * 44f, nowvel.RotatedBy(Main.rand.NextFloat(-0.02f, 0.02f)) * vel, ModContent.ProjectileType<RustyGunProj>(), lastdamage, 2, Owner.whoAmI, 0, 0, 1);
                        if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                            if (proj1.ModProjectile is GOGProj proj2) {
                                proj2.OrichalcumMarkProj = true;
                                proj2.OrichalcumMarkProjcount = 300;
                            }
                        }
                    }
                    bmove = true;

                    //计时重置,通过更改这个值来重置攻击
                    if (interval > 12) interval -= 1;
                    count = Main.rand.Next(6);
                }

                //过载
                if (Main.rand.Next(100) < Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) {
                    for (int i = 0; i < 1; i++) {
                        float vel = Main.rand.NextFloat(0.99f, 1.01f) * 24f;
                        Vector2 nowvel = new Vector2((float)Math.Cos(wrotation), (float)Math.Sin(wrotation));

                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item11, Projectile.Center);
                        Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen + new Vector2(0, 2) + nowvel * 44f, nowvel.RotatedBy(Main.rand.NextFloat(-0.03f, 0.03f)) * vel, ModContent.ProjectileType<RustyGunProj>(), lastdamage, 2, Owner.whoAmI);
                        if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                            if (proj1.ModProjectile is GOGProj proj2) {
                                proj2.OrichalcumMarkProj = true;
                                proj2.OrichalcumMarkProjcount = 300;
                            }
                        }

                        if (interval > 12) interval -= 1;
                    }
                    bmove = true;

                    if (numatk < 3) {
                        numatk += 1;
                        count -= 4;
                    }
                    if (numatk >= 3) {
                        numatk = 0;
                        count = Main.rand.Next(12);
                    }
                }
            }

            ////发射
            //if (count >= interval) {
            //    //普通
            //    if (Main.rand.Next(100) >= Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) {
            //        for (int i = 0; i < 1; i++) {
            //            float vel = Main.rand.NextFloat(0.9f, 1.15f) * 24f;
            //            Vector2 nowvel = new Vector2((float)Math.Cos(wrotation), (float)Math.Sin(wrotation));
            //
            //            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item11, Projectile.Center);
            //            Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen + new Vector2(0, -50) + nowvel * 42f, nowvel.RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * vel, ModContent.ProjectileType<RustyGunProj>(), lastdamage, 0, Owner.whoAmI, 0, 0, 1);
            //            if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
            //                if (proj1.ModProjectile is GOGProj proj2) {
            //                    proj2.OrichalcumMarkProj = true;
            //                    proj2.OrichalcumMarkProjcount = 300;
            //                }
            //            }
            //            bmove = true;
            //        }
            //
            //        if (numatk < 3) {
            //            numatk += 1;
            //            count -= 4;
            //        }
            //        if (numatk >= 3) {
            //            numatk = 0;
            //            count = 0;
            //        }
            //    }
            //
            //    //过载
            //    if (Main.rand.Next(100) < Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) {
            //        for (int i = 0; i < 1; i++) {
            //            float vel = Main.rand.NextFloat(0.9f, 1.15f) * 24f;
            //            Vector2 nowvel = new Vector2((float)Math.Cos(wrotation), (float)Math.Sin(wrotation));
            //
            //            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item11, Projectile.Center);
            //            Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen + new Vector2(0, -50) + nowvel * 42f, nowvel.RotatedBy(Main.rand.NextFloat(-0.05f, 0.05f)) * vel, ModContent.ProjectileType<RustyGunProj>(), lastdamage, 1, Owner.whoAmI);
            //            if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
            //                if (proj1.ModProjectile is GOGProj proj2) {
            //                    proj2.OrichalcumMarkProj = true;
            //                    proj2.OrichalcumMarkProjcount = 300;
            //                }
            //            }
            //            bmove = true;
            //
            //            if (interval > 12) interval -= 1;
            //        }
            //
            //        if (numatk < 12) {
            //            numatk += 1;
            //            count -= 4;
            //        }
            //        if (numatk >= 12) {
            //            numatk = 0;
            //            count = 0;
            //        }
            //    }
            //}
        }

        /// <summary>
        /// 炮台旋转
        /// </summary>
        void Turn(NPC target1) {
            Vector2 tarpos = target1.Center;
            Vector2 projcen = Projectile.Center + new Vector2(0, -24);

            Vector2 vector2 = (tarpos - projcen).SafeNormalize(Vector2.Zero) * Projectile.spriteDirection;
            float rot2 = vector2.ToRotation();
            float degree2 = (float)((180 / Math.PI) * rot2);
            float tarrot = MathHelper.ToRadians(projRot + degree2 * Projectile.spriteDirection);
            float rspeed = 0.06f;

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

        public override bool OnTileCollide(Vector2 oldVelocity) {
            //Projectile.velocity *= 0;
            return false;
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
            fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
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

        //浮动因子
        int floatcount = 0;
        bool bmove = false;
        Vector2 fmove = new Vector2(0, 0);
        public override bool PreDraw(ref Color lightColor) {
            //不同朝向时翻转贴图
            SpriteEffects spriteEffects = ((wrotation % (2 * Math.PI)) > (Math.PI / 2) || (wrotation % (2 * Math.PI)) < -(Math.PI / 2)) ? SpriteEffects.FlipVertically : SpriteEffects.None;

            if (bmove) {
                floatcount++;
                if (spriteEffects == SpriteEffects.None) fmove = new Vector2(Main.rand.NextFloat(-2f, 2f), Main.rand.NextFloat(-2f, 2f));
                if (spriteEffects != SpriteEffects.None) fmove = new Vector2(Main.rand.NextFloat(-0.5f, 0.5f), Main.rand.NextFloat(-0.5f, 0.5f));
            }

            if (floatcount > 18) {
                floatcount = 0;
                bmove = false;
                fmove = new Vector2(0, 0);
            }

            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PaleGunDT").Value;
            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, -8);
            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PaleClockworkGunDT2").Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(-4, -46);
            //if (spriteEffects == SpriteEffects.None) drawPosition += new Vector2(0, -8);
            Vector2 fix = new Vector2(0, 0);
            if (spriteEffects == SpriteEffects.None) fix = new Vector2(-8, 0);
            if (spriteEffects != SpriteEffects.None) fix = new Vector2(-4, 2);
            Main.EntitySpriteDraw(texture, drawPosition + fmove, null, lightColor, wrotation, texture.Size() * 0.5f + new Vector2(-4, -0) + fix + fmove, Projectile.scale * 1.2f, spriteEffects, 0);

            return false;
        }
    }
}