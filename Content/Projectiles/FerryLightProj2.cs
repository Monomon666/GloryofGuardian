using GloryofGuardian.Common;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class FerryLightProj2 : GOGProj
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

            Projectile.scale *= 1.5f;
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

            if (Projectile.ai[0] == 1 && count == 2) Projectile.penetrate = 3;

            if (count % 1 == 0) {
                for (int i = 0; i < 4; i++) {
                    int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Flare, 0f, 0f, 10, Color.White, 2f);
                    Main.dust[num].velocity *= 0.5f;
                    Main.dust[num].noGravity = true;
                }
            }
        }

        public override Color? GetAlpha(Color lightColor) {
            return base.GetAlpha(lightColor);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            if (target.HasBuff(BuffID.OnFire3)) {
                //常态
                if (Projectile.ai[0] == 0) {
                    Projectile.damage /= 2;
                    Projectile.ai[0] = -1;
                }
                //强化
                if (Projectile.ai[0] == 1) {
                    Projectile.ai[0] = -2;
                    Projectile.Kill();
                }
            }

            //常态
            if (Projectile.ai[0] == 0) {
                target.AddBuff(BuffID.OnFire, 30);
            }
            //强化
            if (Projectile.ai[0] == 1) {
                target.AddBuff(BuffID.OnFire, 180);
            }
            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnKill(int timeLeft) {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            //
            if (Projectile.ai[0] == 0) {

            }
            //常态
            if (Projectile.ai[0] == -1) {
                //爆炸
                for (int j = 0; j < 52; j++) {
                    int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.InfernoFork, 0, 0, 0, Color.White, 1f);
                    Main.dust[num2].noGravity = true;
                    Main.dust[num2].velocity = new Vector2((float)Math.Sin(j * 12 / 100f), (float)Math.Cos(j * 12 / 100f)) * Main.rand.NextFloat(6f, 7f);
                }
                SoundEngine.PlaySound(in SoundID.NPCHit3, Projectile.Center);
                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = 80;
                Projectile.position.X = Projectile.position.X - Projectile.width / 2;
                Projectile.position.Y = Projectile.position.Y - Projectile.height / 2;
                Projectile.maxPenetrate = -1;
                Projectile.penetrate = -1;
                Projectile.usesLocalNPCImmunity = false;
                Projectile.usesIDStaticNPCImmunity = true;
                Projectile.idStaticNPCHitCooldown = 0;
                Projectile.Damage();
            }
            //强化
            if (Projectile.ai[0] == -2) {
                //爆炸
                for (int j = 0; j < 52; j++) {
                    int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.InfernoFork, 0, 0, 0, Color.White, 1f);
                    Main.dust[num2].noGravity = true;
                    Main.dust[num2].velocity = new Vector2((float)Math.Sin(j * 12 / 100f), (float)Math.Cos(j * 12 / 100f)) * Main.rand.NextFloat(8f, 9f);
                }
                SoundEngine.PlaySound(in SoundID.NPCHit3, Projectile.Center);
                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = 160;
                Projectile.position.X = Projectile.position.X - Projectile.width / 2;
                Projectile.position.Y = Projectile.position.Y - Projectile.height / 2;
                Projectile.maxPenetrate = -1;
                Projectile.penetrate = -1;
                Projectile.usesLocalNPCImmunity = false;
                Projectile.usesIDStaticNPCImmunity = true;
                Projectile.idStaticNPCHitCooldown = 0;
                Projectile.Damage();
            }

            //基本爆炸粒子
            for (int i = 0; i < 12; i++) {
                int num = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, DustID.Flare, 0f, 0f, 50, Color.White, 2f);
                Main.dust[num].velocity *= 2f;
                if (Main.rand.NextBool(2)) {
                    Main.dust[num].scale = 1f;
                    Main.dust[num].noGravity = true;
                    Main.dust[num].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            return false;
        }
    }
}
