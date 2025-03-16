using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class NanoUAVDT : GOGDT
    {
        //不使用贴图,重写绘制
        public override string Texture => GOGConstant.nulls;
        public override void SetStaticDefaults() {
        }

        public sealed override void SetDefaults() {
            Projectile.width = 80;
            Projectile.height = 52;
            Projectile.tileCollide = true;

            Projectile.friendly = true;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.penetrate = -1;
            Projectile.scale *= 1f;
            Projectile.timeLeft = 36000;

            Projectile.scale *= 1.2f;

            Projectile.light = 1.5f;
        }

        Player Owner => Main.player[Projectile.owner];

        //防止破坏地图道具
        public override bool? CanCutTiles() {
            return false;
        }

        //生成时自由下坠
        public override void OnSpawn(IEntitySource source) {
            count0 = 16;//默认发射间隔
            Projectile.velocity = new Vector2(0, 0);
            wrotation = 0 + 0.2f;
            wrotation2 = -MathHelper.Pi - 0.2f;
            base.OnSpawn(source);
        }

        int count = 0;
        int count0 = 0;
        int count2 = 0;
        int countsin = 0;
        int interval = 0;
        //重力
        bool drop = true;
        //炮台转动
        float wrotation = 0;
        float projRot = 0;

        float wrotation2 = 0;
        float projRot2 = 0;
        //强化状态
        int power = 0;
        //数据读取
        int Gcount = 0;
        int lastdamage = 0;
        //重置判定
        bool firstatk = false;
        int numatk = 0;
        public override void AI() {
            count++;
            count2++;
            countsin++;
            Projectile.timeLeft = 2;
            Projectile.velocity = new Vector2(0, (float)((Math.Sin(countsin / 60f) - 0f) * 0.3f));
            Projectile.StickToTiles(false, false);//形成判定
            Calculate();

            //索敌与行动
            NPC target01 = ((Projectile.Center + new Vector2(-4, -12)).InDirClosestNPC(1200, false, true, 1));
            if (target01 != null && target01.Center.Y < (Projectile.Center + new Vector2(-4, -12)).Y) target01 = null;

            if (target01 != null) {
                Attack(target01);
            }

            NPC target02 = ((Projectile.Center + new Vector2(-4, -12)).InDirClosestNPC(1200, false, true, -1));
            if (target02 != null && target02.Center.Y < (Projectile.Center + new Vector2(-4, -12)).Y) target02 = null;

            if (target02 != null) {
                Attack2(target02);
            }

            //交叉火力
            if (target01 != null && target02 == null) {
                Attack2(target01);
            }
            if (target01 == null && target02 != null) {
                Attack(target02);
            }

            if (floatscore >= 300) power = 1;
            if (floatscore < 300) power = 0;
            if (floatscore > 620) floatscore = 620;

            base.AI();
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
            Vector2 projcen = Projectile.Center + new Vector2(-32, -12);

            //发射
            if (count2 >= Gcount) {
                floatscore += 20;

                for (int i = 0; i < 1; i++) {
                    float vel = Main.rand.NextFloat(0.9f, 1.15f) * 48f;
                    Vector2 nowvel = projcen.Toz(tarpos);

                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Item11, Projectile.Center);
                    //普通
                    if (Main.rand.Next(100) >= Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) {
                        Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen, nowvel.RotatedBy(Main.rand.NextFloat(-0.01f, 0.01f)) * vel, ModContent.ProjectileType<NanoUAVProj>(), lastdamage, 0.1f, Owner.whoAmI, 0);
                        if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                            if (proj1.ModProjectile is GOGProj proj2) {
                                proj2.OrichalcumMarkProj = true;
                                proj2.OrichalcumMarkProjcount = 300;
                            }
                        }
                    }
                    //过载
                    if (Main.rand.Next(100) < Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) {
                        Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen, nowvel.RotatedBy(Main.rand.NextFloat(-0.01f, 0.01f)) * vel * 0.5f, ModContent.ProjectileType<NanoUAVProj>(), (int)(lastdamage * 0.75f), 0.1f, Owner.whoAmI, 1);
                        if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                            if (proj1.ModProjectile is GOGProj proj2) {
                                proj2.OrichalcumMarkProj = true;
                                proj2.OrichalcumMarkProjcount = 300;
                            }
                        }
                    }
                }

                //计时重置,通过更改这个值来重置攻击,枪械不受ex缩减影响
                count2 = 0;
            }
        }
        void Attack2(NPC target2) {
            Vector2 tarpos = target2.Center;
            Vector2 projcen = Projectile.Center + new Vector2(24, -12);

            //发射
            if (count >= Gcount) {
                floatscore += 20;

                for (int i = 0; i < 1; i++) {
                    float vel = Main.rand.NextFloat(0.9f, 1.15f) * 48f;
                    Vector2 nowvel = projcen.Toz(tarpos);

                    Terraria.Audio.SoundEngine.PlaySound(SoundID.Item11, Projectile.Center);
                    //普通
                    if (Main.rand.Next(100) >= Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) {
                        Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen, nowvel.RotatedBy(Main.rand.NextFloat(-0.01f, 0.01f)) * vel, ModContent.ProjectileType<NanoUAVProj>(), lastdamage, 0.1f, Owner.whoAmI, 0);
                        if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                            if (proj1.ModProjectile is GOGProj proj2) {
                                proj2.OrichalcumMarkProj = true;
                                proj2.OrichalcumMarkProjcount = 300;
                            }
                        }
                    }
                    //过载
                    if (Main.rand.Next(100) < Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) {
                        Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen, nowvel.RotatedBy(Main.rand.NextFloat(-0.01f, 0.01f)) * vel * 0.5f, ModContent.ProjectileType<NanoUAVProj>(), (int)(lastdamage * 0.75f), 0.1f, Owner.whoAmI, 1);
                        if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                            if (proj1.ModProjectile is GOGProj proj2) {
                                proj2.OrichalcumMarkProj = true;
                                proj2.OrichalcumMarkProjcount = 300;
                            }
                        }
                    }
                }

                //计时重置,通过更改这个值来重置攻击,枪械不受ex缩减影响
                count = 0;
            }
        }

        /// <summary>
        /// 炮台旋转
        /// </summary>
        //void Turn(NPC target1) {
        //    Vector2 tarpos = target1.Center;
        //    Vector2 projcen = Projectile.Center + new Vector2(-32, -12);
        //
        //    Vector2 vector2 = (tarpos - projcen).SafeNormalize(Vector2.Zero) * Projectile.spriteDirection;
        //    float rot2 = vector2.ToRotation();
        //    float degree2 = (float)((180 / Math.PI) * rot2);
        //    float tarrot = MathHelper.ToRadians(projRot + degree2 * Projectile.spriteDirection);
        //    float rspeed = 0.1f;
        //
        //    //转头
        //    if (wrotation != tarrot) {
        //        if (Math.Abs(wrotation - tarrot) % Math.PI <= rspeed) {//如果方向差小于单次转量则本次旋转将超过目标,取余很重要
        //            wrotation = tarrot;//那么直接让方向与到目标方向防止抖动
        //                               //tarrot = wrotation;
        //            return;
        //        } else {
        //            Vector2 clockwise = (wrotation + rspeed).ToRotationVector2();//这是假设NPC顺时针转动后的单位方向向量
        //            Vector2 anticlockwise = (wrotation - rspeed).ToRotationVector2();//这是假设NPC逆时针转动后的单位方向向量
        //                                                                             //显然，要比较两个向量哪个与目标夹角更近，就是比较他们与目标向量相减后的长度
        //            if ((clockwise - (tarpos - projcen).SafeNormalize(Vector2.Zero)).Length() <= (anticlockwise - (tarpos - projcen).SafeNormalize(Vector2.Zero)).Length())//如果顺时针的差值更小
        //            {
        //                wrotation += rspeed;
        //            } else {
        //                wrotation -= rspeed;
        //            }
        //        }
        //    }
        //}
        //void Turn2(NPC target2) {
        //    Vector2 tarpos = target2.Center;
        //    Vector2 projcen = Projectile.Center + new Vector2(24, -12);
        //
        //    Vector2 vector2 = (tarpos - projcen).SafeNormalize(Vector2.Zero) * Projectile.spriteDirection;
        //    float rot2 = vector2.ToRotation();
        //    float degree2 = (float)((180 / Math.PI) * rot2);
        //    float tarrot = MathHelper.ToRadians(projRot2 + degree2 * Projectile.spriteDirection);
        //    float rspeed = 0.1f;
        //
        //    //转头
        //    if (wrotation2 != tarrot) {
        //        if (Math.Abs(wrotation2 - tarrot) % Math.PI <= rspeed) {//如果方向差小于单次转量则本次旋转将超过目标,取余很重要
        //            wrotation2 = tarrot;//那么直接让方向与到目标方向防止抖动
        //                                //tarrot = wrotation;
        //            return;
        //        } else {
        //            Vector2 clockwise = (wrotation2 + rspeed).ToRotationVector2();//这是假设NPC顺时针转动后的单位方向向量
        //            Vector2 anticlockwise = (wrotation2 - rspeed).ToRotationVector2();//这是假设NPC逆时针转动后的单位方向向量
        //                                                                              //显然，要比较两个向量哪个与目标夹角更近，就是比较他们与目标向量相减后的长度
        //            if ((clockwise - (tarpos - projcen).SafeNormalize(Vector2.Zero)).Length() <= (anticlockwise - (tarpos - projcen).SafeNormalize(Vector2.Zero)).Length())//如果顺时针的差值更小
        //            {
        //                wrotation2 += rspeed;
        //            } else {
        //                wrotation2 -= rspeed;
        //            }
        //        }
        //    }
        //}

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
        float float1 = 0;
        bool canfloat = false;
        int floatscore = 0;
        public override bool PreDraw(ref Color lightColor) {
            if (floatscore > 300) floatcount++;
            float colorsca = 0.7f;
            if (floatscore <= 300) colorsca = 0.7f;
            if (floatscore > 300) colorsca = 1f;
            float1 = (float)Math.Sin(floatcount / 48f) + 1;

            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "NanoUAVDT").Value;
            Vector2 drawPosition1 = Projectile.Center - Main.screenPosition + new Vector2(0, 0);

            Main.EntitySpriteDraw(texture1, drawPosition1, null, lightColor, Projectile.rotation, texture1.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
