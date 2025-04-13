using GloryofGuardian.Common;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles
{
    public class SoulDementorBookProj : GOGProj
    {
        public override string Texture => GOGConstant.Projectiles + "Wind";

        public override void SetStaticDefaults() {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 0;
        }

        public override void SetDefaults() {
            Projectile.width = 60;
            Projectile.height = 60;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = GuardianDamageClass.Instance;
            Projectile.timeLeft = 1200;
            Projectile.ignoreWater = true;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 8;
            Projectile.aiStyle = -1;
            Projectile.penetrate = -1;

            Projectile.tileCollide = true;

            Projectile.light = 1.5f;
            Projectile.scale *= 0.8f;
        }

        Player Owner => Main.player[Projectile.owner];
        Vector2 OwnerPos => Owner.Center;
        Vector2 ToMou => Main.MouseWorld - OwnerPos;

        public override void OnSpawn(IEntitySource source) {
        }

        int count = 0;
        int mode = 0;
        int r = 0;
        List<int> tar0 = new List<int>();
        public override void AI() {
            count++;
            //Projectile.rotation += Main.rand.NextFloat(0.1f, 0.3f);
            if (count <= 60) Projectile.velocity *= 0.95f;

            if (Projectile.ai[0] == 0 && count % 1 == 0) {
                for (int i = 0; i < 6; i++) {
                    int num = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, DustID.SandstormInABottle, 0f, 0f, 50, Color.Black, 0.8f);
                    Main.dust[num].velocity *= 1f;
                    Main.dust[num].noGravity = true;
                }
            }

            if (Projectile.ai[0] == 1 && count % 1 == 0) {
                for (int i = 0; i < 6; i++) {
                    int num = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, DustID.SandstormInABottle, 0f, 0f, 50, Color.Black, 0.8f);
                    Main.dust[num].velocity *= 1f;
                    Main.dust[num].noGravity = true;
                }
            }

            if (count <= 40) r = count * 6;
            if (count > 40) r = 400 - count * 4;

            if (count <= 40) Projectile.scale = 1 + (count / 40f);
            if (count > 40) Projectile.scale = 2f - (count - 40) / 30f;

            if (count <= 40) Projectile.rotation += 0.2f;
            if (count > 40 && count <= 60) Projectile.rotation += 0.15f;
            if (count > 60) Projectile.rotation += 0.2f;

            for (int i = 0; i <= 32; i++) {
                Dust dust1 = Dust.NewDustDirect(Projectile.Center + Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * r, 8, 8, DustID.Wraith, 1f, 1f, 100, Color.Black, 1f);
                dust1.velocity = dust1.position.Toz(Projectile.Center).RotatedBy(MathHelper.Pi / 2) * 4f;
                dust1.noGravity = true;

                Dust dust2 = Dust.NewDustDirect(Projectile.Center + Main.rand.NextFloat(MathHelper.TwoPi).ToRotationVector2() * r, 8, 8, DustID.Wraith, 1f, 1f, 100, Color.Black, 1f);
                dust2.velocity = dust2.position.Toz(Projectile.Center) * 2f;
                dust2.noGravity = true;
            }

            //摄魂
            if (count > 35) {
                foreach (NPC npc in Main.npc) {
                    if (npc != null && npc.active && npc.knockBackResist > 0 && Vector2.Distance(Projectile.Center, npc.Center) < r) {
                        npc.velocity *= 0.99f;
                        npc.velocity += npc.Center.Toz(Projectile.Center) * 0.1f;
                        npc.Center += npc.Center.Toz(Projectile.Center) * 1f;

                        if (Vector2.Distance(Projectile.Center, npc.Center) > r - 32) {
                            npc.velocity *= 0.99f;
                            npc.velocity += npc.Center.Toz(Projectile.Center) * 0.5f;
                            npc.Center += npc.Center.Toz(Projectile.Center) * 4f;
                        }

                        if (!tar0.Contains(npc.whoAmI)) // 避免重复添加
                        {
                            tar0.Add(npc.whoAmI);
                        }
                    }
                }
            }

            // 遍历列表
            foreach (var npcID in tar0.ToList()) // 使用ToList避免修改集合时出错
            {
                NPC npc = Main.npc[npcID]; // 通过whoAmI获取NPC实例

                if (!npc.active) {
                    Dust dust1 = Dust.NewDustDirect(npc.position, npc.width, npc.height, DustID.Wraith, 1f, 1f, 100, Color.Black, 1f);
                    dust1.velocity = dust1.position.Toz(Projectile.Center).RotatedBy(MathHelper.Pi / 2) * 4f;
                    dust1.noGravity = true;
                }

                // 检查NPC是否死亡
                if (!npc.active || npc.life <= 0) {
                    // 执行A()
                    Owner.statLife += 5;

                    CombatText.NewText(npc.Hitbox,
                                Color.DarkRed,
                                "摄魂夺魄",
                                false,
                                false 
                                );

                    CombatText.NewText(npc.Hitbox,
                                Color.Green,
                                5,
                                false,
                                false 
                                );

                    // 从列表中移除死亡的NPC（可选）
                    tar0.Remove(npcID);
                }
            }

            //夺魄
            foreach (NPC npc in Main.npc) {
                if (npc != null && npc.active && !npc.boss && npc.knockBackResist > 0 && Vector2.Distance(Projectile.Center, npc.Center) < 240) {
                    npc.velocity *= 0.99f;
                    npc.velocity += npc.Center.Toz(Projectile.Center) * 0.1f;
                    npc.Center += npc.Center.Toz(Projectile.Center) * 1f;
                }
            }

            Projectile.alpha -= 5;
            if (Projectile.alpha < 50) {
                Projectile.alpha = 50;
            }

            if (count > 90) Projectile.Kill();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            if (Projectile.damage > Projectile.originalDamage / 2) Projectile.damage = (int)(Projectile.damage * 0.9f);

            if (!target.boss && target.knockBackResist > 0) {
                target.velocity = Projectile.velocity;
            }
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            if (Projectile.ai[0] == 1) return false;
            else return base.OnTileCollide(oldVelocity);
        }

        public override Color? GetAlpha(Color lightColor) {
            return Color.Black;
        }

        public override void OnKill(int timeLeft) {
            if (Projectile.timeLeft > 10) Terraria.Audio.SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
        }

        public override bool PreDraw(ref Color lightColor) {
            DrawAfterimagesCentered(Projectile, ProjectileID.Sets.TrailingMode[Projectile.type], lightColor, 1);

            return false;
        }

        void DrawAfterimagesCentered(Projectile proj, int mode, Color lightColor, int typeOneIncrement = 1, Texture2D texture = null, bool drawCentered = true) {
            if (texture is null)
                texture = ModContent.Request<Texture2D>(Texture).Value;

            int frameHeight = texture.Height / Main.projFrames[proj.type];
            int frameY = frameHeight * proj.frame;
            float scale = proj.scale;
            float rotation = proj.rotation;

            Rectangle rectangle = new Rectangle(0, frameY, texture.Width, frameHeight);
            Vector2 origin = rectangle.Size() / 2f;

            SpriteEffects spriteEffects = SpriteEffects.None;
            if (proj.spriteDirection == -1)
                spriteEffects = SpriteEffects.FlipHorizontally;

            Vector2 centerOffset = drawCentered ? proj.Size / 2f : Vector2.Zero;
            Color alphaColor = proj.GetAlpha(Color.Black);
            switch (mode) {
                // Standard afterimages. No customizable features other than total afterimage count.
                // Type 0 afterimages linearly scale down from 100% to 0% opacity. Their color and lighting is equal to the main projectile's.
                case 0:
                    for (int i = 0; i < proj.oldPos.Length; ++i) {
                        Vector2 drawPos = proj.oldPos[i] + centerOffset - Main.screenPosition + new Vector2(0f, proj.gfxOffY);
                        // DO NOT REMOVE THESE "UNNECESSARY" FLOAT CASTS. THIS WILL BREAK THE AFTERIMAGES.
                        Color color = alphaColor * ((proj.oldPos.Length - i) / (float)proj.oldPos.Length);
                        Main.spriteBatch.Draw(texture, drawPos, new Rectangle?(rectangle), color, rotation, origin, scale, spriteEffects, 0f);
                    }
                    break;

                // Paladin's Hammer style afterimages. Can be optionally spaced out further by using the typeOneDistanceMultiplier variable.
                // Type 1 afterimages linearly scale down from 66% to 0% opacity. They otherwise do not differ from type 0.
                case 1:
                    // Safety check: the loop must increment
                    int increment = Math.Max(1, typeOneIncrement);
                    Color drawColor = alphaColor;
                    int afterimageCount = ProjectileID.Sets.TrailCacheLength[proj.type];
                    float afterimageColorCount = afterimageCount * 1.5f;
                    int k = 0;
                    while (k < afterimageCount) {
                        Vector2 drawPos = proj.oldPos[k] + centerOffset - Main.screenPosition + new Vector2(0f, proj.gfxOffY);
                        // DO NOT REMOVE THESE "UNNECESSARY" FLOAT CASTS EITHER.
                        if (k > 0) {
                            float colorMult = afterimageCount - k;
                            drawColor *= colorMult / afterimageColorCount;
                        }
                        Main.spriteBatch.Draw(texture, drawPos, new Rectangle?(rectangle), drawColor, rotation, origin, scale, spriteEffects, 0f);
                        k += increment;
                    }
                    break;

                // Standard afterimages with rotation. No customizable features other than total afterimage count.
                // Type 2 afterimages linearly scale down from 100% to 0% opacity. Their color and lighting is equal to the main projectile's.
                case 2:
                    for (int i = 0; i < proj.oldPos.Length; ++i) {
                        float afterimageRot = proj.oldRot[i];
                        SpriteEffects sfxForThisAfterimage = proj.oldSpriteDirection[i] == -1 ? SpriteEffects.FlipHorizontally : SpriteEffects.None;

                        Vector2 drawPos = proj.oldPos[i] + centerOffset - Main.screenPosition + new Vector2(0f, proj.gfxOffY);
                        // DO NOT REMOVE THESE "UNNECESSARY" FLOAT CASTS. THIS WILL BREAK THE AFTERIMAGES.
                        Color color = alphaColor * ((proj.oldPos.Length - i) / (float)proj.oldPos.Length);
                        Main.spriteBatch.Draw(texture, drawPos, new Rectangle?(rectangle), color, afterimageRot, origin, scale, sfxForThisAfterimage, 0f);
                    }
                    break;

                default:
                    break;
            }
        }
    }
}
