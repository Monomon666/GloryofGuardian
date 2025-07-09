using GloryofGuardian.Common;
using GloryofGuardian.Content.Buffs;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles {
    public class CorruptRainProj : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.localNPCHitCooldown = -1;
            Projectile.penetrate = 3;
        }

        Player Owner => Main.player[Projectile.owner];

        public override void AI() {
            if (count >= 300) Projectile.Kill();

            //if (Projectile.alpha <= 0) Projectile.alpha = 0;
            //else if (Projectile.alpha > 0) Projectile.alpha -= 15;

            base.AI();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            if (Projectile.ai[0] == 1) target.AddBuff(ModContent.BuffType<CorruptDebuff>(), 300);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            return base.OnTileCollide(oldVelocity);
        }

        public override void OnKill(int timeLeft) {
            //基本爆炸粒子
            if (Projectile.ai[0] == 0) {
                for (int i = 0; i < 4; i++) {
                    int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Corruption, 0f, 0f, 50, Color.White, 0.6f);
                    Main.dust[num].velocity *= 1f;
                    Main.dust[num].noGravity = false;
                }
            }

            if (Projectile.ai[0] == 1) {
                for (int i = 0; i < 1; i++) {
                    int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.CorruptGibs, 0f, 0f, 50, Color.White, 0.8f);
                    Main.dust[num].velocity *= 1f;
                    Main.dust[num].noGravity = false;
                    if (Main.rand.NextBool(2)) {
                        Main.dust[num].scale = 0.5f;
                        Main.dust[num].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                    }
                }
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "CorruptRainProj").Value;
            Texture2D texture2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "CorruptRainProj2").Value;

            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0, -8);
            if (Projectile.ai[0] == 0) {
                Main.EntitySpriteDraw(texture, drawPos, null, lightColor, Projectile.rotation, texture2.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

            }
            if (Projectile.ai[0] == 1) {
                Main.EntitySpriteDraw(texture2, drawPos, null, Color.Gray, Projectile.rotation, texture2.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

            }

            return false;
        }
    }
}
