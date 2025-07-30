using System;
using System.Collections.Generic;
using GloryofGuardian.Common;
using GloryofGuardian.Content.Buffs;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.NPCs.GuardNPCs {
    public class OrichalcumDTN : GOGGuardNPC {
        public override string Texture => GOGConstant.nulls;

        public override void SetStaticDefaults() {
            //Main.npcFrameCount[NPC.type] = 1;
            NPCID.Sets.TrailingMode[NPC.type] = 3;
            NPCID.Sets.TrailCacheLength[NPC.type] = 10;
        }

        public override void SetProperty() {
            NPC.lifeMax = 400;

            if (Main.masterMode)
                NPC.lifeMax *= 3;
            else if (Main.expertMode)
                NPC.lifeMax *= 2;

            NPC.width = 30;
            NPC.height = 50;

            // NPC.aiStyle = NPCAIStyleID.FaceClosestPlayer;
            NPC.aiStyle = -1;

            NPC.noGravity = false;//无重力
            NPC.lavaImmune = false;//免疫岩浆伤害
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;//死亡音效

            count0 = 30;
        }

        Projectile p1 = null;
        public override void AI() {
            AttackPos = NPC.Center + new Vector2(-4, -48);

            //浮动因子
            float1 = (float)Math.Sin(drawcount / 12f) + 1;
            float2 = (float)Math.Sin(drawcount / 12f + MathHelper.Pi) + 1;
            float3 = (float)Math.Sin(drawcount / 12f + MathHelper.PiOver2) + 1;

            ringscale = 0.4f;
            ringlastscale = 0.3f;

            foreach (Projectile proj in Main.ActiveProjectiles) {
                if (Vector2.Distance(DTcenter + Main.screenPosition, proj.Center) <= 190
                    && proj.ModProjectile is GOGProj proj1) {
                    proj1.OrichalcumMark = true;
                    if (proj.extraUpdates == 0) proj.velocity *= 1.1f;
                    if (proj.extraUpdates > 0) proj.extraUpdates += 1;
                    proj.penetrate += 1;
                }
            }

            Gcount = count0;
            base.AI();
        }

        protected override void Attack1() {
            FinishAttack = true;
        }

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone) {
            base.OnHitByItem(player, item, hit, damageDone);
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone) {
            base.OnHitByProjectile(projectile, hit, damageDone);
        }

        public override void OnKill() {
            base.OnKill();
        }

        //浮动因子
        float float1 = 0;
        float float2 = 0;
        float float3 = 0;
        float ringscale = 0;
        float ringlastscale = 0;
        Vector2 DTcenter = Vector2.Zero;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.GuardNPCs + "OrichalcumDTN").Value;
            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.GuardNPCs + "OrichalcumDTN2").Value;
            Texture2D texture2 = ModContent.Request<Texture2D>(GOGConstant.GuardNPCs + "OrichalcumDTN3").Value;
            Texture2D texture3 = ModContent.Request<Texture2D>(GOGConstant.GuardNPCs + "OrichalcumDTN4").Value;

            Vector2 drawPosition0 = NPC.Center - Main.screenPosition;
            Main.EntitySpriteDraw(texture0, drawPosition0 + new Vector2(0, 2), null, Color.White, NPC.rotation, texture0.Size() * 0.5f, NPC.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture1, drawPosition0 + new Vector2(-4, -14) + new Vector2(0, -2 * float2), null, Color.White, NPC.rotation, texture0.Size() * 0.5f, NPC.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture2, drawPosition0 + new Vector2(6, -46) + new Vector2(0, -2 * float1), null, Color.White, NPC.rotation, texture0.Size() * 0.5f, NPC.scale, SpriteEffects.None, 0);

            {
                Main.EntitySpriteDraw(texture2, drawPosition0 + new Vector2(36, -18) + new Vector2(0, -4 * float2), null, Color.White, NPC.rotation, texture0.Size() * 0.5f, NPC.scale * 0.5f, SpriteEffects.None, 0);
                Main.EntitySpriteDraw(texture2, drawPosition0 + new Vector2(-32, -18) + new Vector2(0, -4 * float3), null, Color.White, NPC.rotation, texture0.Size() * 0.5f, NPC.scale * 0.5f, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(texture3, drawPosition0 + new Vector2(-8, -14) + new Vector2(0, -4 * float3), null, Color.White, NPC.rotation, texture0.Size() * 0.5f, NPC.scale, SpriteEffects.None, 0);

            Texture2D textures = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "GradientRing3000").Value;
            Texture2D textures2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "GradientRing3000P").Value;
            DTcenter = drawPosition0 + new Vector2(-0, -48) + new Vector2(0, -4 * float1);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            Main.EntitySpriteDraw(textures, DTcenter, null, new Color(163, 22, 158) * 0.8f * Math.Min(1f, drawcount / 120f), NPC.rotation, textures.Size() * 0.5f, NPC.scale * ringscale * ringlastscale * (float1 * 0.01f + 1), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(textures2, DTcenter, null, new Color(163, 22, 158) * 0.7f * Math.Min(1f, drawcount / 120f), NPC.rotation, textures2.Size() * 0.5f, NPC.scale * ringscale * ringlastscale * (float1 * 0.005f + 1), SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }

    public class OrichalcumProj : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.localNPCHitCooldown = -1;
            Projectile.penetrate = -1;

            Projectile.extraUpdates = 2;
            Projectile.timeLeft = 2;
        }

        Player Owner => Main.player[Projectile.owner];

        public override void AI() {
            Projectile.velocity *= 0;
            base.AI();
        }

        bool hitboss = false;
        NPC hitedboss = null;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            return base.OnTileCollide(oldVelocity);
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            if (GOGUtils.CircularHitboxCollision(Projectile.Center, 120, targetHitbox) && Collision.CanHit(Projectile.position, Projectile.width, Projectile.height, targetHitbox.TopLeft(), targetHitbox.Width, targetHitbox.Height)) {
                return true;
            }
            return false;
        }

        public override void OnKill(int timeLeft) {
            //爆炸
            for (int j = 0; j < 52; j++) {
                int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PinkTorch, 0, 0, 0, Color.White, 3f);
                Main.dust[num1].noGravity = true;
                Main.dust[num1].velocity = new Vector2((float)Math.Sin(j * 12 / 100f), (float)Math.Cos(j * 12 / 100f)) * Main.rand.NextFloat(4f, 5.5f);

                int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.PinkTorch, 0, 0, 0, Color.White, 1.5f);
                Main.dust[num2].noGravity = true;
                Main.dust[num2].velocity = new Vector2((float)Math.Sin(j * 12 / 100f), (float)Math.Cos(j * 12 / 100f)) * Main.rand.NextFloat(5f, 5.5f);
            }

            Projectile.position = Projectile.Center;
            Projectile.width = Projectile.height = 40;
            Projectile.position.X = Projectile.position.X - Projectile.width / 2;
            Projectile.position.Y = Projectile.position.Y - Projectile.height / 2;
            Projectile.maxPenetrate = -1;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = false;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 0;
            Projectile.Damage();
        }

        public override bool PreDraw(ref Color lightColor) {
            return false;
        }
    }
}
