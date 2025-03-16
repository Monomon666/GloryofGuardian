﻿using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles.HeldProj
{
    public class GoldTridentHeld : GOGProj
    {
        public override string Texture => GOGConstant.Weapons + "GoldTrident";


        // 蓄力时间
        private int chargeTime = 0;
        private const int MaxChargeTime = 60; // 最大蓄力时间（60 帧 = 1 秒）

        // 是否正在蓄力
        private bool isCharging = false;

        public override void SetDefaults() {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.timeLeft = 600;

            Projectile.scale *= 1.2f;
        }

        Player Owner => Main.player[Projectile.owner];

        int count = 0;
        public override void AI() {
            //计时器
            count++;
            // 使用动画延长到使用完
            Owner.itemAnimation = Owner.itemTime = 60;
            //跟着主人,以及抖动效果
            //改变玩家朝向
            Owner.direction = (Main.MouseWorld - Owner.Center).X > 0 ? 1 : -1;
            //弹幕转向:定角高射炮类型
            //Projectile.Center = Owner.Center + new Vector2(Owner.direction, -3).SafeNormalize(Vector2.Zero) * 16f;
            Projectile.Center = Owner.Center + new Vector2(0, -8);
            // （玩家被）打断的效果
            if (!Owner.active || Owner.dead || Owner.noItems || Owner.CCed) {
                Projectile.Kill();
                return;
            }

            Projectile.velocity = Owner.Center.Toz(Main.MouseWorld);
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45f);

            // 如果玩家松开左键且正在蓄力，投出投枪
            if (!Owner.controlUseItem && isCharging) {
                ThrowSpear(Owner);
                return;
            }

            // 如果玩家按住右键，取消蓄力
            if (Owner.controlUseTile) {
                Projectile.Kill();
                return;
            }

            // 蓄力逻辑
            if (Owner.controlUseItem) {
                isCharging = true;

                // 限制最大蓄力时间
                if (chargeTime > 30) {
                    chargeTime = 30;
                }

                // 显示粒子轨迹
                if (count % 2 == 0) chargeTime += 2;
                if (count % 2 == 0) ShowParticleTrajectory();
            }
        }

        // 投出投枪
        private void ThrowSpear(Player player) {
            // 计算投枪的初始速度
            float speed = 10f + (chargeTime / (float)45) * 10f; // 速度随蓄力时间增加
            Vector2 velocity = Owner.Center.Toz(Main.MouseWorld) * speed;

            // 生成投枪弹幕
            Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                Projectile.Center + new Vector2(0, 0),
                velocity,
                ModContent.ProjectileType<GoldTridentProj>(),
                Projectile.damage,
                Projectile.knockBack,
                Owner.whoAmI,
                chargeTime // 传递蓄力时间
            );

            // 重置状态
            chargeTime = 0;
            isCharging = false;
            Projectile.Kill();
        }

        // 显示粒子轨迹
        private void ShowParticleTrajectory() {
            // 计算投枪的初始速度
            float speed = 10f + (chargeTime / (float)45) * 8f; // 速度随蓄力时间增加
            Vector2 velocity = Owner.Center.Toz(Main.MouseWorld) * speed;

            // 生成投枪弹幕
            Projectile.NewProjectile(
                Projectile.GetSource_FromThis(),
                Projectile.Center + new Vector2(0, 0),
                velocity,
                ModContent.ProjectileType<GoldTridentPreProj>(),
                Projectile.damage,
                Projectile.knockBack,
                Owner.whoAmI,
                chargeTime // 传递蓄力时间
            );
        }

        public override void OnKill(int timeLeft) {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            //基本爆炸粒子
            for (int i = 0; i < 4; i++) {
                int num = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, DustID.Wraith, 0f, 0f, 50, Color.White, 0.8f);
                Main.dust[num].velocity *= 1f;
                if (Main.rand.NextBool(2)) {
                    Main.dust[num].scale = 0.5f;
                    Main.dust[num].noGravity = true;
                    Main.dust[num].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Main.EntitySpriteDraw
                (texture,
                Owner.Center + new Vector2(0, -8) - Main.screenPosition,
                null,
                lightColor,
                Projectile.rotation,
                new Vector2(74, 74) * 0.5f,
                Projectile.scale,
                SpriteEffects.None,
                0);

            return false;
        }

        public override void DrawBehind(int index, List<int> behindNPCsAndTiles, List<int> behindNPCs, List<int> behindProjectiles, List<int> overPlayers, List<int> overWiresUI) {
            base.DrawBehind(index, behindNPCsAndTiles, behindNPCs, behindProjectiles, overPlayers, overWiresUI);
        }
    }

    public class GoldTridentProj : GOGProj
    {
        public override string Texture => GOGConstant.Weapons + "GoldTrident";

        //弩箭炮塔属性预设：

        //下落阶段
        int GravityDelayTimer = 0;//下落计时器

        public override void SetStaticDefaults() {
            //残影机制
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults() {
            //这里的尺寸对应的是碰撞体积
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;//穿透数，1为攻击到第一个敌人就消失

            Projectile.scale *= 1.2f;
        }

        Player Owner => Main.player[Projectile.owner];

        int count = 0;
        public override void AI() {
            count++;
            NormalAI();

            // 遍历 ignore 列表
            for (int i = ignore.Count - 1; i >= 0; i--) {
                int npcIndex = ignore[i];
                NPC npc = Main.npc[npcIndex];

                if (npc.active && !npc.dontTakeDamage) {
                    npc.position = Projectile.Center - new Vector2(npc.width / 2, npc.height / 2);
                } else {
                    ignore.RemoveAt(i);
                }
            }

            base.AI();
        }

        //普通模式
        private void NormalAI() {
            GravityDelayTimer++;

            int GravityDelay = (int)Projectile.ai[0];

            //在一段时间内，标枪将以同样的速度运动，但在此之后，标枪的速度迅速下降。
            if (GravityDelayTimer >= GravityDelay) {
                GravityDelayTimer = GravityDelay;

                // 阻力
                Projectile.velocity.X *= 0.98f;
                // 重力
                Projectile.velocity.Y += 0.35f;
            }

            // 旋转90度
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45f);
        }

        public override Color? GetAlpha(Color lightColor) {
            return null;
        }

        List<int> ignore = new List<int>();
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            // 如果敌人不在 ignore 列表中，则加入列表
            if (!ignore.Contains(target.whoAmI) && target.knockBackResist > 0) {
                ignore.Add(target.whoAmI);
            }
        }

        public override void OnKill(int timeLeft) {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            //基本爆炸粒子
            for (int i = 0; i < 4; i++) {
                int num = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, DustID.Wraith, 0f, 0f, 50, Color.White, 0.8f);
                Main.dust[num].velocity *= 1f;
                if (Main.rand.NextBool(2)) {
                    Main.dust[num].scale = 0.5f;
                    Main.dust[num].noGravity = true;
                    Main.dust[num].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            for (int k = 0; k < Projectile.oldPos.Length; k++) {
                if (k != 0) {
                    Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f) + new Vector2(0f, Projectile.gfxOffY);

                    Color color = lightColor * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    Main.EntitySpriteDraw
                        (texture,
                        drawPos,
                        null,
                        color * 0.4f,
                        Projectile.rotation,
                        new Vector2(74, 74) * 0.5f,
                        Projectile.scale,
                        SpriteEffects.None,
                        0);
                }

                if (k == 0) {
                    Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f) + new Vector2(0f, Projectile.gfxOffY);

                    Main.EntitySpriteDraw
                        (texture,
                        drawPos,
                        null,
                        lightColor,
                        Projectile.rotation,
                        new Vector2(74, 74) * 0.5f,
                        Projectile.scale,
                        SpriteEffects.None,
                        0);
                }
            }

            return false;
        }
    }

    public class GoldTridentPreProj : GOGProj
    {
        public override string Texture => GOGConstant.Weapons + "GoldTrident";

        //弩箭炮塔属性预设：

        //下落阶段
        int GravityDelayTimer = 0;//下落计时器

        public override void SetStaticDefaults() {
            //残影机制
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults() {
            //这里的尺寸对应的是碰撞体积
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.aiStyle = -1;
            Projectile.extraUpdates += 30;

            Projectile.scale *= 1.2f;

            Projectile.hide = true;
        }

        Player Owner => Main.player[Projectile.owner];

        int count = 0;
        public override void AI() {
            count++;
            NormalAI();
            for (int i = 0; i <= 1; i++) {
                Dust dust1 = Dust.NewDustDirect(Projectile.Center + new Vector2(0, 0), 0, 0, DustID.MushroomSpray, 1f, 1f, 100, Color.White, 0.8f);
                dust1.velocity *= 0;
                dust1.noGravity = true;
            }
            base.AI();
        }

        //普通模式
        private void NormalAI() {
            GravityDelayTimer++;

            int GravityDelay = (int)Projectile.ai[0];

            //在一段时间内，标枪将以同样的速度运动，但在此之后，标枪的速度迅速下降。
            if (GravityDelayTimer >= GravityDelay) {
                GravityDelayTimer = GravityDelay;

                // 阻力
                Projectile.velocity.X *= 0.98f;
                // 重力
                Projectile.velocity.Y += 0.35f;
            }

            // 旋转90度
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45f);
        }

        public override Color? GetAlpha(Color lightColor) {
            return null;
        }
    }
}


