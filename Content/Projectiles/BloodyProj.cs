using System;
using GloryofGuardian.Common;
using GloryofGuardian.Content.ParentClasses;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles {
    public class BloodyProj : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.localNPCHitCooldown = 20;
            Projectile.penetrate = 1;

            if (Projectile.ai[1] == 0) Projectile.extraUpdates = 1;

            Projectile.scale *= Main.rand.NextFloat(1, 1.5f);
            if (Projectile.ai[1] == 2) Projectile.tileCollide = false;
        }

        Player Owner => Main.player[Projectile.owner];

        public override void AI() {
            //遵循传入的重力的影响
            float G = Projectile.ai[0];
            Projectile.velocity += new Vector2(0, G);

            if (Projectile.ai[1] == 1 && count < 20) Projectile.friendly = false;
            if (Projectile.ai[1] == 1 && count >= 20) Projectile.friendly = true;


            if (count % 1 == 0) {
                for (int i = 0; i < 4; i++) {
                    int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Crimson, 0f, 0f, 50, Color.Red, 1f);
                    Main.dust[num].velocity *= 0f;
                    Main.dust[num].noGravity = true;
                }
            }

            if (count % 1 == 0) {
                for (int i = 0; i < 4; i++) {
                    int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Crimson, 0f, 0f, 50, Color.Red, 0.8f);
                    Main.dust[num].velocity *= 0.5f;
                    Main.dust[num].noGravity = true;
                }
            }

            if (count % 1 == 0 && Projectile.ai[1] > 1) {
                for (int i = 0; i < 4; i++) {
                    int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Crimson, 0f, 0f, 50, Color.Black, 1f);
                    Main.dust[num].velocity *= 0f;
                    Main.dust[num].noGravity = true;
                }
            }

            if (Projectile.ai[1] == 2 && count > 15) Projectile.Kill();

            base.AI();
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            int dustnum = 0;
            if (Projectile.ai[1] == 0) dustnum = 25;
            if (Projectile.ai[1] == 1) dustnum = 10;
            if (Projectile.ai[1] == 2) dustnum = 5;

            if (Projectile.ai[1] == 2) target.velocity /= 2;

            for (int i = 0; i < dustnum; i++) {
                int num = Dust.NewDust(Projectile.Center, 0, 0, DustID.Crimson, 0f, 0f, 50, Color.Red, 1f);
                int num1 = Dust.NewDust(Projectile.Center, 0, 0, DustID.Crimson, 0f, 0f, 50, Color.Red, 0.6f);
                int num2 = Dust.NewDust(Projectile.Center, 0, 0, DustID.Crimson, 0f, 0f, 50, Color.Black, 1f);
                Main.dust[num].velocity *= 2f;
                Main.dust[num].noGravity = true;
                Main.dust[num1].velocity *= 1f;
                Main.dust[num1].noGravity = false;
                Main.dust[num2].velocity *= 1.2f;
                Main.dust[num2].noGravity = true;
            }

            if (Projectile.ai[1] == 0) {
                for (int i = 0; i < Main.rand.Next(2) + 1; i++) {
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center, new Vector2(0, -4).RotatedByRandom(MathHelper.Pi * 2), ModContent.ProjectileType<BloodyProj>(), Projectile.damage / 2, 6, Owner.whoAmI, Projectile.ai[0], 1);
                }
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            if (Projectile.ai[1] == 0) {
                Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);

                //碰撞竖直墙壁
                if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon) {
                    for (int i = 0; i < 25; i++) {
                        int num = Dust.NewDust(Projectile.Center, 0, 0, DustID.Crimson, 0f, 0f, 50, Color.Red, 1.5f);
                        int num1 = Dust.NewDust(Projectile.Center, 0, 0, DustID.Crimson, 0f, 0f, 50, Color.Red, 0.6f);
                        int num2 = Dust.NewDust(Projectile.Center, 0, 0, DustID.Crimson, 0f, 0f, 50, Color.Black, 1f);

                        if (Main.rand.NextBool(2)) Main.dust[num].velocity += Main.dust[num1].velocity = Main.dust[num2].velocity
                            = new Vector2(-oldVelocity.X, 0).RotatedBy(MathHelper.PiOver4) * 1.5f;
                        else Main.dust[num].velocity += Main.dust[num1].velocity = Main.dust[num2].velocity
                            = new Vector2(-oldVelocity.X, 0).RotatedBy(-MathHelper.PiOver4) * 1.5f;

                        Main.dust[num].velocity *= 2f;
                        Main.dust[num].noGravity = true;
                        Main.dust[num1].velocity *= 1f;
                        Main.dust[num1].noGravity = false;
                        Main.dust[num2].velocity *= 1.2f;
                        Main.dust[num2].noGravity = true;
                    }
                }

                //碰撞水平地面
                if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon) {
                    for (int i = 0; i < 25; i++) {
                        int num = Dust.NewDust(Projectile.Center, 0, 0, DustID.Crimson, 0f, 0f, 50, Color.Red, 1.5f);
                        int num1 = Dust.NewDust(Projectile.Center, 0, 0, DustID.Crimson, 0f, 0f, 50, Color.Red, 0.6f);
                        int num2 = Dust.NewDust(Projectile.Center, 0, 0, DustID.Crimson, 0f, 0f, 50, Color.Black, 1f);

                        if (Main.rand.NextBool(2)) Main.dust[num].velocity += Main.dust[num1].velocity = Main.dust[num2].velocity
                            = new Vector2(0, -oldVelocity.Y).RotatedBy(MathHelper.PiOver4) / 4;
                        else Main.dust[num].velocity += Main.dust[num1].velocity = Main.dust[num2].velocity
                            = new Vector2(0, -oldVelocity.Y).RotatedBy(-MathHelper.PiOver4) / 4;

                        Main.dust[num].velocity *= 2f;
                        Main.dust[num].noGravity = true;
                        Main.dust[num1].velocity *= 1f;
                        Main.dust[num1].noGravity = false;
                        Main.dust[num2].velocity *= 1.2f;
                        Main.dust[num2].noGravity = true;
                    }
                    //Projectile.velocity.Y = -oldVelocity.Y * 0.5f;
                }

                for (int i = 0; i < Main.rand.Next(2) + 1; i++) {
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), Projectile.Center, Vector2.Zero, ModContent.ProjectileType<BloodyProj>(), Projectile.damage / 2, 6, Owner.whoAmI, Projectile.ai[0], 1);

                    if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon) proj1.velocity = new Vector2(-oldVelocity.X / 2f, Main.rand.NextFloat(-2, 2));
                    if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon) proj1.velocity = new Vector2(Main.rand.NextFloat(-2, 2), -oldVelocity.Y / 2f);
                }

                return true;
            }
            else return true;
        }

        public override void OnKill(int timeLeft) {
            //爆炸
            if (Projectile.ai[1] == 1) {
                for (int j = 0; j < 5; j++) {
                    int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Crimson, 0f, 0f, 10, Color.Black, 1.7f);
                    Main.dust[num2].noGravity = true;
                    Main.dust[num2].velocity *= 1f;
                    num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Crimson, 0f, 0f, 10);
                    Main.dust[num2].velocity *= 1f;
                }
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

        public override bool PreDraw(ref Color lightColor) {
            return false;
        }
    }
}
