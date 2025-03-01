using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class MaliceFireGunProj : GOGProj
    {
        public override string Texture => GOGConstant.Projectiles + Name;

        public override void SetStaticDefaults() {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults() {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.timeLeft = 1200;
            //Projectile.light = 1.0f;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;

            Projectile.extraUpdates = 0;
            Projectile.light = 1.5f;
            Projectile.scale *= 1f;
        }

        Player Owner => Main.player[Projectile.owner];
        Vector2 OwnerPos => Owner.Center;
        Vector2 ToMou => Main.MouseWorld - OwnerPos;

        public override void OnSpawn(IEntitySource source) {
        }

        int count = 0;
        int mode = 0;
        public override void AI() {
            count++;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        public override void OnKill(int timeLeft) {
            if (Projectile.timeLeft > 10) Terraria.Audio.SoundEngine.PlaySound(SoundID.Dig);
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
