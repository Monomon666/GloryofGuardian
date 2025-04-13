using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class ShurikenDT : GOGDT
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
        }

        Player Owner => Main.player[Projectile.owner];

        //防止破坏地图道具
        public override bool? CanCutTiles() {
            return false;
        }

        //生成时自由下坠
        public override void OnSpawn(IEntitySource source) {
            count0 = 90;//默认发射间隔
            interval = 60;//多重攻击间隔长度
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
        int frame0 = 0;
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
            NPC target1 = Projectile.Center.InDir2ClosestNPC(1600, false, true);
            if (target1 != null) {
                Attack(target1);
            }

            if (target1 == null) {
                count = 0;
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
            Vector2 projcen = Projectile.Center + new Vector2(0, -28);
            //发射
            if (count >= Gcount) {
                //普通
                if (Main.rand.Next(100) >= Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) {
                    for (int i = 0; i < 1; i++) {
                        Vector2 nowvel = (tarpos - projcen).SafeNormalize(Vector2.Zero) * 8f;
                        if (target1.boss) nowvel *= 1.2f;
                        int type = Main.rand.Next(20);
                        if (Owner.HasBuff(BuffID.WeaponImbuePoison)) type = Main.rand.Next(4);

                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item39, Projectile.Center);
                        if (type == 0) {
                            Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen, nowvel.RotatedBy(Main.rand.NextFloat(-0.01f, 0.01f)), ModContent.ProjectileType<ShurikenProj>(), lastdamage, 0, Owner.whoAmI, 1);
                            if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                                if (proj1.ModProjectile is GOGProj proj2) {
                                    proj2.OrichalcumMarkProj = true;
                                    proj2.OrichalcumMarkProjcount = 300;
                                }
                            }
                        }
                        if (type != 0) {
                            Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen, nowvel.RotatedBy(Main.rand.NextFloat(-0.01f, 0.01f)), ModContent.ProjectileType<ShurikenProj>(), lastdamage, 0, Owner.whoAmI);
                            if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                                if (proj1.ModProjectile is GOGProj proj2) {
                                    proj2.OrichalcumMarkProj = true;
                                    proj2.OrichalcumMarkProjcount = 300;
                                }
                            }
                        }
                    }
                    if (numatk == 0) {
                        frame0 += 1;
                        if (frame0 == 2) frame0 = 0;
                    }
                    if (numatk < 3) {
                        numatk += 1;
                        count -= 5;
                    }
                    if (numatk >= 3) {
                        numatk = 0;
                        count = Owner.GetModPlayer<GOGModPlayer>().GcountEx;
                    }
                }

                //过载
                if (Main.rand.Next(100) < Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) {
                    for (int i = 0; i < 1; i++) {
                        Vector2 nowvel = (tarpos - projcen).SafeNormalize(Vector2.Zero) * 8f;
                        int type = Main.rand.Next(20);
                        if (Owner.HasBuff(BuffID.WeaponImbuePoison)) type = Main.rand.Next(4);

                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item39, Projectile.Center);
                        if (type == 0) {
                            Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen, nowvel.RotatedBy(Main.rand.NextFloat(-0.01f, 0.01f)), ModContent.ProjectileType<ShurikenProj>(), lastdamage, 0, Owner.whoAmI, 1);
                            if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                                if (proj1.ModProjectile is GOGProj proj2) {
                                    proj2.OrichalcumMarkProj = true;
                                    proj2.OrichalcumMarkProjcount = 300;
                                }
                            }
                        }
                        if (type != 0) {
                            Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen, nowvel.RotatedBy(Main.rand.NextFloat(-0.01f, 0.01f)), ModContent.ProjectileType<ShurikenProj>(), lastdamage, 0, Owner.whoAmI);
                            if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                                if (proj1.ModProjectile is GOGProj proj2) {
                                    proj2.OrichalcumMarkProj = true;
                                    proj2.OrichalcumMarkProjcount = 300;
                                }
                            }
                        }
                    }

                    if (numatk == 0) {
                        frame0 += 1;
                        if (frame0 == 2) frame0 = 0;
                    }
                    if (numatk < 5) {
                        numatk += 1;
                        count -= 3;
                    }
                    if (numatk >= 5) {
                        numatk = 0;
                        count = Owner.GetModPlayer<GOGModPlayer>().GcountEx;
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
                int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Wraith, 0f, 0f, 10, Color.Black, 0.8f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].velocity *= 2f;
            }
            for (int j = 0; j < 15; j++) {
                int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Wraith, 0f, 0f, 10, Color.Black, 0.4f);
                Main.dust[num2].noGravity = true;
                Main.dust[num2].velocity *= 1f;
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            //不同朝向时翻转贴图
            SpriteEffects spriteEffects = ((wrotation % (2 * Math.PI)) > (Math.PI / 2) || (wrotation % (2 * Math.PI)) < -(Math.PI / 2)) ? SpriteEffects.FlipVertically : SpriteEffects.None;

            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "ShurikenDT").Value;
            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, 0);
            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "ShurikenDT2").Value;
            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "ShurikenDT3").Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(0, -28);
            if (frame0 == 0) Main.EntitySpriteDraw(texture, drawPosition, null, lightColor, wrotation, texture.Size() * 0.5f, Projectile.scale * 1.2f, spriteEffects, 0);
            if (frame0 == 1) Main.EntitySpriteDraw(texture1, drawPosition, null, lightColor, wrotation, texture.Size() * 0.5f, Projectile.scale * 1.2f, spriteEffects, 0);

            return false;
        }
    }
}