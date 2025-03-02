﻿using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class Blood2Proj0 : GOGProj
    {
        public override string Texture => GOGConstant.nulls;

        public override void SetStaticDefaults() {
            //残影机制
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults() {
            //这里的尺寸对应的是碰撞体积
            Projectile.width = 78;
            Projectile.height = 80;
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

            Projectile.scale *= 0.8f;
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

            Projectile.rotation += 0.1f;
            float G = Projectile.ai[0];

            Projectile.velocity += new Vector2(0, G);

            if (count % 1 == 0) {
                for (int i = 0; i < 4; i++) {
                    int num = Dust.NewDust(Projectile.Center + new Vector2(0, -40), 20, 40, DustID.Crimson, 0f, 0f, 50, Color.Red, 1f);
                    Main.dust[num].velocity *= 0f;
                    Main.dust[num].noGravity = true;
                }
            }

            if (count % 1 == 0) {
                for (int i = 0; i < 4; i++) {
                    int num = Dust.NewDust(Projectile.Center + new Vector2(0, -20), 40, 20, DustID.Ichor, 0f, 0f, 50, Color.White, 1f);
                    Main.dust[num].velocity *= 0f;
                    Main.dust[num].noGravity = true;
                }
            }

            if (count % 30 == 0 && Main.rand.NextBool(2)) {
                Fire();
            }
        }

        void Fire() {
            if (Projectile.ai[1] == 0) {
                for (int i = 0; i < 8; i++) {
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center + new Vector2(10, -20), new Vector2(0, -4).RotatedBy(MathHelper.PiOver4 * i), ModContent.ProjectileType<Blood2Proj1>(), Projectile.damage, 1, Owner.whoAmI);
                    if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                        if (proj1.ModProjectile is GOGProj proj2) {
                            proj2.OrichalcumMarkProj = true;
                            proj2.OrichalcumMarkProjcount = 300;
                        }
                    }
                }
            }

            if (Projectile.ai[1] == 1) {
                for (int i = 0; i < 8; i++) {
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center + new Vector2(10, -20), new Vector2(0, -4).RotatedBy(MathHelper.PiOver4 * i), ModContent.ProjectileType<Blood2Proj1>(), Projectile.damage, 1, Owner.whoAmI, 1);
                    if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                        if (proj1.ModProjectile is GOGProj proj2) {
                            proj2.OrichalcumMarkProj = true;
                            proj2.OrichalcumMarkProjcount = 300;
                        }
                    }
                }
            }
        }

        public override Color? GetAlpha(Color lightColor) {
            return base.GetAlpha(lightColor);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            Fire();
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            //基本爆炸粒子
            for (int i = 0; i < 4; i++) {
                int num = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, DustID.Crimson, 0f, 0f, 50, Color.White, 0.8f);
                Main.dust[num].velocity *= 1f;
                if (Main.rand.NextBool(2)) {
                    Main.dust[num].scale = 0.5f;
                    Main.dust[num].noGravity = true;
                    Main.dust[num].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }

            //过载攻击爆炸
            if (Projectile.ai[1] > 1) {
                for (int j = 0; j < 10; j++) {
                    int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Crimson, 0f, 0f, 10, Color.Black, 1.7f);
                    Main.dust[num2].noGravity = true;
                    Main.dust[num2].velocity *= 3f;
                    num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Crimson, 0f, 0f, 10);
                    Main.dust[num2].velocity *= 3f;
                }
                SoundEngine.PlaySound(in SoundID.NPCHit9, Projectile.Center);
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
            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnKill(int timeLeft) {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);

            //爆炸
            for (int j = 0; j < 52; j++) {
                int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Crimson, 0, 0, 0, Color.White, 1f);
                Main.dust[num2].noGravity = true;
                Main.dust[num2].velocity = new Vector2((float)Math.Sin(j * 12 / 100f), (float)Math.Cos(j * 12 / 100f)) * Main.rand.NextFloat(6f, 7f);
                int num3 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Ichor, 0, 0, 0, Color.White, 1f);
                Main.dust[num3].noGravity = true;
                Main.dust[num3].velocity = new Vector2((float)Math.Sin(j * 12 / 100f), (float)Math.Cos(j * 12 / 100f)) * Main.rand.NextFloat(6f, 7f);
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

            //基本爆炸粒子
            for (int i = 0; i < 12; i++) {
                int num = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, DustID.Crimson, 0f, 0f, 50, Color.White, 2f);
                Main.dust[num].velocity *= 2f;
                if (Main.rand.NextBool(2)) {
                    Main.dust[num].scale = 1f;
                    Main.dust[num].noGravity = true;
                    Main.dust[num].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Weapons + "Bloody2Calling").Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0, 0);

            for (int k = 0; k < 2; k++) {
                Vector2 olddrawPos = (Projectile.oldPos[k] - Main.screenPosition) + new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f) + new Vector2(0f, Projectile.gfxOffY);
                Color color = lightColor * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) * 0.4f;

                if (k != 0) {
                    Main.EntitySpriteDraw(texture, olddrawPos, null, color, Projectile.rotation, new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f), Projectile.scale, SpriteEffects.None, 0);
                }

                if (k == 0) {
                    Main.EntitySpriteDraw(texture, olddrawPos, null, Color.White * 2f, Projectile.rotation, new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f), Projectile.scale, SpriteEffects.None, 0);
                }
            }


            return false;
        }
    }
}
