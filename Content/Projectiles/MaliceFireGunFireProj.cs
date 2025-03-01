using GloryofGuardian.Common;
using GloryofGuardian.Content.Dusts;
using System;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class MaliceFireGunFireProj : GOGProj
    {
        public override string Texture => GOGConstant.Asset + "placeholder";

        public override void SetStaticDefaults() {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 20;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults() {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Summon;
            Projectile.timeLeft = 60;
            Projectile.light = 3.0f;
            Projectile.ignoreWater = false;
            //Projectile.usesLocalNPCImmunity = true;
            //Projectile.localNPCHitCooldown = 60;
            Projectile.idStaticNPCHitCooldown = 12;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;

            Projectile.extraUpdates += 3;
        }

        Player Owner => Main.player[Projectile.owner];

        public override void OnSpawn(IEntitySource source) {
            count += Main.rand.Next(4);
        }

        int count = 0;
        public override void AI() {
            count++;

            float dustscale = count / 8;
            float dustvel = count / 8f;

            if (Projectile.wet) Projectile.Kill();

            //粒子
            int dustnum = Math.Min((int)(count / 30f) + 3, 6);

            if (count % 4 == 0) {
                int num1 = Dust.NewDust(Projectile.position, Projectile.width / 2, Projectile.height / 2, ModContent.DustType<FireLightDust>(), 0, 0, 0, default, 0.5f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].velocity = new Vector2(0, -dustvel);
                Main.dust[num1].scale *= (1 + dustscale);
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            Projectile.velocity *= 0.8f;
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
