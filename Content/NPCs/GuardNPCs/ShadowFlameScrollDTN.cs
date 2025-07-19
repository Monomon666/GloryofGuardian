using System;
using System.Runtime.InteropServices;
using GloryofGuardian.Common;
using GloryofGuardian.Content.Classes;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;

namespace GloryofGuardian.Content.NPCs.GuardNPCs {
    public class ShadowFlameScrollDTN : GOGGuardNPC {
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

            NPC.width = 83;
            NPC.height = 70;

            // NPC.aiStyle = NPCAIStyleID.FaceClosestPlayer;
            NPC.aiStyle = -1;

            NPC.noGravity = false;//无重力
            NPC.lavaImmune = false;//免疫岩浆伤害
            NPC.HitSound = SoundID.NPCHit1;
            NPC.DeathSound = SoundID.NPCDeath1;//死亡音效

            count0 = 2;
        }

        public override void AI() {
            AttackPos = NPC.Center + new Vector2(-2, 24);

            if (drawcount > 120) {
                foreach (Projectile proj in Main.ActiveProjectiles) {
                    // 检测距离是否小于 160，并且 proj 不属于 GOGDT 类
                    if (Vector2.Distance(proj.Center, NPC.Center) < 160
                        && !(proj.ModProjectile is GOGDT)
                        && proj.damage > 1) {
                        // 获取 GlobalProjectile 实例
                        GOGGlobalProj globalProj = proj.GetGlobalProjectile<GOGGlobalProj>();
                        globalProj.ShadowFire = true;
                    }
                }

                foreach (NPC npc0 in Main.npc) {
                    // 检测距离是否小于 160，并且 proj 不属于 GOGDT 类
                    if (!npc0.friendly
                        && Vector2.Distance(npc0.Center, NPC.Center) < 160) {
                        npc0.AddBuff(BuffID.ShadowFlame, 180);
                    }
                }
            }

            for (int i = 0; i <= 4; i++) {
                Dust dust1 = Dust.NewDustDirect(AttackPos, 0, 0, DustID.Shadowflame, 1f, 1f, 100, Color.White, 1f);
                dust1.velocity = Main.rand.NextBool(3) ?
                    new Vector2(0, -4) 
                    : new Vector2(0, -2).RotatedBy(Main.rand.NextFloat(-0.3f, 0.3f));
                dust1.noGravity = true;
            }

            for (int i = 0; i <= (4
                + ((drawcount >= 120) ? 2 : 0)
                + ((drawcount >= 240) ? 2 : 0)
                + ((drawcount >= 360) ? 2 : 0)
                )
                ; i++) {
                float angle = Main.rand.NextFloat(0, MathHelper.Pi * 2);
                Dust dust1 = Dust.NewDustDirect(NPC.Center + new Vector2(-4, -4) + angle.ToRotationVector2() * Main.rand.NextFloat(120, 150), 8, 8, DustID.Shadowflame, 1f, 1f, 100, Color.White, 1.5f);
                dust1.velocity = Main.rand.NextBool(3) ? new Vector2(0, -2) : dust1.position.Toz(NPC.Center).RotatedBy(MathHelper.PiOver2) * 2;
                dust1.noGravity = true;
            }

            //来自绘制的计算
            if (drawcount == 0) ringscale = 0.02f;
            if (drawcount >= 0 && drawcount <= 120) ringscale += 0.002f;
            if (drawcount >= 120 && drawcount <= 240) ringscale += 0.001f;
            if (drawcount >= 240 && drawcount <= 360) ringscale += 0.0003f;
            if (drawcount >= 360 && ringscale < 0.4f) ringscale += 0.0001f;

            base.AI();
        }

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone) {
            base.OnHitByItem(player, item, hit, damageDone);
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone) {
            base.OnHitByProjectile(projectile, hit, damageDone);
        }

        float ringscale = 0;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.GuardNPCs + "ShadowFlameScrollDTN").Value;
            Texture2D texture2 = ModContent.Request<Texture2D>(GOGConstant.GuardNPCs + "ShadowFlameScrollDTN2").Value;
            Vector2 drawPosition0 = NPC.Center - Main.screenPosition + new Vector2(0, 0);
            Vector2 drawPosition1 = NPC.Center - Main.screenPosition + new Vector2(0, (float)(-Math.Sin(drawcount / 20f) * 4f));
            Main.EntitySpriteDraw(texture0, drawPosition0, null, Color.White, NPC.rotation, texture0.Size() * 0.5f, NPC.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture2, drawPosition1, null, Color.White, NPC.rotation, texture2.Size() * 0.5f, NPC.scale, SpriteEffects.None, 0);

            Texture2D textures = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "GradientRing3000").Value;
            Texture2D textures2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "GradientRing3000P").Value;

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);
            
            Main.EntitySpriteDraw(textures, drawPosition1, null, new Color(177, 60, 255) * ringscale * 1.2f, NPC.rotation, textures.Size() * 0.5f, NPC.scale * 0.1f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(textures2, drawPosition1, null, new Color(177, 60, 255) * ringscale * 1.2f, NPC.rotation, textures2.Size() * 0.5f, NPC.scale * 0.1f, SpriteEffects.None, 0);
            
            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}
