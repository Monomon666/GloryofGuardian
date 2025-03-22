using GloryofGuardian.Common;
using GloryofGuardian.Content.Buffs;
using GloryofGuardian.Content.Dusts;
using System;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class FireDragonProj : GOGProj
    {
        public override string Texture => GOGConstant.Asset + "placeholder";

        public override void SetStaticDefaults() {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults() {
            Projectile.width = 4;
            Projectile.height = 4;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = 60;
            Projectile.light = 3.0f;
            Projectile.ignoreWater = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;

            Projectile.extraUpdates += 3;
        }

        Player Owner => Main.player[Projectile.owner];

        public override void OnSpawn(IEntitySource source) {
        }

        int count = 0;
        public override void AI() {
            if (count == 0) count += Main.rand.Next(4);
            if (Projectile.ai[0] > 0) Projectile.localNPCHitCooldown = 60;
            count++;

            float dustscale = count / 8;
            float dustvel = count / 8f;

            if (Projectile.wet) Projectile.Kill();

            int dusttype = ModContent.DustType<FireLightDust>();
            if (Projectile.ai[0] == 1) dusttype = ModContent.DustType<FireLightDust>();
            if (Projectile.ai[0] == 2) dusttype = ModContent.DustType<FireLightDust2>();

            //粒子
            int dustnum = Math.Min((int)(count / 30f) + 3, 6);

            if (count % 1 == 0) {
                int num1 = Dust.NewDust(Projectile.position, Projectile.width / 2, Projectile.height / 2, dusttype, 0, 0, 0, default, 0.5f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].velocity = new Vector2(0, -dustvel);
                Main.dust[num1].scale *= (1 + dustscale);
                if (Projectile.ai[0] == 3) Main.dust[num1].scale = 0.8f;
            }

            if (count % 16 == 0 && Main.rand.NextBool(3)) {
                for (int i = 0; i < 1; i++) {
                    int num1 = Dust.NewDust(Projectile.position, Projectile.width / 2, Projectile.height / 2, DustID.OrangeTorch, 0, 0, 0, Color.Red, 1f);
                    Main.dust[num1].noGravity = false;
                    Main.dust[num1].velocity = Projectile.velocity;
                    Main.dust[num1].scale *= Main.rand.NextFloat(1, 1.5f);
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            if (Projectile.ai[0] < 3) Projectile.velocity *= 0.5f;

            if (Projectile.ai[0] == 0) {
                if (Main.rand.Next(100) < (Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) / 2f) target.AddBuff(BuffID.Oiled, 180);

                target.AddBuff(BuffID.OnFire, 180);
                if (target.HasBuff(BuffID.OnFire) && Main.rand.NextBool(25)) target.AddBuff(BuffID.OnFire3, 180);
                if (target.HasBuff(BuffID.Oiled) && !target.boss) target.velocity *= 0.9f;
                if (target.HasBuff(BuffID.Oiled) && target.type == NPCID.DukeFishron) target.velocity *= 0.995f;
            }

            if (Projectile.ai[0] == 1) {
                target.AddBuff(BuffID.OnFire, 300);
                target.AddBuff(BuffID.OnFire3, 300);
                target.AddBuff(BuffID.Oiled, 300);
            }

            if (Projectile.ai[0] == 2) {
                target.AddBuff(BuffID.OnFire, 300);
                target.AddBuff(BuffID.OnFire3, 300);
                target.AddBuff(BuffID.Frostburn2, 300);
                target.AddBuff(BuffID.CursedInferno, 300);
                target.AddBuff(ModContent.BuffType<OnDragonFireDebuff>(), 300);
            }

            if (Projectile.ai[0] == 3) {
                target.AddBuff(ModContent.BuffType<OnDragonFireDebuff>(), 11);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {

            return base.OnTileCollide(oldVelocity);
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        public override void OnKill(int timeLeft) {
        }
    }
}
