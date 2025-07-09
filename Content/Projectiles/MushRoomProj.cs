using GloryofGuardian.Common;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles {
    public class MushRoomProj : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 22;
            Projectile.height = 24;
            Projectile.localNPCHitCooldown = 30;
            Projectile.penetrate = -1;
        }

        Player Owner => Main.player[Projectile.owner];

        public override void AI() {
            Projectile.rotation += 0.1f;
            if (count >= 120) Projectile.Kill();

            base.AI();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            return base.OnTileCollide(oldVelocity);
        }

        public override void OnKill(int timeLeft) {
            for (int i = 0; i < 12; i++) {
                float angle = Main.rand.NextFloat(0, MathHelper.Pi * 2);
                int num = Dust.NewDust(Projectile.Center, 0, 0, DustID.MushroomTorch, 0f, 0f, 50, Color.White, 3f);
                Main.dust[num].velocity *= 4f;
                Main.dust[num].noGravity = true;
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "MushRoomProj").Value;

            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            Main.EntitySpriteDraw(texture, drawPos, null, lightColor, Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
