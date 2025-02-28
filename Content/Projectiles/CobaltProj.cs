using GloryofGuardian.Common;
using GloryofGuardian.Content.Buffs;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class CobaltProj : GOGProj
    {
        public override string Texture => GOGConstant.Projectiles + Name;

        public override void SetStaticDefaults() {
            //残影机制
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults() {
            //这里的尺寸对应的是碰撞体积
            Projectile.width = 84;
            Projectile.height = 84;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 12;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;//穿透数，1为攻击到第一个敌人就消失
            Projectile.light = 2f;

            Projectile.tileCollide = false;

            Projectile.alpha = 255;

            Projectile.scale *= 1f;
        }

        Player Owner => Main.player[Projectile.owner];

        Vector2 OwnerPos => Owner.Center;

        Vector2 ToMou => Main.MouseWorld - OwnerPos;

        public override void OnSpawn(IEntitySource source) {
        }

        int count = 0;
        int countfade = 0;
        int mode = 0;
        public override void AI() {
            count++;

            if (mode == 0) {
                Projectile.rotation += 0.2f;

                //追踪
                if (Main.npc[(int)Projectile.ai[1]] != null && Main.npc[(int)Projectile.ai[1]].active) {
                    Projectile.ChasingBehavior(Main.npc[(int)Projectile.ai[1]].Center, 16);
                }

                //粒子效果
                for (int i = 0; i <= 32; i++) {
                    float cirrot = (i % 4) * MathHelper.PiOver2;

                    Dust dust1 = Dust.NewDustDirect(Projectile.Center + new Vector2(-6, -12)
                        + new Vector2((float)Math.Cos(Projectile.rotation + cirrot), (float)Math.Sin(Projectile.rotation + cirrot)) * 42f,
                        1, 1, DustID.NorthPole, 1f, 1f, 100, Color.White, 0.8f);
                    dust1.velocity = Projectile.velocity;
                    dust1.noGravity = true;
                }
            }

            if (mode == 1) {
                countfade++;
                Projectile.rotation += 0.3f;
                Projectile.velocity *= 0.8f;

                if (Projectile.ai[0] == 0) {
                    if (countfade % 40 == 0 && Main.rand.NextBool(2)) {
                        Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center + new Vector2(-6, -12), new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1)).SafeNormalize(Vector2.Zero) * 4f, ModContent.ProjectileType<CobaltProj2>(), Projectile.damage, 0, Owner.whoAmI, 0);
                        if (Projectile.ModProjectile is GOGProj proj0 && proj0.OrichalcumMarkProj) {
                            if (proj1.ModProjectile is GOGProj proj2) {
                                proj2.OrichalcumMarkProj = true;
                                proj2.OrichalcumMarkProjcount = 300;
                            }
                        }
                    }

                    if (countfade > 120) Projectile.Kill();
                }

                if (Projectile.ai[0] == 1) {
                    if (countfade == 30) {
                        Vector2 mainvel = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1)).SafeNormalize(Vector2.Zero);
                        for (int i = 0; i < 3; i++) {
                            Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center + new Vector2(-6, -12), mainvel.RotatedBy(i * 2 * (MathHelper.Pi / 3f)) * 8f, ModContent.ProjectileType<CobaltProj2>(), Projectile.damage, 0, Owner.whoAmI, 1, Projectile.Center.X, Projectile.Center.Y);
                            if (Projectile.ModProjectile is GOGProj proj0 && proj0.OrichalcumMarkProj) {
                                if (proj1.ModProjectile is GOGProj proj2) {
                                    proj2.OrichalcumMarkProj = true;
                                    proj2.OrichalcumMarkProjcount = 300;
                                }
                            }
                        }
                    }

                    if (countfade > 120) Projectile.Kill();
                }

                //粒子效果
                if (mode == 0) {
                    for (int i = 0; i <= 12; i++) {
                        Dust dust1 = Dust.NewDustDirect(Projectile.Center + new Vector2(-6, -12)
                            + Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * 42f,
                            1, 1, DustID.NorthPole, 1f, 1f, 100, Color.White, 1f);
                        dust1.velocity = Projectile.velocity;
                        dust1.noGravity = true;
                    }
                }

                if (mode == 1) {
                    for (int i = 0; i <= 2; i++) {
                        Dust dust1 = Dust.NewDustDirect(Projectile.Center + new Vector2(-6, -12)
                            + Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * 42f,
                            1, 1, DustID.NorthPole, 1f, 1f, 100, Color.White, 0.6f);
                        dust1.velocity *= 0;
                        dust1.noGravity = true;
                    }

                    for (int i = 0; i <= 12; i++) {
                        Dust dust1 = Dust.NewDustDirect(Projectile.Center + new Vector2(-6, -12)
                            + Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * 42f,
                            1, 1, DustID.NorthPole, 1f, 1f, 100, Color.White, 1f);
                        dust1.velocity = (dust1.position - (Projectile.Center + new Vector2(-6, -12))).SafeNormalize(Vector2.Zero).RotatedBy(-MathHelper.PiOver2) * 4f;
                        dust1.noGravity = true;
                    }
                }
            }

            base.AI();
        }

        public override Color? GetAlpha(Color lightColor) {
            return null;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            if (Main.npc[(int)Projectile.ai[1]].boss && Main.npc[(int)Projectile.ai[1]] == target) if (mode == 0) mode = 1;
            if (!Main.npc[(int)Projectile.ai[1]].boss) if (mode == 0) mode = 1;

            target.AddBuff(ModContent.BuffType<CobaltDebuff>(), 300);

            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnKill(int timeLeft) {
            //爆炸粒子
            for (int j = 0; j < 30; j++) {
                int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.NorthPole, 0f, 0f, 10, Color.White, 1f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].velocity = (Main.dust[num1].position - (Projectile.Center + new Vector2(-6, -12))).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * 6f;

                Main.dust[num1].velocity *= 2f;
            }
            for (int j = 0; j < 15; j++) {
                int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.NorthPole, 0f, 0f, 10, Color.White, 0.8f);
                Main.dust[num2].noGravity = true;
                Main.dust[num2].velocity = (Main.dust[num2].position - (Projectile.Center + new Vector2(-6, -12))).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * 6f;

                Main.dust[num2].velocity *= 1f;
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "CobaltProj").Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0, -8);

            Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f), Projectile.scale, SpriteEffects.None, 0);

            if (Projectile.ai[0] == 1) {
                Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f), Projectile.scale, SpriteEffects.None, 0);

                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
                Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f), Projectile.scale, SpriteEffects.None, 0);
                Main.spriteBatch.End();
                Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            }
            return false;
        }
    }
}
