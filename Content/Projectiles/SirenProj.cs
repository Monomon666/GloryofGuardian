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
            Projectile.timeLeft = 1200;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 20;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;//穿透数，1为攻击到第一个敌人就消失
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
            projtype = (int)Projectile.ai[0];
            if (projtype == 2) spawnpos = Projectile.Center;
        }

        int count = 0;
        int countdust = 0;
        public override void AI() {
            count++;

            Projectile.rotation = Projectile.velocity.ToRotation();
            //飞
            if (projtype == 0) {
                if (count <= 120) Projectile.velocity *= 0.98f;

                if (count > 180) Projectile.alpha += 20;
                if (Projectile.alpha > 255) Projectile.Kill();
            }//散
            if (projtype == 1) {
                if (count > 180) Projectile.alpha += 20;
                if (Projectile.alpha > 255) Projectile.Kill();
            }
            //转
            if (projtype == 2) {
                if (count >= 30) {
                    Projectile.velocity *= 0;
                    Projectile.Center += (Projectile.Center - spawnpos).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * 6f;

                    if (count > 180) Projectile.alpha += 5;
                    if (Projectile.alpha > 255) Projectile.Kill();
                }
            }
            //吸
            if (projtype == 3) {
                if (count <= 60) Projectile.velocity *= 0.98f;
                if (count > 60) {
                    countdust++;
                    if (countdust >= 60) countdust = 0;

                    for (int i = 0; i <= 8; i++) {
                        Dust dust1 = Dust.NewDustDirect(Projectile.Center + Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * (60 - countdust) * 4, 8, 8, DustID.SnowSpray, 1f, 1f, 100, Color.Blue, 0.8f);
                        dust1.velocity *= 0;
                        dust1.noGravity = true;
                    }

                    foreach (NPC npc in Main.npc) {
                        if (npc != null && npc.active && !npc.boss && npc.knockBackResist > 0 && Vector2.Distance(Projectile.Center, npc.Center) < 240) {
                            npc.velocity *= 0.99f;
                            npc.velocity += npc.Center.Toz(Projectile.Center) * 0.1f;
                            npc.Center += npc.Center.Toz(Projectile.Center) * 1f;
                        }
                    }
                }

                if (count > 60) Projectile.alpha += 1;
                if (Projectile.alpha > 255) Projectile.Kill();
            }
            //追
            if (projtype == 4) {
                if (count > 60) {
                    NPC target1 = Projectile.Center.InPosClosestNPC(1200, true, true);
                    if (target1 != null && target1.active) Projectile.ChasingBehavior(target1.Center, 6f, 16);
                }
            }
        }

        public override Color? GetAlpha(Color lightColor) {
            return base.GetAlpha(lightColor);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            if (projtype == 1) Projectile.velocity *= 0.6f;
            if (projtype == 4) Projectile.Kill();
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
            if (projtype == 2) texture = texture5;
            if (projtype == 3) texture = texture3;
            if (projtype == 4) texture = texture4;

            for (int k = 0; k < Projectile.oldPos.Length; k++) {
                if (k != 0) {
                    Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f) + new Vector2(0f, Projectile.gfxOffY);

                    Color color = lightColor * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) * ((255 - Projectile.alpha) / 255f);
                    Main.EntitySpriteDraw(texture, drawPos, null, color * 0.3f, Projectile.rotation, new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f), Projectile.scale, SpriteEffects.None, 0);
                }

                if (k == 0) {
                    Vector2 drawPos = (Projectile.oldPos[k] - Main.screenPosition) + new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f) + new Vector2(0f, Projectile.gfxOffY);

                    Main.EntitySpriteDraw(texture, drawPos, null, lightColor * ((255 - Projectile.alpha) / 255f), Projectile.rotation, new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f), Projectile.scale, SpriteEffects.None, 0);
                }
            }

            return false;
        }
    }
}
