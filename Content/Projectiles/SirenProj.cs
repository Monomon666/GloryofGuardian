using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class SirenProj : GOGProj
    {
        public override string Texture => GOGConstant.nulls;

        public override void SetStaticDefaults() {
            //残影机制
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults() {
            //这里的尺寸对应的是碰撞体积
            Projectile.width = 24;
            Projectile.height = 24;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 6;//穿透数，1为攻击到第一个敌人就消失
            Projectile.tileCollide = false;

            Projectile.light = 1f;

            Projectile.scale *= 1.5f;
        }

        Player Owner => Main.player[Projectile.owner];

        Vector2 OwnerPos => Owner.Center;

        Vector2 ToMou => Main.MouseWorld - OwnerPos;

        int projtype = 0;
        Vector2 spawnpos = new Vector2(0, 0);
        public override void OnSpawn(IEntitySource source) {
            SetStaticDefaults();
            projtype = Main.rand.Next(4);
            if (Projectile.ai[0] == 1) {
                projtype = 4;
                spawnpos = Projectile.Center;
            }
        }

        int count = 0;
        public override void AI() {
            count++;

            Projectile.rotation = Projectile.velocity.ToRotation();

            if (projtype == 0 || projtype == 1) {
                if (count <= 120) Projectile.velocity *= 0.98f;
                if (count >= 180) Projectile.Kill();
            }

            if(projtype == 4) {
                if (count >= 30) {
                    Projectile.velocity *= 0;
                    Projectile.Center += (Projectile.Center - spawnpos).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * 12f;
                }
            }

            //if (count % 1 == 0 && Projectile.ai[1] > 1) {
            //    for (int i = 0; i < 4; i++) {
            //        int num = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, DustID.Crimson, 0f, 0f, 50, Color.Black, 1f);
            //        Main.dust[num].velocity *= 0f;
            //        Main.dust[num].noGravity = true;
            //    }
            //}
        }

        public override Color? GetAlpha(Color lightColor) {
            return base.GetAlpha(lightColor);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnKill(int timeLeft) {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            //基本爆炸粒子
            //for (int i = 0; i < 4; i++) {
            //    int num = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, DustID.Crimson, 0f, 0f, 50, Color.White, 0.8f);
            //    Main.dust[num].velocity *= 1f;
            //    if (Main.rand.NextBool(2)) {
            //        Main.dust[num].scale = 0.5f;
            //        Main.dust[num].noGravity = true;
            //        Main.dust[num].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
            //    }
            //}
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "SirenProj1").Value;
            Texture2D texture2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "SirenProj2").Value;
            Texture2D texture3 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "SirenProj3").Value;
            Texture2D texture4 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "SirenProj4").Value;
            Texture2D texture5 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "SirenProj5").Value;

            Texture2D texture = texture1;
            if (projtype == 0) texture = texture1;
            if (projtype == 1) texture = texture2;
            if (projtype == 2) texture = texture3;
            if (projtype == 3) texture = texture4;
            if (projtype == 4) texture = texture5;

            for (int k = 0; k < Projectile.oldPos.Length; k++) {
                if (k != 0) {
                    Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f) + new Vector2(0f, Projectile.gfxOffY);

                    Color color = lightColor * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    Main.EntitySpriteDraw(texture, drawPos, null, color * 0.3f, Projectile.rotation, new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f), Projectile.scale, SpriteEffects.None, 0);
                }

                if (k == 0) {
                    Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f) + new Vector2(0f, Projectile.gfxOffY);

                    Main.EntitySpriteDraw(texture, drawPos, null, lightColor, Projectile.rotation, new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f), Projectile.scale, SpriteEffects.None, 0);
                }
            }

            return false;
        }
    }
}
