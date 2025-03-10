using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class SRFrostProj0 : GOGProj
    {
        public override string Texture => GOGConstant.nulls;

        public override void SetStaticDefaults() {
            //残影机制
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults() {
            //这里的尺寸对应的是碰撞体积
            Projectile.width = 26;
            Projectile.height = 26;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.timeLeft = 600;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;//穿透数，1为攻击到第一个敌人就消失
            Projectile.tileCollide = false;

            Projectile.light = 1f;

            Projectile.scale *= 1f;
        }

        Player Owner => Main.player[Projectile.owner];

        Vector2 OwnerPos => Owner.Center;

        public override void OnSpawn(IEntitySource source) {
        }

        int count = 0;
        bool onhit = false;
        public override void AI() {
            count++;
            Projectile.rotation += 0.1f;

            NPC target1 = Projectile.Center.InPosClosestNPC(1600, true, true);
            if (!onhit && target1 != null && target1.active) Projectile.ChasingBehavior(target1.Center, 12f, 16);

            if (!onhit) {
                if (count % 1 == 0) {
                    for (int i = 0; i < 1; i++) {
                        int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SnowSpray, 0f, 0f, 10, Color.White, 1f);
                        Main.dust[num2].velocity *= 0f;
                        Main.dust[num2].noGravity = false;
                    }

                    for (int i = 0; i < 1; i++) {
                        int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.YellowStarDust, 0f, 0f, 10, Color.White, 1f);
                        Main.dust[num2].velocity *= 0f;
                        Main.dust[num2].noGravity = false;
                    }
                }
            }

            if (onhit) {
                if (Projectile.ai[0] == 1) Projectile.Center = tar0.Center + totar * (1 - count / 60f);

                if (count % 20 == 0) {
                    //Vector2 pos = tar0.Center + new Vector2(Main.rand.NextFloat(-64, 64), Main.rand.NextFloat(-64, 64));
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center, Vector2.UnitY.RotatedBy(Main.rand.NextFloat(-MathHelper.Pi, MathHelper.Pi)) * 1f, ModContent.ProjectileType<SRFrostProj>(), Projectile.damage, 6, Owner.whoAmI, 1);
                    proj1.penetrate = 5;
                    proj1.tileCollide = false;
                    proj1.timeLeft = 90;
                    proj1.extraUpdates = 2;
                    if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                        if (proj1.ModProjectile is GOGProj proj2) {
                            proj2.OrichalcumMarkProj = true;
                            proj2.OrichalcumMarkProjcount = 300;
                        }
                    }
                }

                if (Projectile.ai[1] != 0 && count == 30) {
                    if (Projectile.ai[1] == 1) {
                        for (int i = 0; i < 4; i++) {
                            float vec1 = Projectile.ai[1] == 1 ? 0 : MathHelper.PiOver4;
                            Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center, Vector2.UnitY.RotatedBy(MathHelper.PiOver2 * i + vec1) * 8f, ModContent.ProjectileType<SRFrostProj>(), Projectile.damage, 6, Owner.whoAmI, Projectile.ai[1] + 1);
                            proj1.penetrate = -1;
                            if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                                if (proj1.ModProjectile is GOGProj proj2) {
                                    proj2.OrichalcumMarkProj = true;
                                    proj2.OrichalcumMarkProjcount = 300;
                                }
                            }
                        }
                    }
                    if (Projectile.ai[1] == 2) {
                        for (int i = 0; i < 8; i++) {
                            Vector2 pos = tar0.Center + new Vector2(0, -320).RotatedBy(MathHelper.PiOver4 * i);
                            float vec1 = Projectile.ai[1] == 1 ? 0 : MathHelper.PiOver4;
                            Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), pos, Vector2.Zero, ModContent.ProjectileType<SRFrostProj>(), Projectile.damage, 6, Owner.whoAmI, Projectile.ai[1] + 1);
                            if (Projectile.ModProjectile is GOGDT proj0 && proj0.OrichalcumMarkDT) {
                                if (proj1.ModProjectile is GOGProj proj2) {
                                    proj2.OrichalcumMarkProj = true;
                                    proj2.OrichalcumMarkProjcount = 300;
                                }
                            }
                        }
                    }
                }
                if (count > 60) Projectile.Kill();
            }
        }

        public override Color? GetAlpha(Color lightColor) {
            return base.GetAlpha(lightColor);
        }

        NPC tar0;
        Vector2 totar = Vector2.Zero;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            if (tar0 == null || !tar0.active) {
                tar0 = target;
                totar = target.Center.To(Projectile.Center);
                Projectile.velocity *= 0;
                Projectile.damage = 1;
            }

            if (target.HasBuff(BuffID.Frostburn)) {
                target.AddBuff(BuffID.Frostburn2, 180);
            }

            //常态
            target.AddBuff(BuffID.Frostburn, 180);

            onhit = true;
            count = 2;

            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnKill(int timeLeft) {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            //爆炸
            for (int j = 0; j < 52; j++) {
                int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.SnowflakeIce, 0, 0, 0, Color.White, 1f);
                Main.dust[num2].noGravity = true;
                Main.dust[num2].velocity = new Vector2((float)Math.Sin(j * 12 / 100f), (float)Math.Cos(j * 12 / 100f)) * Main.rand.NextFloat(6f, 7f);
            }
            SoundEngine.PlaySound(in SoundID.NPCHit3, Projectile.Center);
            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 120;
            Projectile.position.X = Projectile.position.X - Projectile.width / 2;
            Projectile.position.Y = Projectile.position.Y - Projectile.height / 2;
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = false;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 0;
            Projectile.Damage();

            Projectile.damage = 0;

            //基本爆炸粒子
            for (int i = 0; i < 12; i++) {
                int num = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, DustID.SnowflakeIce, 0f, 0f, 50, Color.White, 0.5f);
                if (Main.rand.NextBool(2)) {
                    Main.dust[num].scale = 1f;
                    Main.dust[num].noGravity = true;
                    Main.dust[num].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
                int num2 = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, DustID.SnowSpray, 0f, 0f, 50, Color.White, 0.5f);
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "SRFrostProj1").Value;

            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0, 0);

            for (int k = 0; k < Projectile.oldPos.Length; k++) {
                Vector2 olddrawPos = (Projectile.oldPos[k] - Main.screenPosition) + new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f) + new Vector2(0f, Projectile.gfxOffY);
                Color color = lightColor * ((Projectile.oldPos.Length - k) / (float)Projectile.oldPos.Length) * 0.5f;

                if (k != 0) {
                    Main.EntitySpriteDraw(texture, olddrawPos, null, color, Projectile.rotation, new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f), Projectile.scale, SpriteEffects.None, 0);
                }

                if (k == 0) {
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
                    Main.EntitySpriteDraw(texture, olddrawPos, null, Color.White * 2f, Projectile.rotation, new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f), Projectile.scale, SpriteEffects.None, 0);
                    Main.spriteBatch.End();
                    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                }
            }

            return false;
        }
    }
}
