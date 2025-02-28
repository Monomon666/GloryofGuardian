using GloryofGuardian.Common;
using GloryofGuardian.Content.Buffs;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class CobaltProj2 : GOGProj
    {
        public override string Texture => GOGConstant.Projectiles + Name;

        public override void SetStaticDefaults() {
            //残影机制
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults() {
            //这里的尺寸对应的是碰撞体积
            Projectile.width = 46;
            Projectile.height = 42;
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

        Vector2 originvel;
        public override void OnSpawn(IEntitySource source) {
            originvel = Projectile.velocity;
        }

        int count = 0;
        int countfade = 0;
        int mode = 0;
        public override void AI() {
            count++;

            Projectile.rotation += 0.2f;

            if (Projectile.ai[0] == 0) {
                Projectile.velocity -= originvel * 0.016f;
                if (count >= 60) Projectile.velocity *= 0;
                if (count == 120) Projectile.Kill();
            }

            if (Projectile.ai[0] == 1) {
                Vector2 fromcenter = (new Vector2(Projectile.ai[1], Projectile.ai[2]));
                Vector2 tofrom = (Projectile.Center - fromcenter).SafeNormalize(Vector2.Zero);

                if (count <= 30) Projectile.velocity -= tofrom * 8f * 0.034f;
                if (count >= 30 && count <= 120) Projectile.velocity *= 0;
                if (count <= 120) Projectile.Center += tofrom.RotatedBy(MathHelper.PiOver2).SafeNormalize(Vector2.Zero) * (((Projectile.Center - fromcenter) / tofrom) * 0.05f);
                if (count >= 120) Projectile.velocity += -tofrom * 0.3f;
                if (count == 150) Projectile.Kill();
            }

            //粒子效果
            for (int i = 0; i <= 4; i++) {
                float cirrot = (i % 4) * MathHelper.PiOver2;

                Dust dust1 = Dust.NewDustDirect(Projectile.Center + new Vector2(-4, -14)
                    + new Vector2((float)Math.Cos(Projectile.rotation + cirrot), (float)Math.Sin(Projectile.rotation + cirrot)) * 23f,
                    1, 1, DustID.NorthPole, 1f, 1f, 100, Color.White, 0.8f);
                if (Projectile.ai[0] == 0 && count >= 30 && count <= 120) dust1.velocity = (dust1.position - (Projectile.Center + new Vector2(-6, -12))).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * 2f;
                else if (Projectile.ai[0] == 1 && count >= 30 && count <= 60) dust1.velocity = (dust1.position - (Projectile.Center + new Vector2(-6, -12))).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * 2f;
                else {
                    dust1.velocity *= 0;
                }
                dust1.noGravity = true;
            }
        }

        public override Color? GetAlpha(Color lightColor) {
            return null;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            if (mode == 0) mode = 1;

            target.AddBuff(ModContent.BuffType<CobaltDebuff>(), 300);

            base.OnHitNPC(target, hit, damageDone);
        }

        public override void OnKill(int timeLeft) {
            //爆炸粒子
            for (int j = 0; j < 30; j++) {
                int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.NorthPole, 0f, 0f, 10, Color.White, 1f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].velocity = (Main.dust[num1].position - (Projectile.Center + new Vector2(-6, -12))).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * 6f;

                Main.dust[num1].velocity *= 1f;
            }
            for (int j = 0; j < 15; j++) {
                int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.NorthPole, 0f, 0f, 10, Color.White, 0.8f);
                Main.dust[num2].noGravity = true;
                Main.dust[num2].velocity = (Main.dust[num2].position - (Projectile.Center + new Vector2(-6, -12))).SafeNormalize(Vector2.Zero).RotatedBy(MathHelper.PiOver2) * 6f;

                Main.dust[num2].velocity *= 0.6f;
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "CobaltProj2").Value;
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0, -8);
            //if (Projectile.ai[0] == 0) {
            //    Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f), Projectile.scale, SpriteEffects.None, 0);
            //}
            //if (Projectile.ai[0] == 1) {
            //    Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f), Projectile.scale, SpriteEffects.None, 0);
            //
            //    Main.spriteBatch.End();
            //    Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
            //    Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f), Projectile.scale, SpriteEffects.None, 0);
            //    Main.spriteBatch.End();
            //    Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            //}

            Main.EntitySpriteDraw(texture, drawPos, null, Color.White, Projectile.rotation, new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f), Projectile.scale, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
            Main.EntitySpriteDraw(texture, drawPos, null, Color.White * 2f, Projectile.rotation, new Vector2(texture.Width * 0.5f, Projectile.height * 0.5f), Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}
