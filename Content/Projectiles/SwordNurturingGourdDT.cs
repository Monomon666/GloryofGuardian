using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class SwordNurturingGourdDT : GOGDT
    {
        //不使用贴图,重写绘制
        public override string Texture => GOGConstant.nulls;
        public override void SetStaticDefaults() {
        }

        public sealed override void SetDefaults() {
            Projectile.width = 38;
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
            count0 = 30;//默认发射间隔
            interval = 180;
            base.OnSpawn(source);
        }

        int count = 0;
        int count0 = 0;
        //内置CD
        int interval = 0;
        //数据读取
        int Gcount = 0;
        int lastdamage = 0;
        Projectile projcenter = null;
        public override void AI() {
            count++;
            interval++;
            Projectile.timeLeft = 2;
            Projectile.StickToTiles(false, false);//形成判定
            Calculate();

            float breath = (float)Math.Sin(drawcount / 16f);

            if (count % 1 == 0) {
                for (int j = 0; j < 1; j++) {
                    int num1 = Dust.NewDust(Projectile.Center + new Vector2(-16, 4) + new Vector2(0, -1) * breath, 32, 16, DustID.Firework_Pink, 0f, 0f, 10, Color.Pink, 1f);
                    Main.dust[num1].scale *= Main.rand.NextFloat(0.4f, 0.8f);
                    Main.dust[num1].noGravity = true;
                    Main.dust[num1].velocity = new Vector2(0, 1f);
                    Main.dust[num1].velocity *= Main.rand.NextFloat(-0.2f, 0.8f);
                }
            }

            //索敌与行动
            NPC target1 = Projectile.Center.InPosClosestNPC(3000, true, true);
            if (target1 != null) {
                Attack(target1);
            }

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
            float breath = (float)Math.Sin(drawcount / 16f);

            Vector2 tarpos = target1.Center;
            Vector2 firepos = Projectile.Center + new Vector2(0, -12) + new Vector2(0, -4) * breath;

            if (count >= Gcount) {
                if (Main.rand.Next(100) >= Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1] || interval < 180) {
                    for (int i = 0; i < 1; i++) {
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);

                        Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), firepos, new Vector2(0, -12f).RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f)), ModContent.ProjectileType<SwordNurturingGourdProj>(), lastdamage, 2, Owner.whoAmI);
                        if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                            if (proj1.ModProjectile is GOGProj proj2) {
                                proj2.OrichalcumMarkProj = true;
                                proj2.OrichalcumMarkProjcount = 300;
                            }
                        }
                    }

                    //计时重置
                    count = Owner.GetModPlayer<GOGModPlayer>().GcountEx;
                }

                if (Main.rand.Next(100) < Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1] && interval > 180) {
                    for (int i = 0; i < 1; i++) {
                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);

                        Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), firepos + new Vector2(0, -32f), new Vector2(0, -4f), ModContent.ProjectileType<SwordNurturingGourdProj>(), lastdamage, 2, Owner.whoAmI, 1);
                        if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                            if (proj1.ModProjectile is GOGProj proj2) {
                                proj2.OrichalcumMarkProj = true;
                                proj2.OrichalcumMarkProjcount = 300;
                            }
                        }
                    }

                    //计时重置
                    count = Owner.GetModPlayer<GOGModPlayer>().GcountEx;
                    interval = 0;
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
            float breath = (float)Math.Sin(drawcount / 16f);

            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Weapons + "SwordNurturingGourdCalling").Value;
            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "SwordNurturingGourdProj0").Value;
            Texture2D texture2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "SwordNurturingGourdProj").Value;

            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, 0) + new Vector2(0, -4) * breath;

            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}