using GloryofGuardian.Common;
using System;
using GloryofGuardian.Content.ParentClasses;
using Terraria.ID;
using GloryofGuardian.Content.Buffs;
using System.Collections.Generic;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace GloryofGuardian.Content.NPCs.GuardNPCs {
    public class BeeHiveDTN : GOGGuardNPC {
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
            AttackPos = NPC.Center + new Vector2(-3, -24);

            //环状
            for (int i = 0; i <= 5; i++) {
                float angle = Main.rand.NextFloat(0, MathHelper.Pi * 2);
                Dust dust1 = Dust.NewDustDirect(AttackPos + angle.ToRotationVector2() * Math.Min(120, 40 + drawcount), 8, 8, DustID.Honey, 1f, 1f, 100, Color.White, 1f);
                dust1.velocity *= 0;
                dust1.velocity = new Vector2(0, -1f).RotatedBy(angle);
                dust1.noGravity = true;
            }

            for (int i = 0; i <= 5; i++) {
                float angle = Main.rand.NextFloat(0, MathHelper.Pi * 2);
                Dust dust1 = Dust.NewDustDirect(AttackPos + angle.ToRotationVector2() * (Main.rand.Next(Math.Min(drawcount + 40, 115), 115) + (float)Math.Sin(drawcount / 60f) * 5), 8, 8, DustID.Honey, 1f, 1f, 100, Color.White, 1f);
                dust1.velocity *= 0;
                dust1.velocity = new Vector2(0, 1f).RotatedBy(angle);
                dust1.noGravity = true;
            }

            for (int i = 0; i <= 6; i++) {
                float angle = Main.rand.NextFloat(0, MathHelper.Pi * 2);
                Dust dust1 = Dust.NewDustDirect(AttackPos + angle.ToRotationVector2() * (Main.rand.Next(Math.Min(drawcount * 5, 1200), 1200) + (float)Math.Sin(drawcount / 60f) * 5), 8, 8, DustID.Honey, 1f, 1f, 100, Color.White, 2f);
                dust1.velocity *= 0;
                dust1.velocity = new Vector2(0, 1f).RotatedBy(angle);
                if (Main.rand.NextBool(4)) dust1.velocity = dust1.position.Toz(NPC.Center) * 2f;
                dust1.noGravity = true;
            }

            base.AI();
        }

        protected override void Attack1() {
            List<Projectile> projlist = new List<Projectile>();

            foreach (Player player in Main.ActivePlayers) {
                if (Vector2.Distance(NPC.Center, player.Center) <= 120) {
                    if (player.statLife < player.statLifeMax2) {

                        if (!player.HasBuff<HoneyCDbuff>()) {
                            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item4, NPC.Center);
                            player.statLife += 40;
                            player.AddBuff(ModContent.BuffType<HoneyCDbuff>(), 3600);
                            CombatText.NewText(player.Hitbox,
                            Color.LightGreen,//颜色
                            "40",
                            false,
                            false
                            );
                            for (int i = 0; i <= 5; i++) {
                                Dust dust1 = Dust.NewDustDirect(player.Center, 0, 0, DustID.Honey, 1f, 1f, 100, Color.White, 1.5f);
                                dust1.velocity *= 2;
                                dust1.noGravity = true;
                            }
                        }
                    }
                }
                if (Vector2.Distance(NPC.Center, player.Center) <= 1200
                    && drawcount >= 240) {
                    player.AddBuff(BuffID.Honey, 300);
                    player.AddBuff(BuffID.Regeneration, 300);
                    if (player.HasBuff(BuffID.Poisoned)) {
                        player.ClearBuff(BuffID.Poisoned);
                        for (int i = 0; i <= 5; i++) {
                            Dust dust1 = Dust.NewDustDirect(player.Center, 0, 0, DustID.Honey, 1f, 1f, 100, Color.White, 1.5f);
                            dust1.velocity *= 10;
                            dust1.noGravity = true;
                        }
                    }
                }
            }

            NPC.life += 2;
            CombatText.NewText(NPC.Hitbox,
                            Color.LightGreen,//颜色
                            "2",
                            false,
                            false
                            );

            FinishAttack = true;
        }

        protected override void Attack2() {
            List<Projectile> projlist = new List<Projectile>();

            foreach (Player player in Main.ActivePlayers) {
                if (Vector2.Distance(NPC.Center, player.Center) <= 120) {
                    if (player.statLife < player.statLifeMax2) {

                        if (!player.HasBuff<HoneyCDbuff>()) {
                            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item4, NPC.Center);
                            player.statLife += 60;
                            player.AddBuff(ModContent.BuffType<HoneyCDbuff>(), 3600);
                            CombatText.NewText(player.Hitbox,
                            Color.LightGreen,//颜色
                            "60",
                            false,
                            false
                            );
                            for (int i = 0; i <= 5; i++) {
                                Dust dust1 = Dust.NewDustDirect(player.Center, 0, 0, DustID.Honey, 1f, 1f, 100, Color.White, 1.5f);
                                dust1.velocity *= 2;
                                dust1.noGravity = true;
                            }
                        }
                    }
                }
                if (Vector2.Distance(NPC.Center, player.Center) <= 1200
                    && drawcount >= 240) {
                    player.AddBuff(BuffID.Honey, 300);
                    player.AddBuff(BuffID.Regeneration, 300);
                    if (player.HasBuff(BuffID.Poisoned)) {
                        player.ClearBuff(BuffID.Poisoned);
                        for (int i = 0; i <= 5; i++) {
                            Dust dust1 = Dust.NewDustDirect(player.Center, 0, 0, DustID.Honey, 1f, 1f, 100, Color.White, 1.5f);
                            dust1.velocity *= 10;
                            dust1.noGravity = true;
                        }
                    }
                }
            }

            NPC.life += 2;
            CombatText.NewText(NPC.Hitbox,
                            Color.LightGreen,//颜色
                            "2",
                            false,
                            false
                            );

            FinishAttack = true;
        }

        public override void OnHitByItem(Player player, Item item, NPC.HitInfo hit, int damageDone) {
            base.OnHitByItem(player, item, hit, damageDone);
        }

        public override void OnHitByProjectile(Projectile projectile, NPC.HitInfo hit, int damageDone) {
            base.OnHitByProjectile(projectile, hit, damageDone);
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor) {
            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.GuardNPCs + "BeeHiveDTN").Value;
            Vector2 drawPosition0 = NPC.Center - Main.screenPosition + new Vector2(0, 0);
            Main.EntitySpriteDraw(texture0, drawPosition0, null, Color.White, NPC.rotation, texture0.Size() * 0.5f, NPC.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
