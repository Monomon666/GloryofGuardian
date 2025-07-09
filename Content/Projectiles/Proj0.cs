using GloryofGuardian.Common;
using GloryofGuardian.Content.ParentClasses;

namespace GloryofGuardian.Content.Projectiles {
    public class Proj0 : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            base.SetProperty();
        }

        public override void AI() {
            base.AI();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            base.OnHitNPC(target, hit, damageDone);
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
