using System;
using GloryofGuardian.Common;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles {
    public class BloodyProj2 : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Main.projFrames[Projectile.type] = 6;//动图帧数

            Projectile.width = 36;
            Projectile.height = 36;
            Projectile.localNPCHitCooldown = 0;
            Projectile.penetrate = 4;

            Projectile.scale = 0.6f;

            // 旋转90度
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(180f);
            framevel = Main.rand.Next(3, 7);
        }

        int framevel = 0;
        bool hungry = false;

        Player Owner => Main.player[Projectile.owner];

        public override void AI() {
            if (!hadbit) normal();
            else biting();

            //帧图
            Projectile.frameCounter++;
            Projectile.frame = (Projectile.frameCounter / framevel)
                % 6;//要手动填，不然会出错
        }

        void normal() {
            // 旋转90度
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(180f);
            if (count >= Main.rand.Next(5, 20)) hungry = true;

            if (hungry) {
                //追踪,路径释放粒子,咬两口,击中墙壁爆炸
                if (target0 != null && target0.active) {
                    Projectile.velocity *= 0.95f;
                    Projectile.velocity += Projectile.Center.Toz(target0.Center) * Main.rand.NextFloat(0.5f, 1.5f);
                    if (Vector2.Distance(Projectile.Center, target0.Center) < 64) {
                        Projectile.velocity *= 0.7f;
                        Projectile.velocity += Projectile.Center.Toz(target0.Center) * Main.rand.NextFloat(1f, 1.5f);
                    }
                }

                if (count % 1 == 0) {
                    for (int i = 0; i < 12; i++) {
                        int num = Dust.NewDust(Projectile.position, (int)(Projectile.width * Projectile.scale), (int)(Projectile.height * Projectile.scale), DustID.Crimson, 0f, 0f, 50, Color.Red, 0.8f);
                        Main.dust[num].velocity *= 0.5f;
                        Main.dust[num].velocity -= Projectile.velocity / 4;
                        Main.dust[num].noGravity = true;

                        if (Main.rand.NextBool(2)) {
                            Main.dust[num].scale = 0.5f;
                        }
                        if (Main.rand.NextBool(8)) {
                            Main.dust[num].noGravity = false;
                        }
                    }
                }
            }
        }

        void biting() {
            if (Main.npc[bittennpc] != null && Main.npc[bittennpc].active) {
                Projectile.Center = Main.npc[bittennpc].Center + bitepos;
                if (count % 60 == 0) {
                    //释放次级弹幕
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center - Projectile.rotation.ToRotationVector2() * 16, Vector2.Zero, ModContent.ProjectileType<BloodyProj>(), Projectile.damage / 2, 6, Owner.whoAmI, Projectile.ai[0], 2);
                    proj1.velocity = -Projectile.rotation.ToRotationVector2() * 8;

                    for (int i = 0; i < 24; i++) {
                        int num = Dust.NewDust(Projectile.position, (int)(Projectile.width * Projectile.scale), (int)(Projectile.height * Projectile.scale), DustID.Crimson, 0f, 0f, 50, Color.Red, 1.5f);
                        Main.dust[num].velocity = -Projectile.rotation.ToRotationVector2() * 8;
                        Main.dust[num].noGravity = true;

                        if (Main.rand.NextBool(2)) {
                            Main.dust[num].scale *= 1f;
                            Main.dust[num].velocity *= 2;
                        }
                        if (Main.rand.NextBool(2)) {
                            Main.dust[num].scale *= 0.8f;
                            Main.dust[num].velocity *= 1.5f;
                        }
                    }

                    if (Projectile.penetrate > 1) Projectile.penetrate -= 1;
                    if (Projectile.penetrate == 1) Projectile.Kill();
                }
            }
            else {
                hadbit = false;
                Projectile.velocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 8f;
            }
        }

        bool hadbit = false;
        int bittennpc = 0;
        Vector2 bitepos = Vector2.Zero;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            hadbit = true;
            Projectile.friendly = false;
            bittennpc = target.whoAmI;
            bitepos = target.Center.To(Projectile.Center);

            if (Owner.statLifeMax2 != Owner.statLife) {
                int addlife = Math.Min(Owner.statLifeMax2 - Owner.statLife, 3);
                Owner.statLife += addlife;
                CombatText.NewText(Owner.Hitbox,
                                Color.LightGreen,
                                addlife,
                                false,
                                false
                                );
            }

            //如果满血就整点儿活
            if (Owner.statLifeMax2 == Owner.statLife) {
                CombatText.NewText(Owner.Hitbox,
                                Color.LightGreen,
                                GOGUtils.Full(),
                                false,
                                false
                                );
            }

            base.OnHitNPC(target, hit, damageDone);
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            return base.OnTileCollide(oldVelocity);
        }

        public override void OnKill(int timeLeft) {
            base.OnKill(timeLeft);
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "BloodyProj2").Value;
            int singleFrameY = texture.Height / Main.projFrames[Type];
            Vector2 drawPos = Projectile.Center - Main.screenPosition + new Vector2(0, 0);

            Main.spriteBatch.Draw(
                texture,
                drawPos,
                new Rectangle(0, singleFrameY * Projectile.frame, texture.Width, singleFrameY),//动图读帧
                lightColor,
                Projectile.rotation,
                new Vector2(34, 18) / Projectile.scale,
                Projectile.scale * 1.2f,
                SpriteEffects.None,
                0);
            return true;
        }
    }
}
