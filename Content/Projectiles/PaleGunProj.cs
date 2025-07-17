using GloryofGuardian.Common;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles {
    public class PaleGunProj : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.localNPCHitCooldown = 0;
            Projectile.penetrate = 1;
        }

        Player Owner => Main.player[Projectile.owner];

        public override void AI() {
            Projectile.rotation = Projectile.velocity.ToRotation();

            base.AI();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {

            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            return base.OnTileCollide(oldVelocity);
        }

        public override void OnKill(int timeLeft) {
            if (Projectile.timeLeft > 10) Terraria.Audio.SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "PaleGunProj").Value;

            Main.EntitySpriteDraw(
                    texture,
                    Projectile.Center - Main.screenPosition,
                    null,
                    lightColor,
                    Projectile.rotation,
                    texture.Size() / 2,
                    Projectile.scale,
                    SpriteEffects.None,
                    0);

            return false;
        }
    }
}
