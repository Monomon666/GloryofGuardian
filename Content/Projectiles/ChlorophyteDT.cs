using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class ChlorophyteDT : GOGDT
    {
        //不使用贴图,重写绘制
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
            rangeMultiplier = 0.5f; // 当前范围倍数
            MaxStacks = 10; // 最大叠加层数
            StackValue = 0.08f; // 每层增幅
            RecoilSpeed = 0.01f; // 每秒回缩速率

            count0 = 60;//默认发射间隔
            Projectile.velocity = new Vector2(0, 8);
            base.OnSpawn(source);
        }

        int count = 0;
        int count0 = 0;
        //重力
        bool drop = true;
        //数据读取
        int Gcount = 0;
        int Acount = 0;
        int lastdamage = 0;
        //特殊数值
        public float PstackCount = 0;//交互生长,实现不了,已删除

        float rangeMultiplier = 1f;// 当前范围倍数
        int stackCount;// 当前叠加层数
        int MaxStacks = 0;// 最大叠加层数
        float StackValue = 0;// 每层增幅
        float RecoilSpeed = 0;// 每秒回缩速率
        int countdec = 0;// 回缩倒计时
        public override void AI() {
            //同步
            if (PstackCount > stackCount) {
                stackCount = (int)PstackCount;
            }
            PstackCount = stackCount;

            count++;
            Projectile.timeLeft = 2;
            Projectile.StickToTiles(false, false);//形成判定
            Drop();
            Calculate();
            //常态下即攻击
            Attack();

            // 每秒回缩1%（每帧减少0.01/60）
            countdec--;
            if (rangeMultiplier > 0.5f && countdec < 0) {
                rangeMultiplier -= RecoilSpeed;
                rangeMultiplier = Math.Max(rangeMultiplier, 0.5f); // 不低于原始大小

                // 当回到基础值时重置层数
                if (rangeMultiplier <= 0.5f) {
                    stackCount = 0;
                    rangeMultiplier = 0.5f;
                }
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
        void Attack() {
            Vector2 projcen = Projectile.Center + new Vector2(0, -12);
            float AddR = 1 * rangeMultiplier;// 应用动态范围系数

            //碰撞粒子展现
            for (int i = 0; i <= 1; i++) {
                Dust dust1 = Dust.NewDustDirect(projcen + Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * Main.rand.Next(240) * AddR, 8, 8, DustID.ChlorophyteWeapon, 1f, 1f, 100, Color.White, 1f);
                dust1.velocity *= 0;
                dust1.noGravity = true;
            }
            for (int i = 0; i <= 5; i++) {
                Dust dust1 = Dust.NewDustDirect(projcen + Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * 240 * AddR, 8, 8, DustID.ChlorophyteWeapon, 1f, 1f, 100, Color.White, 1f);
                dust1.velocity *= 0;
                dust1.noGravity = true;
            }

            //发射
            //9f大约稳定在圈边缘
            Vector2 velfire = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1)).SafeNormalize(Vector2.Zero) * 2f * AddR;
            if (count >= Gcount) {
                //普通
                if (Main.rand.Next(100) >= Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) {
                    for (int i = 0; i < 1; i++) {
                        Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen, velfire, ModContent.ProjectileType<ChlorophyteProj>(), lastdamage, 2, Owner.whoAmI);
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
                    for (int i = -1; i < 2; i++) {
                        Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), projcen, velfire.RotatedBy(2f * i), ModContent.ProjectileType<ChlorophyteProj>(), lastdamage, 2, Owner.whoAmI);
                        if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                            if (proj1.ModProjectile is GOGProj proj2) {
                                proj2.OrichalcumMarkProj = true;
                                proj2.OrichalcumMarkProjcount = 300;
                            }
                        }
                    }
                }

                //计时重置
                count = Owner.GetModPlayer<GOGModPlayer>().GcountEx;
            }
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
            fallThrough = true;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            float AddR = 1 * rangeMultiplier;
            if (GOGUtils.CircularHitboxCollision(Projectile.Center, 240 * AddR, targetHitbox)) {
                return true;
            }
            return false;
        }

        public override bool? CanHitNPC(NPC target) {
            return true;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            countdec = 120;//重置回缩倒计时

            for (int i = 0; i <= 16; i++) {
                Dust dust1 = Dust.NewDustDirect(target.position, target.width, target.height,
                    DustID.ChlorophyteWeapon, 1f, 1f, 100, Color.White, 1.2f);
                dust1.velocity *= 1;
                dust1.noGravity = true;
            }

            // 新增：击中时扩大范围
            if (target.type != NPCID.TargetDummy && stackCount < MaxStacks) {
                rangeMultiplier *= (1 + StackValue); // 增加5%
                stackCount++;
            }
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            //Projectile.velocity *= 0;
            return false;
        }

        public override void OnKill(int timeLeft) {
            //爆炸粒子
            for (int j = 0; j < 15; j++) {
                int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.ChlorophyteWeapon, 0f, 0f, 10, Color.White, 1);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].velocity *= 2f;
            }
            for (int j = 0; j < 15; j++) {
                int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.ChlorophyteWeapon, 0f, 0f, 10, Color.White, 0.6f);
                Main.dust[num2].noGravity = true;
                Main.dust[num2].velocity *= 1f;
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + Name).Value;
            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, -16);
            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}