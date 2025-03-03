using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class FlamingJackProj : GOGProj
    {
        public override string Texture => GOGConstant.nulls;

        public override void SetStaticDefaults() {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults() {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.timeLeft = 1200;
            //Projectile.light = 1.0f;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 12;

            Projectile.extraUpdates = 0;
            Projectile.light = 1.5f;
            Projectile.scale *= 0.9f;
        }

        Player Owner => Main.player[Projectile.owner];
        Vector2 OwnerPos => Owner.Center;
        Vector2 ToMou => Main.MouseWorld - OwnerPos;

        public override void OnSpawn(IEntitySource source) {
            if (Projectile.ai[0] == 0) reboundcount = 3;
            if (Projectile.ai[0] == 1) reboundcount = 0;
        }

        int count = 0;
        int mode = 0;
        int jumpnum = 0;
        public override void AI() {
            count++;
            if (Projectile.ai[0] == 0 && count <= 60) reboundcount = 3;
            if (Projectile.ai[0] == 0 && count <= 60) reboundcount0 = 12;

            Projectile.rotation += 0.1f;

            Projectile.velocity.Y += 0.35f;

            if (count % 1 == 0) {
                for (int i = 0; i < 12; i++) {
                    int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Torch, 0f, 0f, 50, Color.White, 1f);
                    Main.dust[num].velocity *= 0f;
                    Main.dust[num].noGravity = true;
                }
            }

            if (reboundcount0 < 0) Projectile.Kill();
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            target.AddBuff(BuffID.OnFire, 180);
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
            fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        int reboundcount0 = 0;
        int reboundcount = 0;
        public override bool OnTileCollide(Vector2 oldVelocity) {
            bool willcontinue = false;
            //制造地雷
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item154, Projectile.position);

            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon) {
                if (Projectile.ai[0] == 0) Projectile.velocity.X = -oldVelocity.X;
                if (Projectile.ai[0] == 1 && !willcontinue) Projectile.velocity.X *= 0f;
                reboundcount--;
                reboundcount0--;
            }

            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon) {
                if (Projectile.ai[0] == 0) Projectile.velocity.Y = -oldVelocity.Y * 0.95f;
                if (reboundcount > 0) {
                    reboundcount--;
                    reboundcount0--;
                }
                if (reboundcount <= 0) {
                    if (Projectile.velocity.Y <= 0) {

                        NPC target1 = Projectile.Center.InPosClosestNPC(800, true, true);
                        if (target1 != null && target1.active) {
                            // 计算水平距离和垂直距离
                            float distanceX = target1.Center.X - Projectile.Center.X;
                            float distanceY = target1.Center.Y - Projectile.Center.Y;

                            // 假设飞行时间为固定值（可以根据需要调整）
                            float time = 60f; // 60 帧（1 秒）

                            // 计算水平速度和垂直速度
                            float velocityX = distanceX / time;
                            float velocityY = (distanceY - 0.5f * 0.35f * time * time) / time;

                            // 设置初速度
                            Vector2 initialVelocity = new Vector2(velocityX, velocityY);
                            Projectile.velocity = initialVelocity;
                        }

                        reboundcount = 3;
                    }
                }

            }
            return false;
        }

        public override void OnKill(int timeLeft) {
            if (Projectile.timeLeft > 10) Terraria.Audio.SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
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
                    0);

            return false;
        }
    }
}
