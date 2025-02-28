using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class AdamantiteProj : GOGProj
    {
        public override string Texture => GOGConstant.Projectiles + Name;

        public override void SetStaticDefaults() {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults() {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.timeLeft = 1200;
            //Projectile.light = 1.0f;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 2;

            Projectile.extraUpdates = 0;
            Projectile.light = 1.5f;
            Projectile.scale *= 1f;
        }

        Player Owner => Main.player[Projectile.owner];
        Vector2 OwnerPos => Owner.Center;
        Vector2 ToMou => Main.MouseWorld - OwnerPos;

        public override void OnSpawn(IEntitySource source) {
            if (Projectile.ai[0] == 0) {
                Projectile.width = 2;
                Projectile.height = 2;
            }
            if (Projectile.ai[0] == 1) {
                Projectile.width = 8;
                Projectile.height = 8;
            }
            if (Projectile.ai[2] == 1) Projectile.extraUpdates += 2;
            if (Projectile.ai[2] == 1) Projectile.ArmorPenetration += 10;
            reboundcount = 3;
        }

        int count = 0;
        int mode = 0;
        int reboundcount = 0;
        public override void AI() {
            count++;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        List<int> ignore = new List<int>();
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            reboundcount--;
            if (Projectile.ai[0] == 0) Projectile.Kill();
            if (Projectile.ai[0] == 1) {
                ignore.Add(target.whoAmI);
                NPC target1 = Projectile.Center.InDirClosestNPC(800, false, true, (int)Projectile.ai[1], ignore);
                if (target1 == null) {
                    Projectile.velocity = Projectile.velocity.RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f));
                }
                if (target1 != null) {
                    Projectile.velocity = (target1.Center - Projectile.Center).SafeNormalize(Vector2.Zero).RotatedBy(Main.rand.NextFloat(-0.1f, 0.1f)) * 12f;
                }
            }

            for (int i = 0; i < 1; i++) {
                int num = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, DustID.Firework_Red, 0f, 0f, 50, Color.White, 0.8f);
                Main.dust[num].velocity *= 1f;
            }

            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            if (reboundcount <= 0) return true;
            else {
                reboundcount--;
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
                SoundEngine.PlaySound(SoundID.Dig, Projectile.position);

                if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon) {
                    Projectile.velocity.X = -oldVelocity.X;
                }

                if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon) {
                    Projectile.velocity.Y = -oldVelocity.Y * 0.8f;
                }
                return false;
            }
        }

        public override void OnKill(int timeLeft) {
            if (Projectile.timeLeft > 10) Terraria.Audio.SoundEngine.PlaySound(SoundID.Dig);

            for (int i = 0; i < 1; i++) {
                int num = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, DustID.Firework_Red, 0f, 0f, 50, Color.White, 0.8f);
                Main.dust[num].velocity *= 1f;
                if (Main.rand.NextBool(2)) {
                    Main.dust[num].scale = 0.5f;
                    Main.dust[num].noGravity = true;
                    Main.dust[num].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture0 = null;
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "AdamantiteProj").Value;
            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "AdamantiteProj2").Value;

            if (Projectile.ai[0] == 0) texture0 = texture;
            if (Projectile.ai[0] == 1) texture0 = texture1;

            float scaproj = 1;
            if (Projectile.ai[0] == 0) scaproj = 1.2f;
            if (Projectile.ai[0] == 1) scaproj = 0.8f;

            for (int k = 0; k < Projectile.oldPos.Length; k++) {
                if (k != 0 && Projectile.ai[0] == 1) {
                    Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + texture0.Size() / 2 + new Vector2(0f, Projectile.gfxOffY);

                    Color color = lightColor * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length);
                    Main.EntitySpriteDraw(texture0, drawPos, null, color, Projectile.rotation, texture0.Size() / 2, Projectile.scale * scaproj, SpriteEffects.None, 0);
                }

                if (k == 0) {
                    Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + texture0.Size() / 2 + new Vector2(0f, Projectile.gfxOffY);

                    Main.EntitySpriteDraw(texture0, drawPos, null, lightColor * 3, Projectile.rotation, texture0.Size() / 2, Projectile.scale * scaproj, SpriteEffects.None, 0);
                }
            }

            return false;
        }
    }
}
