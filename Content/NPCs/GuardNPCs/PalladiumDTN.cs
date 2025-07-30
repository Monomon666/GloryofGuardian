using System;
using GloryofGuardian.Common;
using GloryofGuardian.Content.Classes;
using GloryofGuardian.Content.ParentClasses;
using GloryofGuardian.Content.Projectiles;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;

namespace GloryofGuardian.Content.NPCs.GuardNPCs {
    public class PalladiumDTN : GOGGuardNPC {
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

            count0 = 240;
        }

        public override void AI() {
            AttackPos = NPC.Center + new Vector2(0, 0);

            //浮动因子计算
            {
                float1 = (float)Math.Sin(drawcount / 12f) + 1;
                float2 = (float)Math.Sin(drawcount / 12f + MathHelper.PiOver2) + 1;
                float3 = (float)Math.Cos(drawcount / ((float)Gcount / 2) * MathHelper.Pi - MathHelper.PiOver2);
                float4 = (float)Math.Cos(drawcount / ((float)Gcount / 2) * MathHelper.Pi);

                if (drawcount == 0) ringscale = 0.02f;
                if (drawcount >= 0 && drawcount <= 120) ringscale += 0.002f;
                if (drawcount >= 120 && drawcount <= 240) ringscale += 0.001f;
                if (drawcount >= 240 && drawcount <= 360) ringscale += 0.0003f;
                if (drawcount >= 360 && ringscale < 0.4f) ringscale += 0.0001f;

                ringlastscale = 0.25f;
            }

            foreach (Player player in Main.ActivePlayers) {
                if (Vector2.Distance(DTcenter + Main.screenPosition, player.Center) <= 750) {
                    findplayer = true;
                    player.AddBuff(BuffID.RapidHealing, 300);
                    player.AddBuff(BuffID.Lovestruck, 300);

                    player.AddBuff(BuffID.Campfire, 300);
                    player.AddBuff(BuffID.HeartLamp, 300);
                    player.AddBuff(BuffID.StarInBottle, 300);
                    player.AddBuff(BuffID.DryadsWard, 300);
                }
            }

            Gcount = count0;

            base.AI();
        }

        bool findplayer = false;
        protected override void Attack1() {
            Vector2 projcen = DTcenter;
            //发射
            Vector2 velfire = new Vector2(Main.rand.NextFloat(-1, 1), Main.rand.NextFloat(-1, 1)).SafeNormalize(Vector2.Zero) * 16f;

            if (count >= Gcount) {
                for (int i = 0; i <= 4; i++) {
                    foreach (Player player in Main.ActivePlayers) {
                        if (Vector2.Distance(DTcenter + Main.screenPosition, player.Center) <= 1250 * ringscale * 0.62f) {
                            player.statLife += 5;

                            CombatText.NewText(player.Hitbox,
                                    Color.LightGreen,
                                    "+",
                                    false,
                                    false
                                    );
                        }
                    }
                }

                for (int i = 0; i <= 4; i++) {
                    foreach (NPC npc in Main.ActiveNPCs) {
                        if (npc.ModNPC is GOGGuardNPC
                            && Vector2.Distance(DTcenter + Main.screenPosition, npc.Center) <= 1250 * ringscale * 0.62f * 3
                            && npc.life < npc.lifeMax) {
                            npc.life += 20;
                            if (npc.life > npc.lifeMax) npc.life = npc.lifeMax;

                            CombatText.NewText(npc.Hitbox,
                                    Color.LightGreen,
                                    "+",
                                    false,
                                    false
                                    );
                        }
                    }
                }
                FinishAttack = true;
            }
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
        float float4 = 0;
        float ringscale = 0;
        float ringlastscale = 0;
        Vector2 DTcenter = Vector2.Zero;
        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.GuardNPCs + "PalladiumDTN").Value;
            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.GuardNPCs + "PalladiumDTN2").Value;
            Texture2D texture2 = ModContent.Request<Texture2D>(GOGConstant.GuardNPCs + "PalladiumDTN3").Value;
            Texture2D texture3 = ModContent.Request<Texture2D>(GOGConstant.GuardNPCs + "PalladiumDTN4").Value;

            Vector2 drawPosition0 = NPC.Center - Main.screenPosition + new Vector2(0, 2);

            Main.EntitySpriteDraw(texture0, drawPosition0, null, Color.White, 0, texture0.Size() * 0.5f, NPC.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture1, drawPosition0 + new Vector2(-2, -8) + new Vector2(0, -2 * float2), null, Color.White, NPC.rotation, texture0.Size() * 0.5f, NPC.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture2, drawPosition0 + new Vector2(4, -36) + new Vector2(0, -4 * float1), null, Color.White, NPC.rotation, texture0.Size() * 0.5f, NPC.scale * (float1 * 0.05f + 1), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture3, drawPosition0 + new Vector2(-2, -8) + new Vector2(0, -2 * float2), null, Color.White, NPC.rotation, texture0.Size() * 0.5f, NPC.scale, SpriteEffects.None, 0);

            Texture2D textures = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "GradientRing3000").Value;
            Texture2D textures2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "GradientRing3000P").Value;
            DTcenter = drawPosition0 + new Vector2(-0, -48) + new Vector2(0, -4 * float1);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.AnisotropicWrap, DepthStencilState.None, RasterizerState.CullNone, null, Main.GameViewMatrix.ZoomMatrix);

            Main.EntitySpriteDraw(textures, DTcenter, null, new Color(226, 116, 5) * 0.8f, NPC.rotation, textures.Size() * 0.5f, NPC.scale * ringscale * ringlastscale * (float1 * 0.01f + 1), SpriteEffects.None, 0);
            Main.EntitySpriteDraw(textures2, DTcenter, null, new Color(226, 116, 5) * 0.7f, NPC.rotation, textures2.Size() * 0.5f, NPC.scale * ringscale * ringlastscale * (float1 * 0.005f + 1), SpriteEffects.None, 0);

            Main.EntitySpriteDraw(textures, DTcenter, null, new Color(226, 116, 5) * 0.4f, NPC.rotation, textures.Size() * 0.5f, NPC.scale * ringscale * ringlastscale * (float1 * 0.005f + 1) * 6, SpriteEffects.None, 0);

            Main.spriteBatch.End();
            Main.spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);

            return false;
        }
    }
}
