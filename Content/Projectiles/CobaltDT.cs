using GloryofGuardian.Common;
using GloryofGuardian.Content.Buffs;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class CobaltDT : GOGDT
    {
        //不使用贴图,重写绘制
        public override string Texture => GOGConstant.nulls;
        public override void SetStaticDefaults() {
        }

        public sealed override void SetDefaults() {
            Projectile.width = 52;
            Projectile.height = 66;
            Projectile.tileCollide = true;

            Projectile.friendly = true;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.penetrate = -1;
            Projectile.scale *= 1f;
            Projectile.timeLeft = 36000;
            Projectile.light = 1.5f;

            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 50;

            Projectile.scale *= 1f;
        }

        Player Owner => Main.player[Projectile.owner];

        //防止破坏地图道具
        public override bool? CanCutTiles() {
            return false;
        }

        //生成时自由下坠
        public override void OnSpawn(IEntitySource source) {
            count0 = 240;//默认发射间隔
            Projectile.velocity = new Vector2(0, 8);
            base.OnSpawn(source);
        }

        int count = 0;
        int count0 = 0;
        int interval = 0;
        //状态机
        int mode = 0;
        int modecount = 0;
        float projrotvel = 0;
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

            //蓄力状态，刚召唤或攻击后来到这个状态
            //在该状态下，刃叶旋转重新加速
            if (mode == 0) {
                modecount++;
                projrotvel = ((float)modecount / Gcount) * 0.2f;
                if (projrotvel >= 0.05) Projectile.localNPCHitCooldown = 44;
                if (projrotvel >= 0.1) Projectile.localNPCHitCooldown = 36;
                if (projrotvel >= 0.15) Projectile.localNPCHitCooldown = 26;
                if (projrotvel >= 0.2) Projectile.localNPCHitCooldown = 14;

                if (modecount >= Gcount) {
                    mode = 1;
                    projrotvel = 0.2f;
                    count = 0;
                    modecount = 0;
                    Projectile.localNPCHitCooldown = 12;
                }
            }

            //待机状态，该状态下可以攻击
            //该状态下，刃叶旋转速度满速
            if (mode == 1) {
                //索敌与行动
                NPC target1 = Projectile.Center.InDir2ClosestNPC(1600, false, true);
                if (target1 != null) {
                    Attack(target1);
                }

                if (target1 == null) {
                    count = 0;
                    firstatk = false;
                }
            }

            //粒子效果
            for (int i = 0; i <= 16; i++) {
                float cirrot = (i % 4) * MathHelper.PiOver2;
                float dustrut = projrot + cirrot;

                Dust dust1 = Dust.NewDustDirect(Projectile.Center + new Vector2(0, -28) + new Vector2(-6, -6)
                    + new Vector2((float)Math.Cos(dustrut), (float)Math.Sin(dustrut)) * 42f,
                    1, 1, DustID.NorthPole, 1f, 1f, 100, Color.White, 0.8f);
                dust1.velocity *= 0;
                dust1.velocity += (dust1.position - (Projectile.Center + new Vector2(0, -28) + new Vector2(-6, -6))).SafeNormalize(Vector2.Zero) * 18f * projrotvel;
                dust1.noGravity = true;
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
            Vector2 tarpos = target1.Center;
            Vector2 projcen = Projectile.Center + new Vector2(0, -28);
            //发射
            if (count >= Gcount) {
                //普通
                if (Main.rand.Next(100) >= Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) {
                    Vector2 nowvel = (tarpos - projcen).SafeNormalize(Vector2.Zero) * 16f;
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen, nowvel.RotatedBy(Main.rand.NextFloat(-0.01f, 0.01f)), ModContent.ProjectileType<CobaltProj>(), lastdamage, 0, Owner.whoAmI, 0, target1.whoAmI);
                    if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                        if (proj1.ModProjectile is GOGProj proj2) {
                            proj2.OrichalcumMarkProj = true;
                            proj2.OrichalcumMarkProjcount = 300;
                        }
                    }

                    //状态重置
                    mode = 0;
                    count = Owner.GetModPlayer<GOGModPlayer>().GcountEx;
                }

                //过载
                if (Main.rand.Next(100) < Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) {
                    Vector2 nowvel = (tarpos - projcen).SafeNormalize(Vector2.Zero) * 16f;
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen, nowvel.RotatedBy(Main.rand.NextFloat(-0.01f, 0.01f)), ModContent.ProjectileType<CobaltProj>(), lastdamage, 0, Owner.whoAmI, 1, target1.whoAmI);
                    if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                        if (proj1.ModProjectile is GOGProj proj2) {
                            proj2.OrichalcumMarkProj = true;
                            proj2.OrichalcumMarkProjcount = 300;
                        }
                    }

                    //状态重置
                    mode = 0;
                    count = Owner.GetModPlayer<GOGModPlayer>().GcountEx;

                }

                count = Owner.GetModPlayer<GOGModPlayer>().GcountEx;
            }
        }

        public override bool? CanHitNPC(NPC target) {
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            target.AddBuff(ModContent.BuffType<CobaltDebuff>(), 300);

            base.OnHitNPC(target, hit, damageDone);
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
            if (GOGUtils.CircularHitboxCollision(Projectile.Center + new Vector2(0, -28), 42, targetHitbox) && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, targetHitbox.TopLeft(), targetHitbox.Width, targetHitbox.Height)) {
                return true;
            } else return false;
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


        //浮动因子
        //int floatcount = 0;
        float projrot = 0;
        public override bool PreDraw(ref Color lightColor) {
            //floatcount++;
            projrot += projrotvel;

            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "CobaltDT").Value;
            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, 0);
            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "CobaltProj").Value;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition + new Vector2(0, -28);

            Main.EntitySpriteDraw(texture, drawPosition, null, lightColor * (projrotvel * 3 + 0.4f), projrot, texture.Size() * 0.5f, Projectile.scale * 1.2f, SpriteEffects.None, 0);

            return false;
        }
    }
}