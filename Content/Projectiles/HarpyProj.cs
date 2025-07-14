using GloryofGuardian.Common;
using GloryofGuardian.Content.Buffs;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;

namespace GloryofGuardian.Content.Projectiles {
    public class HarpyProj : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.localNPCHitCooldown = -1;
            Projectile.penetrate = 2;

            Projectile.alpha = 255;

            base.SetProperty();
        }
        Player Owner => Main.player[Projectile.owner];

        public override void AI() {
            //渐入
            if (Projectile.alpha <= 0) Projectile.alpha = 0;
            else if (Projectile.alpha > 0) Projectile.alpha -= 15;
            // 旋转
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2;

            if (mode == 0) ai1();
            if (mode == 1) ai2();

            base.AI();
        }

        //常态
        void ai1() {

        }

        //钉入
        void ai2() {
            if (sticknpc == null || !sticknpc.active) Projectile.Kill();
            else {
                Projectile.Center = sticknpc.Center + relapos;
            }
        }

        Vector2 relapos = Vector2.Zero;
        NPC sticknpc = null;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            if (mode == 0) {
                mode = 1;//改变状态机为钉入状态
                relapos = target.Center.To(Projectile.Center);
                sticknpc = target;
                Projectile.friendly = false;

                pmode = 1;

                target.AddBuff(ModContent.BuffType<JavelinDebuff1>(), 60);
            }
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            return base.OnTileCollide(oldVelocity);
        }

        public override void OnKill(int timeLeft) {
            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "HarpyProj").Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0, -8);
            Main.EntitySpriteDraw(texture, drawPos, null, lightColor * ((255f - Projectile.alpha) / 255f), Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
