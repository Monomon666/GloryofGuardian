using GloryofGuardian.Common;
using GloryofGuardian.Content.Buffs;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles {
    public class HarpyProj : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.localNPCHitCooldown = -1;
            Projectile.penetrate = 2;

            Projectile.alpha = 255;

            Projectile.scale *= 0.6f;

            base.SetProperty();
        }
        Player Owner => Main.player[Projectile.owner];

        public override void AI() {
            Lighting.AddLight(Projectile.Center, 0.5f, 0.7f, 0.8f);
            //渐入
            if (Projectile.alpha <= 0) Projectile.alpha = 0;
            else if (Projectile.alpha > 0) Projectile.alpha -= 15;
            // 旋转
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.PiOver2 + 0.05f;

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

            if (count > fadetime + 180) Projectile.Kill();
        }

        Vector2 relapos = Vector2.Zero;
        NPC sticknpc = null;
        int fadetime = 0;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            if (mode == 0) {
                mode = 1;//改变状态机为钉入状态
                fadetime = count;
                relapos = target.Center.To(Projectile.Center);
                sticknpc = target;
                pmode = 1;//标记为已击中
                Projectile.ai[1] = target.whoAmI;//标记正确的受debuff的目标
                Projectile.friendly = false;

                target.AddBuff(ModContent.BuffType<JavelinDebuff>(), 60);
            }
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            return base.OnTileCollide(oldVelocity);
        }

        public override void OnKill(int timeLeft) {
            for (int i = 0; i < 4; i++) {
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Cloud, 0f, 0f, 10, Color.White, 2f);
                Main.dust[num].velocity *= 0.5f;
                Main.dust[num].noGravity = true;
            }
            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "HarpyProj").Value;
            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "HarpyProj2Shadow").Value;
            Texture2D texture2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "HarpyProj2BlackShadow").Value;

            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0, -8);

            Main.EntitySpriteDraw(texture2, drawPos, null, new Color(148, 216, 255) * ((255f - Projectile.alpha) / 255f), Projectile.rotation, texture2.Size() / 2, Projectile.scale * 1.2f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture1, drawPos, null, new Color(148, 216, 255, 0) * ((255f - Projectile.alpha) / 255f), Projectile.rotation, texture1.Size() / 2, Projectile.scale * 1.5f, SpriteEffects.None, 0);

            Main.EntitySpriteDraw(texture, drawPos, null, lightColor * ((255f - Projectile.alpha) / 255f), Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
