using GloryofGuardian.Common;
using GloryofGuardian.Content.ParentClasses;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles {
    public class Bloody2Proj4 : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {

            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.localNPCHitCooldown = 20;
            Projectile.penetrate = 5;
        }

        public override void AI() {
            if (Projectile.ai[0] != 0) {
                Projectile.extraUpdates = 2;
                //遵循传入的重力的影响
                float G = Projectile.ai[0];
                Projectile.velocity += new Vector2(0, G);
            }

            if (count % 1 == 0) {
                for (int i = 0; i < 4; i++) {
                    int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Ichor, 0f, 0f, 50, Color.White, 1f);
                    Main.dust[num].velocity *= 0f;
                    Main.dust[num].noGravity = true;
                }
            }

            if (count % 1 == 0) {
                for (int i = 0; i < 4; i++) {
                    int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Ichor, 0f, 0f, 50, Color.White, 0.8f);
                    Main.dust[num].velocity *= 0.5f;
                    Main.dust[num].noGravity = true;
                }
            }

            if (count % 1 == 0 && Projectile.ai[1] > 1) {
                for (int i = 0; i < 4; i++) {
                    int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Ichor, 0f, 0f, 50, Color.White, 1f);
                    Main.dust[num].velocity *= 0f;
                    Main.dust[num].noGravity = true;
                }
            }

            base.AI();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            target.AddBuff(BuffID.Ichor, 300);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            return base.OnTileCollide(oldVelocity);
        }

        public override void OnKill(int timeLeft) {
            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor) {
            return base.PreDraw(ref lightColor);
        }
    }
}
