using System;
using GloryofGuardian.Common;
using GloryofGuardian.Content.Classes;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;

namespace GloryofGuardian.Content.NPCs.GuardNPCs {
    public class TitaniumDTN : GOGGuardNPC {
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

        public override void AI() {
            AttackPos = NPC.Center + new Vector2(0, 0);

            //浮动
            float1 = (float)Math.Sin(drawcount / 12f) + 1;
            float2 = (float)Math.Sin(drawcount / 12f + MathHelper.PiOver2) + 1;

            foreach (Player player in Main.ActivePlayers) {
                if (Vector2.Distance(DTcenter + Main.screenPosition, player.Center) <= 3000) {
                    player.AddBuff(BuffID.Swiftness, 300);
                    player.AddBuff(BuffID.Ironskin, 300);
                    player.AddBuff(BuffID.Thorns, 300);
                    player.AddBuff(BuffID.WellFed, 300);
                }
            }

            foreach (NPC npc in Main.ActiveNPCs) {
                if (Vector2.Distance(DTcenter + Main.screenPosition, npc.Center) <= 750
                    && npc.ModNPC is GOGGuardNPC npc1) {
                    npc1.Titan = true;
                    npc1.Titancount = 3;
                }
            }

            foreach (Projectile proj in Main.ActiveProjectiles) {
                if (Vector2.Distance(DTcenter + Main.screenPosition, proj.Center) <= 750
                    && proj.ModProjectile is GOGDT gogdt) {
                    gogdt.titanstkspeed =true;
                    gogdt.titancount = 2;
                }
            }

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
        Vector2 DTcenter = Vector2.Zero;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.GuardNPCs + "TitaniumDTN").Value;
            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.GuardNPCs + "TitaniumDTN2").Value;
            Texture2D texture2 = ModContent.Request<Texture2D>(GOGConstant.GuardNPCs + "TitaniumDTN3").Value;
            Texture2D texture3 = ModContent.Request<Texture2D>(GOGConstant.GuardNPCs + "TitaniumDTN4").Value;

            Texture2D textures = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "GradientRing3000").Value;
            Texture2D textures2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "GradientRing3000P").Value;

            Vector2 drawPosition0 = NPC.Center - Main.screenPosition;

            DTcenter = drawPosition0 + new Vector2(-0, -48) + new Vector2(0, -4 * float1);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            Main.EntitySpriteDraw(textures, DTcenter, null, new Color(130, 130, 130) * 0.4f, NPC.rotation, textures.Size() * 0.5f, NPC.scale * 0.5f, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            //有一个图层低于炮台本体的部分
            Main.EntitySpriteDraw(texture1, drawPosition0 + new Vector2(14, -4) + new Vector2(0, 4 * float1), null, Color.White, NPC.rotation, texture0.Size() * 0.5f, NPC.scale, SpriteEffects.None, 0);
            {
                Main.EntitySpriteDraw(texture0, drawPosition0, null, Color.White, NPC.rotation, texture0.Size() * 0.5f, NPC.scale, SpriteEffects.None, 0);
            }
            Main.EntitySpriteDraw(texture2, drawPosition0 + new Vector2(33, -32) + new Vector2(0, -4 * float2), null, Color.White, NPC.rotation, texture0.Size() * 0.5f, NPC.scale * 1.2f, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture3, drawPosition0 + new Vector2(14, -4) + new Vector2(0, 4 * float1), null, Color.White, NPC.rotation, texture0.Size() * 0.5f, NPC.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
