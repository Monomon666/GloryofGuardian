﻿using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class FrostProj : GOGProj
    {
        public override string Texture => GOGConstant.nulls;

        public override void SetStaticDefaults() {
            //残影机制
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults() {
            //这里的尺寸对应的是碰撞体积
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;//穿透数，1为攻击到第一个敌人就消失
            Projectile.tileCollide = true;

            Projectile.light = 1f;

            Projectile.scale *= 1f;
        }

        Player Owner => Main.player[Projectile.owner];

        Vector2 OwnerPos => Owner.Center;

        Vector2 ToMou => Main.MouseWorld - OwnerPos;

        int projtype = 0;
        Vector2 spawnpos = new Vector2(0, 0);
        public override void OnSpawn(IEntitySource source) {
        }

        int count = 0;
        public override void AI() {
            count++;
            int dustnum = Projectile.ai[0] == 1 ? 4 : 2;
            if (count % 1 == 0) {
                for (int i = 0; i < dustnum; i++) {
                    int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SnowflakeIce, 0f, 0f, 10, Color.White, 1f);
                    Main.dust[num].velocity *= 0.5f;
                    Main.dust[num].noGravity = true;
                }

                for (int i = 0; i < dustnum / 2; i++) {
                    int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SnowSpray, 0f, 0f, 10, Color.White, 0.5f);
                    Main.dust[num2].velocity = new Vector2(0, Main.rand.NextFloat(-0.5f, 1.5f) * 1f);
                    Main.dust[num2].noGravity = true;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor) {
            return base.GetAlpha(lightColor);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            if (target.HasBuff(BuffID.Frostburn) && !target.boss) {
                target.velocity *= 0;
            }

            //常态
            target.AddBuff(BuffID.Frostburn, 180);
            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnKill(int timeLeft) {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            if (Projectile.ai[0] == 0) {

            }
            if (Projectile.ai[0] == 1) {
                //爆炸
                for (int j = 0; j < 52; j++) {
                    int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SnowflakeIce, 0, 0, 0, Color.White, 1f);
                    Main.dust[num2].noGravity = true;
                    Main.dust[num2].velocity = new Vector2((float)Math.Sin(j * 12 / 100f), (float)Math.Cos(j * 12 / 100f)) * Main.rand.NextFloat(6f, 7f);
                }
                SoundEngine.PlaySound(in SoundID.NPCHit3, Projectile.Center);
                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = 120;
                Projectile.position.X = Projectile.position.X - Projectile.width / 2;
                Projectile.position.Y = Projectile.position.Y - Projectile.height / 2;
                Projectile.maxPenetrate = -1;
                Projectile.penetrate = -1;
                Projectile.usesLocalNPCImmunity = false;
                Projectile.usesIDStaticNPCImmunity = true;
                Projectile.idStaticNPCHitCooldown = 0;
                Projectile.Damage();
            }
            if (Projectile.ai[0] == -1) {
                //爆炸
                for (int j = 0; j < 52; j++) {
                    int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Firework_Blue, 0, 0, 0, Color.White, 1f);
                    Main.dust[num2].noGravity = false;
                    Main.dust[num2].velocity = new Vector2((float)Math.Sin(j * 12 / 100f), (float)Math.Cos(j * 12 / 100f)) * Main.rand.NextFloat(6f, 7f);
                }
                SoundEngine.PlaySound(in SoundID.NPCHit3, Projectile.Center);
                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = 120;
                Projectile.position.X = Projectile.position.X - Projectile.width / 2;
                Projectile.position.Y = Projectile.position.Y - Projectile.height / 2;
                Projectile.maxPenetrate = -1;
                Projectile.penetrate = -1;
                Projectile.usesLocalNPCImmunity = false;
                Projectile.usesIDStaticNPCImmunity = true;
                Projectile.idStaticNPCHitCooldown = 0;
                Projectile.Damage();
            }

            for (int i = 0; i < 54; i++) {
                Dust dust2 = Dust.NewDustDirect(Projectile.Center + Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * 120, 8, 8, DustID.SnowflakeIce, 1f, 1f, 100, Color.White, 1f);
                dust2.velocity *= 1;
                dust2.noGravity = false;
            }

            //基本爆炸粒子
            for (int i = 0; i < 12; i++) {
                int num = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, DustID.SnowflakeIce, 0f, 0f, 50, Color.White, 0.5f);
                if (Main.rand.NextBool(2)) {
                    Main.dust[num].scale = 1f;
                    Main.dust[num].noGravity = true;
                    Main.dust[num].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
                int num2 = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, DustID.SnowSpray, 0f, 0f, 50, Color.White, 0.5f);
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FrostProj").Value;
            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "SRFrostProj04").Value;

            Texture2D text = Projectile.ai[0] == 0 ? texture : texture1;

            Main.EntitySpriteDraw(
                    text,
                    Projectile.Center - Main.screenPosition,
                    null,
                    lightColor,
                    Projectile.rotation,
                    text.Size() / 2,
                    Projectile.scale,
                    SpriteEffects.None,
                    0);

            return false;
        }
    }
}
