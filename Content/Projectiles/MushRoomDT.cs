using System;
using System.Collections.Generic;
using GloryofGuardian.Common;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles {
    public class MushRoomDT : GOGDT {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 42;
            Projectile.friendly = true;//本炮塔具有碰撞伤害
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
            Projectile.penetrate = -1;

            OtherHeight = 32;

            count0 = 60;

            findboss = true;
            exdust = DustID.MushroomSpray;
        }

        Player Owner => Main.player[Projectile.owner];

        public override void AI() {
            //环境加成
            if (!Main.dayTime) Projectile.ai[1] = 10;
            othercrit = Owner.ZoneGlowshroom ? 25 : 0;

            if (!Main.dayTime || Owner.ZoneGlowshroom) {
                Lighting.AddLight(Projectile.Center, 1.5f, 1.5f, 1.5f);

                if (count % 1 == 0) {
                    for (int i = 0; i < 1; i++) {
                        float angle = Main.rand.NextFloat(0, MathHelper.Pi * 2);
                        int num = Dust.NewDust(Projectile.Center + new Vector2(-6, -16) + new Vector2(0, -16).RotatedBy(angle), 0, 0, DustID.MushroomTorch, 0f, 0f, 50, Color.White, 2f);
                        Main.dust[num].velocity = new Vector2(0, -1.5f).RotatedBy(angle);
                        Main.dust[num].velocity *= 1f;
                        Main.dust[num].noGravity = true;
                    }
                }
            }

            //逐渐产生粒子,直到产生完全的粒子
            if (count >= 120) {
                //包围圈和内部粒子
                if (count % 1 == 0) {
                    for (int i = 0; i < 4; i++) {
                        float angle = Main.rand.NextFloat(0, MathHelper.Pi * 2);
                        int num = Dust.NewDust(Projectile.Center + new Vector2(-6, -16) + new Vector2(0, -160).RotatedBy(angle), 0, 0, DustID.MushroomTorch, 0f, 0f, 50, Color.White, 3f);
                        Main.dust[num].velocity = new Vector2(0, -1.5f).RotatedBy(angle + MathHelper.PiOver2);
                        Main.dust[num].velocity *= 1f;
                        Main.dust[num].noGravity = true;
                        Main.dust[num].scale *= Main.rand.NextFloat(0.5f, 1);
                    }
                }
                if (count % 1 == 0) {
                    for (int i = 0; i < 3; i++) {
                        float angle = Main.rand.NextFloat(0, MathHelper.Pi * 2);
                        int num = Dust.NewDust(Projectile.Center + new Vector2(-6, -16) + new Vector2(0, Main.rand.Next(-160, 0)).RotatedBy(angle), 0, 0, DustID.MushroomTorch, 0f, 0f, 50, Color.White, 1f);
                        Main.dust[num].velocity = new Vector2(0, -1.5f).RotatedBy(angle);
                        Main.dust[num].velocity *= 1f;
                        Main.dust[num].noGravity = true;
                    }
                }
            }
            else if (count < 120) {
                //包围圈和内部粒子
                if (count % 1 == 0) {
                    for (int i = 0; i < (count / 40) + 1; i++) {
                        float angle = Main.rand.NextFloat(0, MathHelper.Pi * 2);
                        int num = Dust.NewDust(Projectile.Center + new Vector2(-6, -16) + new Vector2(0, 20 - count * 1.5f).RotatedBy(angle), 0, 0, DustID.MushroomTorch, 0f, 0f, 50, Color.White, 3f);
                        Main.dust[num].velocity = new Vector2(0, -1.5f).RotatedBy(angle + MathHelper.PiOver2);
                        Main.dust[num].velocity *= 1f;
                        Main.dust[num].noGravity = true;
                        Main.dust[num].scale *= Main.rand.NextFloat(0.5f, 1);
                    }
                }
                if (count % 1 == 0) {
                    for (int i = 0; i < 3; i++) {
                        float angle = Main.rand.NextFloat(0, MathHelper.Pi * 2);
                        int num = Dust.NewDust(Projectile.Center + new Vector2(-6, -16) + new Vector2(0, Main.rand.Next(-40 - count, 0)).RotatedBy(angle), 0, 0, DustID.MushroomTorch, 0f, 0f, 50, Color.White, 1f);
                        Main.dust[num].velocity = new Vector2(0, -1.5f).RotatedBy(angle);
                        Main.dust[num].velocity *= 1f;
                        Main.dust[num].noGravity = true;
                    }
                }
            }

            //额外的持续攻击
            {
                int range = 0;
                if (count < 120) range = 44 + count;
                if (count >= 120) range = 164;

                // 遍历并选取 NPC
                if (count % 10 == 0) {
                    int dotdamage = (Main.rand.Next(100) >= Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) ? 1 : 2;
                    for (int i = 0; i < Main.maxNPCs; i++) {
                        NPC target = Main.npc[i];
                        if (target != null && target.active) {
                            if (Vector2.Distance(Projectile.Center + new Vector2(-6, -16), Main.npc[i].Center) < range) {
                                target.life -= dotdamage;
                                CombatText.NewText(target.Hitbox,
                                Color.Orange,
                                dotdamage,
                                false,
                                true
                                );
                            }
                        }
                    }
                }
            }

            AttackPos = Projectile.Center + new Vector2(-4, -30);

            //todo 粒子动画
            base.AI();
        }

        //判定并非一整次攻击,而是
        protected override List<Projectile> Attack1() {
            List<Projectile> projlist = new List<Projectile>();
            return projlist;
        }

        protected override List<Projectile> Attack2() {
            List<Projectile> projlist = new List<Projectile>();

            if (count >= 120) {
                // 遍历并选取 NPC
                List<NPC> mushroomnpc = new List<NPC>();

                if (count % 10 == 0) {
                    for (int i = 0; i < Main.maxNPCs; i++) {
                        NPC target = Main.npc[i];
                        if (target != null && target.active) {
                            if (Vector2.Distance(Projectile.Center + new Vector2(-6, -16), Main.npc[i].Center) < 164) {
                                mushroomnpc.Add(target);
                            }
                        }
                    }

                    if (mushroomnpc != null && mushroomnpc.Count != 0) {
                        Random random = new Random();
                        int randomIndex = random.Next(mushroomnpc.Count);

                        if (mushroomnpc[randomIndex] != null && mushroomnpc[randomIndex].active) {
                            Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), mushroomnpc[randomIndex].Center + new Vector2(Main.rand.Next(-16, 16), Main.rand.Next(-16, 16)), Vector2.Zero, ModContent.ProjectileType<MushRoomProj>(), lastdamage, 8, Owner.whoAmI);
                        }
                    }
                }
            }
            return null;
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            for (int i = 0; i < 6; i++) {
                float angle = Main.rand.NextFloat(0, MathHelper.Pi * 2);
                int num = Dust.NewDust(target.Center, 0, 0, DustID.MushroomTorch, 0f, 0f, 50, Color.White, 3f);
                Main.dust[num].velocity *= 2f;
                Main.dust[num].noGravity = true;
            }
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            if (
                (count < 120 && GOGUtils.CircularHitboxCollision(Projectile.Center + new Vector2(-6, -16), 44 + count, targetHitbox))
                || (count >= 120 && GOGUtils.CircularHitboxCollision(Projectile.Center + new Vector2(-6, -16), 164, targetHitbox))
                ){
                return true;
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "MushRoomDT").Value;
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "MushRoomDT2").Value;
            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, -16);

            Main.EntitySpriteDraw
                    ((!Main.dayTime || Owner.ZoneGlowshroom) ? texture : texture0,
                    drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
