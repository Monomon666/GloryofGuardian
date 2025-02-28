using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;

namespace DreamJourney.Content.Projectiles.Ranged
{
    public class SlimeProj : GOGProj
    {
        public override string Texture => GOGConstant.Projectiles + Name;

        public override void SetStaticDefaults() {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults() {
            Projectile.width = 10;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.timeLeft = 1200;
            //Projectile.light = 1.0f;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 6;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 5;

            Projectile.extraUpdates = 0;
            Projectile.light = 1.5f;
            Projectile.scale *= 1f;
        }

        Player Owner => Main.player[Projectile.owner];
        Vector2 OwnerPos => Owner.Center;
        Vector2 ToMou => Main.MouseWorld - OwnerPos;

        public override void OnSpawn(IEntitySource source) {
            Projectile.tileCollide = false;
            if (Projectile.ai[0] == 3) {
                Projectile.localNPCHitCooldown = 12;
            }
        }

        int count = 0;
        int mode = 0;
        public override void AI() {
            count++;
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (count >= 20) Projectile.tileCollide = true;

            Projectile.velocity.Y += 0.35f;
            if (Projectile.velocity.Y >= 4) {
                Projectile.velocity.Y = 8;
                Projectile.velocity.X *= 0.95f;
            }

            for (int j = 0; j < 2; j++) {
                int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BlueFairy, 0f, 0f, 10, Color.White, 1f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].velocity = Projectile.velocity;
            }
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        public override void OnKill(int timeLeft) {
            if (Projectile.timeLeft > 10) Terraria.Audio.SoundEngine.PlaySound(SoundID.Dig);

            //爆炸粒子
            for (int j = 0; j < 15; j++) {
                int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BlueFairy, 0f, 0f, 10, Color.White, 0.8f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].velocity *= 2f;
            }
            for (int j = 0; j < 15; j++) {
                int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BlueFairy, 0f, 0f, 10, Color.White, 0.4f);
                Main.dust[num2].noGravity = true;
                Main.dust[num2].velocity *= 1f;
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;

            Main.EntitySpriteDraw(
                    texture,
                    Projectile.Center - Main.screenPosition,
                    null,
                    lightColor,
                    Projectile.rotation,
                    texture.Size() / 2,
                    Projectile.scale,
                    SpriteEffects.None,
                    0); ;

            return false;
        }
    }
}
