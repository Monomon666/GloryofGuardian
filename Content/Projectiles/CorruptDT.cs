using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class CorruptDT : GOGDT
    {
        public override string Texture => GOGConstant.nulls;
        public override void SetStaticDefaults() {
        }

        public sealed override void SetDefaults() {
            Projectile.width = 28;
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
            count0 = 120;//默认发射间隔
            Projectile.velocity = new Vector2(0, 8);
            base.OnSpawn(source);
        }

        int count = 0;
        int count0 = 0;
        //重力
        bool drop = true;
        //数据读取
        int Gcount = 0;
        int lastdamage = 0;
        public override void AI() {
            count++;
            Projectile.timeLeft = 2;
            Projectile.StickToTiles(false, false);//形成判定
            Drop();
            Calculate();
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
            Vector2 tarpos = target1.Center + new Vector2(Main.rand.Next(-16, 16), Main.rand.Next(-16, 16));
            Vector2 projcen = Projectile.Center + new Vector2(0, -32);

            //发射
            if (count >= count0) {
                //普通
                if (Main.rand.Next(100) >= Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) {

                    for (int i = 0; i < 100; i++) {
                        Vector2 perpos = projcen + (tarpos + new Vector2(0, -270) - projcen) * i / 100f;
                        int num = Dust.NewDust(new Vector2(perpos.X, perpos.Y), 1, 1, DustID.Wraith, 0f, 0f, 50, Color.White, 0.8f);
                        Main.dust[num].velocity *= 0f;
                        Main.dust[num].noGravity = true;
                    }

                    for (int i = 0; i < 1; i++) {
                        float vel = Main.rand.NextFloat(0.9f, 1.15f) * 24f;

                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);
                        Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), tarpos + new Vector2(0, -240), Vector2.Zero, ModContent.ProjectileType<CorruptCloudProj>(), lastdamage, 1, Owner.whoAmI, 0);
                        if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                            if (proj1.ModProjectile is GOGProj proj2) {
                                proj2.OrichalcumMarkProj = true;
                                proj2.OrichalcumMarkProjcount = 300;
                            }
                        }
                    }
                }

                //过载
                if (Main.rand.Next(100) < Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) {

                    for (int i = 0; i < 100; i++) {
                        Vector2 perpos = projcen + (tarpos + new Vector2(0, -270) - projcen) * i / 100f;
                        int num = Dust.NewDust(new Vector2(perpos.X, perpos.Y), 1, 1, DustID.Wraith, 0f, 0f, 50, Color.White, 0.8f);
                        Main.dust[num].velocity *= 0f;
                        Main.dust[num].noGravity = true;
                    }

                    for (int i = 0; i < 1; i++) {
                        float vel = Main.rand.NextFloat(0.9f, 1.15f) * 24f;

                        Terraria.Audio.SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);
                        Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), tarpos + new Vector2(0, -240), Vector2.Zero, ModContent.ProjectileType<CorruptCloudProj>(), lastdamage, 1, Owner.whoAmI, 1);
                        if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                            if (proj1.ModProjectile is GOGProj proj2) {
                                proj2.OrichalcumMarkProj = true;
                                proj2.OrichalcumMarkProjcount = 300;
                            }
                        }
                    }
                }

                count = Main.rand.Next(0, 15);
            }
        }

        static double NormalizeAngleRad(double angleRad) {
            // 使用Math.IEEERemainder得到-π到π之间的值  
            double normalized = Math.IEEERemainder(angleRad, Math.PI * 2);

            // 如果结果在-π到0之间，则加上2π使其变为0到2π之间  
            if (normalized < 0) {
                normalized += Math.PI * 2;
            }

            return normalized;
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
            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "CorruptDT").Value;
            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, -16);
            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
