using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class PinkSlimeCallProj : GOGProj
    {
        public override string Texture => GOGConstant.Projectiles + Name;

        public override void SetStaticDefaults() {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 6;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults() {
            Projectile.width = 30;
            Projectile.height = 28;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.timeLeft = 1200;
            //Projectile.light = 1.0f;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 0;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 1;

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
        public override void AI() {
            count++;
            if (Projectile.ai[0] == 0 && count <= 60) reboundcount = 3;

            Projectile.rotation += 0.1f;

            Projectile.velocity.Y += 0.35f;
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.White;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            for (int i = 0; i < 8; i++) {
                Vector2 vel = new Vector2(0, -8);
                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), target.Center, vel.RotatedBy(-MathHelper.Pi + MathHelper.PiOver4 * i), ModContent.ProjectileType<PinkSlimeProj>(), Projectile.damage / 3, 1, Owner.whoAmI, 2);
                if (Projectile.ModProjectile is GOGProj proj0 && proj0.OrichalcumMarkProj) {
                    if (proj1.ModProjectile is GOGProj proj2) {
                        proj2.OrichalcumMarkProj = true;
                        proj2.OrichalcumMarkProjcount = 300;
                    }
                }
            }
            Projectile.Kill();
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool TileCollideStyle(ref int width, ref int height, ref bool fallThrough, ref Vector2 hitboxCenterFrac) {
            fallThrough = false;
            return base.TileCollideStyle(ref width, ref height, ref fallThrough, ref hitboxCenterFrac);
        }

        int reboundcount = 0;
        public override bool OnTileCollide(Vector2 oldVelocity) {
            bool willcontinue = false;
            //制造地雷
            Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);
            SoundEngine.PlaySound(SoundID.Item154, Projectile.position);

            for (int i = 0; i < Main.maxProjectiles; i++) {
                Projectile p = Main.projectile[i];
                if (p.active) {//安全性检测
                    if (p.type == ModContent.ProjectileType<SlimeProj0>()) {//判戍卫弹幕
                        if (Vector2.Distance(Projectile.Center, p.Center) < 32) {//判距离
                            reboundcount++;
                            if (Projectile.ai[0] == 1) {
                                Projectile.velocity.X *= 1.05f;
                                Projectile.velocity.Y -= 4f;
                                willcontinue = true;
                            }
                        }
                    }
                }
            }

            if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon) {
                if (Projectile.ai[0] == 0) Projectile.velocity.X = -oldVelocity.X;
                if (Projectile.ai[0] == 1 && !willcontinue) Projectile.velocity.X *= 0f;
                reboundcount--;
            }

            if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon) {
                if (Projectile.ai[0] == 0) Projectile.velocity.Y = -oldVelocity.Y * 0.8f;
                if (reboundcount > 0) {
                    reboundcount--;
                }
                if (reboundcount <= 0) {
                    if (Projectile.velocity.Y <= 0) {
                        //判定过载攻击
                        if (Main.rand.Next(100) >= Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) {
                            Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center, new Vector2(0, 64), ModContent.ProjectileType<PinkSlimeProj0>(), Projectile.damage, 0, Owner.whoAmI, 0);
                            if (Projectile.ModProjectile is GOGProj proj0 && proj0.OrichalcumMarkProj) {
                                if (proj1.ModProjectile is GOGProj proj2) {
                                    proj2.OrichalcumMarkProj = true;
                                    proj2.OrichalcumMarkProjcount = 300;
                                }
                            }
                        }
                        if (Main.rand.Next(100) < Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) {
                            Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center, new Vector2(0, 64), ModContent.ProjectileType<PinkSlimeProj0>(), Projectile.damage, 0, Owner.whoAmI, 1);
                            if (Projectile.ModProjectile is GOGProj proj0 && proj0.OrichalcumMarkProj) {
                                if (proj1.ModProjectile is GOGProj proj2) {
                                    proj2.OrichalcumMarkProj = true;
                                    proj2.OrichalcumMarkProjcount = 300;
                                }
                            }
                        }
                        Projectile.Kill();
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
