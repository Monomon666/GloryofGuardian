using GloryofGuardian.Common;
using GloryofGuardian.Content.Items.Accessories;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class MythrilDT : GOGDT
    {
        //不使用贴图,重写绘制
        public override string Texture => GOGConstant.nulls;
        public override void SetStaticDefaults() {
        }

        public sealed override void SetDefaults() {
            Projectile.width = 50;
            Projectile.height = 46;
            Projectile.tileCollide = true;

            Projectile.friendly = true;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.penetrate = -1;
            Projectile.scale *= 1f;
            Projectile.timeLeft = 1200;
            Projectile.light = 3f;

            Projectile.scale *= 1f;
        }

        Player Owner => Main.player[Projectile.owner];

        //防止破坏地图道具
        public override bool? CanCutTiles() {
            return false;
        }

        //生成时自由下坠
        public override void OnSpawn(IEntitySource source) {
            count0 = 120;//默认发射间隔
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
        public override void AI() {
            count++;
            Projectile.timeLeft = 2;
            Projectile.StickToTiles(false, false);//形成判定
            Drop();
            Calculate();
            //索敌与行动
            NPC target1 = (Projectile.Center + new Vector2(-6, -32)).InPosClosestNPC(800, false, true);
            if (target1 != null) {
                Attack(target1);
                Turn(target1);
                Turn(target1);
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
            float accessory = 1;
            //获取来自专属饰品的加成
            if (Owner.GetModPlayer<GOGModPlayer>().reversehookarrow)
            {
                accessory = 0.8f;
            }

            Gcount = (int)(count0 * Owner.GetModPlayer<GOGModPlayer>().GcountR * Projectile.ai[0] * accessory);//攻击间隔因子重新提取
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
            Vector2 projcen = Projectile.Center + new Vector2(-6, -24);

            int acccrit = 0;
            //获取来自专属饰品的加成
            if (Owner.GetModPlayer<GOGModPlayer>().reversehookarrow)
            {
                acccrit = 20;
                if(target1.boss) acccrit = 30;
            }

            //发射
            if (count >= Gcount) {
                for (int i = 0; i < 1; i++) {
                    Vector2 nowvel = new Vector2((float)Math.Cos(wrotation), (float)Math.Sin(wrotation));
                    float rotby = Main.rand.NextFloat(0.05f, 0.08f);

                    //普通
                    if (Main.rand.Next(100) >= Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1] + acccrit) {

                        Terraria.Audio.SoundEngine.PlaySound(SoundID.DD2_BallistaTowerShot, Projectile.Center);
                        if (count == Gcount) {
                            if (Main.rand.NextBool(5)) {
                                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen + new Vector2(0, 0) + nowvel * 52f, nowvel * 36f, ModContent.ProjectileType<MythrilProj2>(), lastdamage, 8, Owner.whoAmI);
                                if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                                    if (proj1.ModProjectile is GOGProj proj2) {
                                        proj2.OrichalcumMarkProj = true;
                                        proj2.OrichalcumMarkProjcount = 300;
                                    }
                                }
                            } else {
                                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen + new Vector2(0, 0) + nowvel * 52f, nowvel * 36f, ModContent.ProjectileType<MythrilProj>(), lastdamage, 8, Owner.whoAmI);
                                if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                                    if (proj1.ModProjectile is GOGProj proj2) {
                                        proj2.OrichalcumMarkProj = true;
                                        proj2.OrichalcumMarkProjcount = 300;
                                    }
                                }
                            }

                        }
                    }

                    //过载
                    if (Main.rand.Next(100) < Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1] + acccrit) {

                        Terraria.Audio.SoundEngine.PlaySound(SoundID.DD2_BallistaTowerShot, Projectile.Center);
                        if (count == Gcount + 2) {
                            if (Main.rand.NextBool(3)) {
                                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen + new Vector2(0, 0) + nowvel * 52f, nowvel.RotatedBy(-rotby) * 24f, ModContent.ProjectileType<MythrilProj2>(), lastdamage, 0, Owner.whoAmI);
                                if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                                    if (proj1.ModProjectile is GOGProj proj2) {
                                        proj2.OrichalcumMarkProj = true;
                                        proj2.OrichalcumMarkProjcount = 300;
                                    }
                                }
                                if (proj1.ModProjectile is GOGProj proj3)
                                {
                                    if (Owner.GetModPlayer<GOGModPlayer>().reversehookarrow) proj3.ReverseHookArrow = true;
                                }
                            } else {
                                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen + new Vector2(0, 0) + nowvel * 52f, nowvel.RotatedBy(-rotby) * 24f, ModContent.ProjectileType<MythrilProj>(), lastdamage, 0, Owner.whoAmI);
                                if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                                    if (proj1.ModProjectile is GOGProj proj2) {
                                        proj2.OrichalcumMarkProj = true;
                                        proj2.OrichalcumMarkProjcount = 300;
                                    }
                                }
                                if (proj1.ModProjectile is GOGProj proj3)
                                {
                                    if (Owner.GetModPlayer<GOGModPlayer>().reversehookarrow) proj3.ReverseHookArrow = true;
                                }
                            }
                        }
                        if (count == Gcount + 4) {
                            if (Main.rand.NextBool(3)) {
                                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen + new Vector2(0, 2) + nowvel * 52f, nowvel * 24f, ModContent.ProjectileType<MythrilProj2>(), lastdamage / 2, 0, Owner.whoAmI);
                                if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                                    if (proj1.ModProjectile is GOGProj proj2) {
                                        proj2.OrichalcumMarkProj = true;
                                        proj2.OrichalcumMarkProjcount = 300;
                                    }
                                }
                            } else {
                                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen + new Vector2(0, 2) + nowvel * 52f, nowvel * 24f, ModContent.ProjectileType<MythrilProj>(), lastdamage / 2, 0, Owner.whoAmI);
                                if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                                    if (proj1.ModProjectile is GOGProj proj2) {
                                        proj2.OrichalcumMarkProj = true;
                                        proj2.OrichalcumMarkProjcount = 300;
                                    }
                                }
                            }
                        }
                        if (count == Gcount + 6) {
                            if (Main.rand.NextBool(3)) {
                                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen + new Vector2(0, 4) + nowvel * 52f, nowvel.RotatedBy(rotby) * 24f, ModContent.ProjectileType<MythrilProj>(), lastdamage / 3, 0, Owner.whoAmI);
                                if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                                    if (proj1.ModProjectile is GOGProj proj2) {
                                        proj2.OrichalcumMarkProj = true;
                                        proj2.OrichalcumMarkProjcount = 300;
                                    }
                                }
                            } else {
                                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen + new Vector2(0, 4) + nowvel * 52f, nowvel.RotatedBy(rotby) * 24f, ModContent.ProjectileType<MythrilProj>(), lastdamage / 3, 0, Owner.whoAmI);
                                if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                                    if (proj1.ModProjectile is GOGProj proj2) {
                                        proj2.OrichalcumMarkProj = true;
                                        proj2.OrichalcumMarkProjcount = 300;
                                    }
                                }
                            }
                        }
                    }
                }

                //计时重置,通过更改这个值来重置攻击
                if (count >= Gcount + 8) count = Owner.GetModPlayer<GOGModPlayer>().GcountEx;
            }
        }

        /// <summary>
        /// 炮台旋转
        /// </summary>
        void Turn(NPC target1) {
            Vector2 tarpos = target1.Center;
            Vector2 projcen = Projectile.Center + new Vector2(-6, -24);

            Vector2 vector2 = (tarpos - projcen).SafeNormalize(Vector2.Zero) * Projectile.spriteDirection;
            float rot2 = vector2.ToRotation();
            float degree2 = (float)((180 / Math.PI) * rot2);
            float tarrot = MathHelper.ToRadians(projRot + degree2 * Projectile.spriteDirection);
            float rspeed = 0.06f;

            //转头
            if (wrotation != tarrot) {
                if (Math.Abs(wrotation - tarrot) % Math.PI <= rspeed) {
                    wrotation = tarrot;
                                       //
                    return;
                } else {
                    Vector2 clockwise = (wrotation + rspeed).ToRotationVector2();
                    Vector2 anticlockwise = (wrotation - rspeed).ToRotationVector2();
                                                                                     //
                    if ((clockwise - (tarpos - projcen).SafeNormalize(Vector2.Zero)).Length() <= (anticlockwise - (tarpos - projcen).SafeNormalize(Vector2.Zero)).Length())
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

        public override bool PreDraw(ref Color lightColor) {
            //不同朝向时翻转贴图
            SpriteEffects spriteEffects = ((wrotation % (2 * Math.PI)) > (Math.PI / 2) || (wrotation % (2 * Math.PI)) < -(Math.PI / 2)) ? SpriteEffects.FlipVertically : SpriteEffects.None;

            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "MythrilDT").Value;
            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, 2);
            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "MythrilDT2").Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(0, -20);
            if (spriteEffects == SpriteEffects.None) drawPosition += new Vector2(0, 0);
            if (spriteEffects == SpriteEffects.FlipVertically) drawPosition += new Vector2(-16, 0);
            Main.EntitySpriteDraw(texture, drawPosition, null, lightColor, wrotation + 0.05f, texture.Size() * 0.5f + new Vector2(0, 0), Projectile.scale * 1.2f, spriteEffects, 0);

            return false;
        }
    }
}