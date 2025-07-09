using System;
using System.Collections.Generic;
using GloryofGuardian.Common;
using GloryofGuardian.Content.Buffs;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles {
    public class BeeHiveDT : GOGDT {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 24;
            Projectile.friendly = false;
            Projectile.penetrate = -1;

            OtherHeight = 46;

            count0 = 2;

            findboss = true;
            exdust = DustID.Honey;
        }

        Player Owner => Main.player[Projectile.owner];
        Vector2 AttackPos2 = Vector2.Zero;

        public override void AI() {
            AttackPos = Projectile.Center + new Vector2(-3, -24);

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
                if (Main.rand.NextBool(4)) dust1.velocity = dust1.position.Toz(Projectile.Center) * 2f;
                dust1.noGravity = true;
            }

            base.AI();
        }

        protected override List<Projectile> Attack1() {
            List<Projectile> projlist = new List<Projectile>();

            foreach (Player player in Main.ActivePlayers) {
                if (Vector2.Distance(Projectile.Center, player.Center) <= 120) {
                    if (player.statLife < player.statLifeMax2) {

                        if (!player.HasBuff<HoneyCDbuff>()) {
                            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item4, Projectile.Center);
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
                if (Vector2.Distance(Projectile.Center, player.Center) <= 1200
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

            FinishAttack = true;
            return projlist;
        }

        protected override List<Projectile> Attack2() {
            List<Projectile> projlist = new List<Projectile>();

            foreach (Player player in Main.ActivePlayers) {
                if (Vector2.Distance(Projectile.Center, player.Center) <= 120) {
                    if (player.statLife < player.statLifeMax2) {

                        if (!player.HasBuff<HoneyCDbuff>()) {
                            Terraria.Audio.SoundEngine.PlaySound(SoundID.Item4, Projectile.Center);
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
                if (Vector2.Distance(Projectile.Center, player.Center) <= 1200
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

            FinishAttack = true;
            return projlist;
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "BeeHiveDT").Value;
            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, -16);
            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
