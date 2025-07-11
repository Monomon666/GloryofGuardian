using System;
using System.Collections.Generic;
using GloryofGuardian.Common;
using GloryofGuardian.Content.Buffs;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles {
    public class Corrupt2RainProj : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.localNPCHitCooldown = 0;
            Projectile.penetrate = 1;
        }

        Player Owner => Main.player[Projectile.owner];

        public override void AI() {
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.Pi;

            if (Projectile.ai[0] == 2) {
                if (count % 1 == 0) {
                    for (int i = 0; i < 1; i++) {
                        int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GreenFairy, 0f, 0f, 50, Color.White, 1f);
                        Main.dust[num].velocity = Projectile.velocity;
                        Main.dust[num].velocity *= 1f;
                        Main.dust[num].noGravity = true;
                    }
                }
            }

            //腐化伏行
            if (Projectile.ai[0] == 3 || Projectile.ai[0] == 3.5f) {
                Projectile.width = Projectile.height = 8;

                Drop();

                if (count % 1 == 0) {
                    for (int i = 0; i < 4; i++) {
                        int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.GreenFairy, 0f, 0f, 50, Color.White, 1f);
                        Main.dust[num].velocity *= 0f;
                        Main.dust[num].noGravity = true;
                    }
                }

                if (Projectile.ai[0] == 3 && count % 5 == 0) {
                    int num = count / 30;
                    int damage1 = (int)(Projectile.damage * ((13 - num) / 12f));

                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center + new Vector2(0, 16), new Vector2(0, -4), ModContent.ProjectileType<Corrupt2RainProj>(), damage1, 6, Owner.whoAmI, 4, num);
                    proj1.penetrate = -1;
                    proj1.localNPCHitCooldown = 30;
                }

                if (count > 40) Projectile.Kill();
            }

            //腐化柱子
            if (Projectile.ai[0] == 4) {
                Projectile.width = Projectile.height = 16;

                height0 = (int)(80 - 40 * Projectile.ai[1]);
                Projectile.velocity *= 0;

                float dying = count / 20f;

                if (1 == 1) {
                    if (count % 1 == 0) {
                        for (int i = 0; i < 4; i++) {
                            int num = Dust.NewDust(Projectile.position + new Vector2(0, Projectile.height - height0 * dying - 16), 16, 32, DustID.GreenFairy, 0f, 0f, 50, Color.Yellow, 1f);
                            Main.dust[num].velocity = new Vector2(0, -4);
                            Main.dust[num].noGravity = true;
                        }
                    }

                    if (count % 1 == 0) {
                        for (int i = 0; i < 1; i++) {
                            int num = Dust.NewDust(Projectile.position + new Vector2(0, Projectile.height - height0 * dying - 16), 16, 32, DustID.CursedTorch, 0f, 0f, 50, Color.White, 2f);
                            Main.dust[num].velocity = new Vector2(0, -2);
                            Main.dust[num].noGravity = true;
                        }
                    }
                }

                if (count > 20) Projectile.Kill();
            }

            base.AI();
        }

        bool drop = true;
        int height0 = 0;
        /// <summary>
        /// 坠落
        /// </summary>
        void Drop() {
            Projectile.velocity.Y += 0.2f;
            if (Projectile.velocity.Y > 8f) {
                Projectile.velocity.Y = 8f;
            }

            Vector2 droppos = Projectile.Bottom;
            if (drop) {
                int maxdropdis = 5000;
                for (int y = 0; y < maxdropdis; y++) {
                    Tile tile0 = TileHelper.GetTile(GOGUtils.WEPosToTilePos(droppos + new Vector2(0, y) * 16));
                    if (tile0.HasTile) {
                        Projectile.Bottom = (droppos + new Vector2(0, y - 1) * 16);
                        break;
                    }
                }
            }
        }

        bool boom = false;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            target.AddBuff(ModContent.BuffType<CorruptSeedbedDebuff>(), 180);
            if (Projectile.ai[0] == 2) target.AddBuff(BuffID.CursedInferno, 180);

            if (Projectile.ai[0] == 2 &&
                TileHelper.FindTilesInRectangle(target.Center + new Vector2(target.width / 2, target.height), 3, 3, 1, 2, true) != null) {

                for (int i = 0; i < 1; i++) {
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.NPCHit9, Projectile.Center);
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), target.Center + new Vector2(target.width / 2, target.height), new Vector2(4, 0), ModContent.ProjectileType<Corrupt2RainProj>(), Projectile.damage / 2, 4, Owner.whoAmI, 3);
                    Projectile proj3 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), target.Center + new Vector2(target.width / 2, target.height), new Vector2(-4, 0), ModContent.ProjectileType<Corrupt2RainProj>(), Projectile.damage / 2, 4, Owner.whoAmI, 3);
                    proj1.tileCollide = proj3.tileCollide = false;
                    proj1.localNPCHitCooldown = proj3.localNPCHitCooldown = -1;
                }
            }

            if (Projectile.ai[0] == 2 &&
                TileHelper.FindTilesInRectangle(target.Center + new Vector2(target.width / 2, target.height), 3, 3, 1, 2, true) == null) {
                boom = true;
            }

            if (Projectile.ai[0] == 3) {
                for (int i = 0; i < 25; i++) {
                    int num = Dust.NewDust(target.position, target.width, target.height, DustID.GreenFairy, 0f, 0f, 50, Color.Yellow, 1.5f);
                    Main.dust[num].velocity = new Vector2(Main.dust[num].position.Toz(target.Center).X , -4);
                    Main.dust[num].noGravity = true;
                }
            }

            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            if (Projectile.ai[0] == 2) {
                for (int i = 0; i < 1; i++) {
                    Terraria.Audio.SoundEngine.PlaySound(SoundID.NPCHit9, Projectile.Center);
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center, new Vector2(2, 0), ModContent.ProjectileType<Corrupt2RainProj>(), Projectile.damage / 2, 4, Owner.whoAmI, 3.5f);
                    Projectile proj3 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center, new Vector2(-2, 0), ModContent.ProjectileType<Corrupt2RainProj>(), Projectile.damage / 2, 4, Owner.whoAmI, 3.5f);
                    proj1.tileCollide = proj3.tileCollide = false;
                    proj1.localNPCHitCooldown = proj3.localNPCHitCooldown = -1;
                }
            }
            
            return base.OnTileCollide(oldVelocity);
        }

        public override void OnKill(int timeLeft) {
            if (boom) {
                //爆炸
                for (int j = 0; j < 60; j++) {
                    int num1 = Dust.NewDust(Projectile.Center, 0, 0, DustID.GreenFairy, 0f, 0f, 10, Color.Yellow, 1.7f);
                    Main.dust[num1].velocity = new Vector2(0, -1 * Main.rand.NextFloat(0.8f, 1.2f)).RotatedBy(MathHelper.Pi / 30f * j);
                    Main.dust[num1].noGravity = true;
                    Main.dust[num1].velocity *= 8f;
                }

                SoundEngine.PlaySound(in SoundID.NPCHit9, Projectile.Center);
                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = (int)(120 * Projectile.scale);
                Projectile.position.X = Projectile.position.X - Projectile.width / 2;
                Projectile.position.Y = Projectile.position.Y - Projectile.height / 2;
                Projectile.maxPenetrate = -1;
                Projectile.penetrate = -1;
                Projectile.usesLocalNPCImmunity = false;
                Projectile.usesIDStaticNPCImmunity = true;
                Projectile.idStaticNPCHitCooldown = 0;
                Projectile.Damage();
            }
            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "Corrupt2RainProj0").Value;
            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "Corrupt2RainProj").Value;
            Texture2D texture2 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "Corrupt2RainProj2").Value;

            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "RainShadow").Value;

            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition - Projectile.velocity.SafeNormalize(Vector2.Zero) * texture1.Width / 2;

            if (Projectile.ai[0] == 0 || Projectile.ai[0] == 1) {
                if (Main.dayTime) {
                    Main.EntitySpriteDraw(texture, drawPosition0, null, new Color(0, 0, 0), Projectile.rotation,
                    texture.Size() * 0.5f,
                    Projectile.scale, SpriteEffects.None, 0);
                }

                Main.EntitySpriteDraw(texture1, drawPosition0, null, new Color(70, 0, 161, 0) * 2, Projectile.rotation,
                    texture1.Size() * 0.5f,
                    Projectile.scale, SpriteEffects.None, 0);
            }

            if (Projectile.ai[0] == 2) {
                if (Main.dayTime) {
                    Main.EntitySpriteDraw(texture, drawPosition0, null, new Color(0, 0, 0), Projectile.rotation,
                    texture.Size() * 0.5f,
                    Projectile.scale, SpriteEffects.None, 0);
                }

                Main.EntitySpriteDraw(texture1, drawPosition0, null, new Color(3, 239, 2, 0) * 20, Projectile.rotation,
                    texture1.Size() * 0.5f,
                    Projectile.scale, SpriteEffects.None, 0);
            }

            if (Projectile.ai[0] == 3 || Projectile.ai[0] == 4) {
                Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation,
                    texture0.Size() * 0.5f,
                    Projectile.scale * 0.5f, SpriteEffects.None, 0);
            }

            return base.PreDraw(ref lightColor);
        }
    }
}
