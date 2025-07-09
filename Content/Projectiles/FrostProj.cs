using GloryofGuardian.Common;
using GloryofGuardian.Content.Buffs;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles {
    public class FrostProj : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.localNPCHitCooldown = -1;
            Projectile.penetrate = 1;
        }

        Player Owner => Main.player[Projectile.owner];

        public override void AI() {
            if (Projectile.ai[0] == 0) {
                for (int i = 0; i < 8; i++) {
                    int num = Dust.NewDust(Projectile.Center + Projectile.velocity.RotatedBy(Main.rand.NextFloat(0, MathHelper.Pi * 2)) * 0.6f, 0, 0, DustID.SnowSpray, 0f, 0f, 10, Color.White, 0.6f);
                    Main.dust[num].velocity = (Main.dust[num].position.Toz(Projectile.Center)) * 1f;
                    Main.dust[num].noGravity = true;
                }
            }

            if (Projectile.ai[0] == 1 && count % 1 == 0) {
                //SnowflakeIce SnowSpray
                for (int i = 0; i < 12; i++) {
                    int num = Dust.NewDust(Projectile.Center + Projectile.velocity.RotatedBy(count / 4f + MathHelper.Pi * (i % 2)) * 0.8f, 0, 0, (Main.rand.NextBool(2)) ? DustID.SnowSpray : DustID.SnowflakeIce, 0f, 0f, 10, Color.White, 1f);
                    Main.dust[num].velocity *= 0f;
                    Main.dust[num].noGravity = true;
                }
            }

            base.AI();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            if (target.HasBuff(BuffID.Frostburn) || target.HasBuff(BuffID.Frostburn2)) {
                for (int i = 0; i < 48; i++) {

                    int num = Dust.NewDust(Projectile.Center + new Vector2(0, -60).RotatedBy(Main.rand.NextFloat(0, MathHelper.Pi * 2)), 0, 0, (Main.rand.NextBool(3)) ? DustID.SnowflakeIce : DustID.SnowSpray, 0f, 0f, 10, Color.White, 1f);
                    Main.dust[num].velocity = Main.dust[num].position.Toz(Projectile.Center).RotatedBy(MathHelper.PiOver2) * 4;
                    Main.dust[num].velocity *= Main.dust[num].type == DustID.SnowSpray ? 2 : 1;
                    Main.dust[num].noGravity = true;

                    int num2 = Dust.NewDust(Projectile.Center + new Vector2(0, -60).RotatedBy(Main.rand.NextFloat(0, MathHelper.Pi * 2)), 0, 0, (Main.rand.NextBool(3)) ? DustID.SnowflakeIce : DustID.SnowSpray, 0f, 0f, 10, Color.White, 1f);
                    Main.dust[num2].velocity = Main.dust[num2].position.Toz(Projectile.Center).RotatedBy(MathHelper.PiOver2) * 4;
                    Main.dust[num2].noGravity = true;
                }
            }
            else {
                for (int i = 0; i < 48; i++) {
                    int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, (Main.rand.NextBool(3)) ? DustID.SnowflakeIce : DustID.SnowSpray, 0f, 0f, 10, Color.White, 1f);
                    Main.dust[num].velocity = Projectile.velocity.SafeNormalize(Vector2.Zero).RotatedBy(Main.rand.NextFloat(-0.5f, 0.5f));
                    Main.dust[num].velocity *= Main.rand.NextFloat(0, 3f);
                    Main.dust[num].velocity *= Main.dust[num].type == DustID.SnowSpray ? 2 : 1;
                    Main.dust[num].noGravity = true;
                }
            }

            if (Projectile.ai[0] == 1) {
                target.AddBuff(BuffID.Frostburn, 180);
                target.velocity *= 0.5f;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            for (int j = 0; j < 5; j++) {
                int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SnowSpray, 0f, 0f, 10, Color.Black, 1.7f);
                Main.dust[num2].noGravity = true;
                Main.dust[num2].velocity *= 1f;
                num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SnowflakeIce, 0f, 0f, 10);
                Main.dust[num2].velocity *= 1f;
            }
            return base.OnTileCollide(oldVelocity);
        }

        public override void OnKill(int timeLeft) {
            //爆炸
            if (Projectile.ai[0] == 1) {
                for (int j = 0; j < 5; j++) {
                    int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SnowSpray, 0f, 0f, 10, Color.Black, 1.7f);
                    Main.dust[num2].noGravity = true;
                    Main.dust[num2].velocity *= 1f;
                    num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SnowflakeIce, 0f, 0f, 10);
                    Main.dust[num2].velocity *= 1f;
                }

                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = 160;
                Projectile.position.X = Projectile.position.X - Projectile.width / 2;
                Projectile.position.Y = Projectile.position.Y - Projectile.height / 2;
                Projectile.maxPenetrate = -1;
                Projectile.penetrate = -1;
                Projectile.usesLocalNPCImmunity = false;
                Projectile.usesIDStaticNPCImmunity = true;
                Projectile.idStaticNPCHitCooldown = 0;
                Projectile.Damage();
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            if (Projectile.ai[0] == 0) {
                Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FrostProj").Value;

                Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0, 0);
                Main.EntitySpriteDraw(texture, drawPos, null, lightColor, Projectile.rotation, texture.Size() / 2, Projectile.scale, SpriteEffects.None, 0);

            }else return false;
            
            return false;
        }
    }
}
