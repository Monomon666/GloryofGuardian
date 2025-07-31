using System.Collections.Generic;
using GloryofGuardian.Common;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles {
    public class ChlorophyteDT : GOGDT {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 64;
            Projectile.friendly = true;//本炮塔具有碰撞伤害
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
            Projectile.penetrate = -1;

            OtherHeight = 34;

            count0 = 60;

            exdust = DustID.Chlorophyte;
        }

        Player Owner => Main.player[Projectile.owner];

        public override void AI() {
            AttackPos = Projectile.Center + new Vector2(-4, -30);

            Lighting.AddLight(Projectile.Center, 1.5f, 1.5f, 1.5f);

            if (Main.dayTime || Owner.ZoneJungle) {
                if (drawcount % 1 == 0) {
                    for (int i = 0; i < 1; i++) {
                        float angle = Main.rand.NextFloat(0, MathHelper.Pi * 2);
                        int num = Dust.NewDust(Projectile.Center + new Vector2(-6, -16) + new Vector2(0, -16).RotatedBy(angle), 0, 0, DustID.PoisonStaff, 0f, 0f, 50, Color.White, 0.8f);
                        Main.dust[num].velocity = new Vector2(0, -1.5f).RotatedBy(angle);
                        Main.dust[num].velocity *= 1f;
                        Main.dust[num].noGravity = true;
                    }
                }
            }

            //逐渐产生粒子,直到产生完全的粒子
            if (drawcount >= 120) {
                //包围圈和内部粒子
                if (drawcount % 1 == 0) {
                    for (int i = 0; i < 4; i++) {
                        float angle = Main.rand.NextFloat(0, MathHelper.Pi * 2);
                        int num = Dust.NewDust(Projectile.Center + new Vector2(-6, -16) + new Vector2(0, Main.rand.Next(-250, -230)).RotatedBy(angle), 0, 0, DustID.PoisonStaff, 0f, 0f, 50, Color.Green, 2f);
                        Main.dust[num].velocity = new Vector2(0, -1.5f).RotatedBy(angle + MathHelper.PiOver2);
                        Main.dust[num].velocity *= 1f;
                        Main.dust[num].noGravity = true;
                        Main.dust[num].scale *= Main.rand.NextFloat(0.5f, 1);
                    }
                }
                if (drawcount % 1 == 0) {
                    for (int i = 0; i < 3; i++) {
                        float angle = Main.rand.NextFloat(0, MathHelper.Pi * 2);
                        int num = Dust.NewDust(Projectile.Center + new Vector2(-6, -16) + new Vector2(0, Main.rand.Next(-240, 0)).RotatedBy(angle), 0, 0, DustID.PoisonStaff, 0f, 0f, 50, Color.Green, 1f);
                        Main.dust[num].velocity = new Vector2(0, -1.5f).RotatedBy(angle);
                        Main.dust[num].velocity *= 1f;
                        Main.dust[num].noGravity = true;
                    }
                }
            }
            else if (drawcount < 120) {
                //包围圈和内部粒子
                if (drawcount % 1 == 0) {
                    for (int i = 0; i < (drawcount / 40) + 1; i++) {
                        float angle = Main.rand.NextFloat(0, MathHelper.Pi * 2);
                        int num = Dust.NewDust(Projectile.Center + new Vector2(-6, -16) + new Vector2(0, 20 - drawcount * 2.2f).RotatedBy(angle), 0, 0, DustID.PoisonStaff, 0f, 0f, 50, Color.Green, 3f);
                        Main.dust[num].velocity = new Vector2(0, -1.5f).RotatedBy(angle + MathHelper.PiOver2);
                        Main.dust[num].velocity *= 1f;
                        Main.dust[num].noGravity = true;
                        Main.dust[num].scale *= Main.rand.NextFloat(0.5f, 1);
                    }
                }
                if (drawcount % 1 == 0) {
                    for (int i = 0; i < 3; i++) {
                        float angle = Main.rand.NextFloat(0, MathHelper.Pi * 2);
                        int num = Dust.NewDust(Projectile.Center + new Vector2(-6, -16) + new Vector2(0, Main.rand.Next(-80 - drawcount, 0)).RotatedBy(angle), 0, 0, DustID.PoisonStaff, 0f, 0f, 50, Color.Green, 2f);
                        Main.dust[num].velocity = new Vector2(0, -1.5f).RotatedBy(angle);
                        Main.dust[num].velocity *= 1f;
                        Main.dust[num].noGravity = true;
                    }
                }
            }

            //额外的持续攻击
            {
                int range = 0;
                if (drawcount < 170) range = 80 + drawcount;
                if (drawcount >= 170) range = 250;

                // 遍历并选取 NPC
                if (drawcount % 60 == 0) {
                    if (Main.rand.Next(100) <= Owner.GetCritChance<GenericDamageClass>() + (int)Projectile.ai[1]) {
                        for (int i = 0; i < Main.maxNPCs; i++) {
                            NPC target = Main.npc[i];
                            if (target != null && target.active && !target.friendly) {
                                if (Vector2.Distance(Projectile.Center + new Vector2(-6, -16), Main.npc[i].Center) < range) {
                                    target.life -= Projectile.damage;
                                    CombatText.NewText(target.Hitbox,
                                    Color.Orange,
                                    Projectile.damage,
                                    false,
                                    true
                                    );

                                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), target.Center + new Vector2(Main.rand.Next(-16, 16), Main.rand.Next(-16, 16)), Vector2.Zero, ModContent.ProjectileType<ChlorophyteProj>(), lastdamage, 8, Owner.whoAmI);
                                }
                            }
                        }
                    } 
                }
            }

            //todo 粒子动画
            base.AI();
        }

        protected override List<Projectile> Attack1() {
            List<Projectile> projlist = new List<Projectile>();

            FinishAttack = true;
            return projlist;
        }

        protected override List<Projectile> Attack2() {
            List<Projectile> projlist = new List<Projectile>();

            FinishAttack = true;
            return projlist;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox) {
            if (
                (drawcount < 120 && GOGUtils.CircularHitboxCollision(Projectile.Center + new Vector2(-6, -16), 70 + count * 1.5f, targetHitbox))
                || (drawcount >= 120 && GOGUtils.CircularHitboxCollision(Projectile.Center + new Vector2(-6, -16), 250, targetHitbox))
                ) {
                return true;
            }
            return false;
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "ChlorophyteDT").Value;
            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "ChlorophyteProj").Value;

            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, -24);
            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
    public class ChlorophyteProj : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Main.projFrames[Projectile.type] = 5;//动图帧数
            Projectile.tileCollide = false;
            Projectile.penetrate = 12;
            Projectile.frame += Main.rand.Next(5);
            Projectile.localNPCHitCooldown = 12;

            base.SetProperty();
        }
        Player Owner => Main.player[Projectile.owner];

        Vector2 totar = Vector2.Zero;
        public override void AI() {
            if (Projectile.alpha > 210) Projectile.Kill();
            if (count >= 60) Projectile.alpha += 1;
            if(count == 1) Projectile.rotation += Main.rand.NextFloat(MathHelper.Pi * 2);

            if (Projectile.ai[0] == 1) {
                if(drawcount == 1) Projectile.scale *= 0.5f;
                Projectile.localNPCHitCooldown = 60;
                Projectile.knockBack = 0.5f;

                NPC npc = Main.npc[(int)Projectile.ai[1]];
                if(npc != null && npc.active && totar == Vector2.Zero) {
                    totar = npc.Center.To(Projectile.Center);
                }
                if (npc != null && npc.active && totar != Vector2.Zero) {
                    Projectile.Center = npc.Center + totar;
                }

                if (npc == null && !npc.active) {
                    totar = Vector2.Zero;
                }
            }

            //帧图
            Projectile.frameCounter++;
            Projectile.frame = (Projectile.frameCounter / 6)
                % 5;//要手动填，不然会出错
            base.AI();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            if (Main.npc[(int)Projectile.ai[1]] == null || !Main.npc[(int)Projectile.ai[1]].active) Projectile.ai[1] = target.whoAmI;
            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            return base.OnTileCollide(oldVelocity);
        }

        public override void OnKill(int timeLeft) {
            //基本爆炸粒子
            for (int i = 0; i < 4; i++) {
                int num = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, DustID.ChlorophyteWeapon, 0f, 0f, 50, Color.White, 0.8f);
                Main.dust[num].velocity *= 1f;
                if (Main.rand.NextBool(2)) {
                    Main.dust[num].scale = 0.5f;
                    Main.dust[num].noGravity = true;
                    Main.dust[num].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + Name).Value;
            int singleFrameY = texture.Height / Main.projFrames[Type];
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0, -8);
            //Main.EntitySpriteDraw(texture, drawPos, new Rectangle(0, singleFrameY * Projectile.frame, texture.Width, singleFrameY), lightColor * ((255f - Projectile.alpha) / 255f), Projectile.rotation, new Vector2(27, 23), Projectile.scale, SpriteEffects.None, 0);
            Main.spriteBatch.Draw(
                texture,
                drawPos,
                new Rectangle(0, singleFrameY * Projectile.frame, texture.Width, singleFrameY),//动图读帧
                lightColor * ((255f - Projectile.alpha) / 255f),
                Projectile.rotation,
                new Vector2(14, 16),
                Projectile.scale,
                SpriteEffects.None,
                0);
            return base.PreDraw(ref lightColor);
        }
    }
}
