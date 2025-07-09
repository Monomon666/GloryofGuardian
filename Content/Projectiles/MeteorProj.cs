using System;
using GloryofGuardian.Common;
using GloryofGuardian.Content.ParentClasses;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles {
    public class MeteorProj : GOGProj {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.localNPCHitCooldown = 20;
            Projectile.penetrate = 1;

            Projectile.scale *= 1.5f;
            Projectile.extraUpdates = 1;

            //wet判定的前提
            Projectile.ignoreWater = false;

            if (Projectile.ai[0] == 0 || Projectile.ai[0] == 2) Projectile.penetrate = 3;
        }

        Player Owner => Main.player[Projectile.owner];

        Vector2 statpos = Vector2.Zero;
        public override void AI() {
            if (count == 1) statpos = Projectile.Center;
            //火系弹幕遇水消失
            if (Projectile.wet) Projectile.Kill();

            if (Projectile.ai[0] == 1) Projectile.penetrate = 1;

            if (Projectile.ai[0] == 0 || Projectile.ai[0] == 2) {
                if (count % 1 == 0) {
                    for (int i = 0; i < 4; i++) {
                        int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Flare, 0f, 0f, 10, Color.White, 2f);
                        Main.dust[num].velocity *= 0.5f;
                        Main.dust[num].noGravity = true;
                    }
                }
            }
            if (Projectile.ai[0] == 1) {
                if (count % 1 == 0) {
                    for (int i = 0; i < 16; i++) {
                        int num = Dust.NewDust(Projectile.Center + Projectile.velocity.RotatedBy(count / 4f + MathHelper.Pi * (i % 2)) * 0.8f, 0, 0, DustID.Flare, 0f, 0f, 10, Color.White, 2f);
                        Main.dust[num].velocity *= 0.5f;
                        Main.dust[num].velocity += Projectile.velocity / 1f * Math.Min(0.5f, count / 20f);
                        Main.dust[num].noGravity = true;
                    }

                    if (Owner.magmaStone) {
                        for (int i = 0; i < 2; i++) {
                            int num = Dust.NewDust(Projectile.Center + Projectile.velocity.RotatedBy(count / 4f + MathHelper.Pi * (i % 2)) * 0.8f, 0, 0, DustID.SolarFlare, 0f, 0f, 10, Color.White, 2f);
                            Main.dust[num].velocity *= 0.5f;
                            Main.dust[num].velocity += statpos.Toz(Main.dust[num].position) * Math.Min(4f, count / 20f);
                            Main.dust[num].velocity += Projectile.velocity / 1f * Math.Min(0.5f, count / 20f);
                            Main.dust[num].noGravity = true;
                        }
                    }
                }
            }

            if (Projectile.ai[0] == 2) {
                Projectile.extraUpdates = 0;
                Projectile.localNPCHitCooldown = 30;
                if (count < 30) Projectile.friendly = false;
                if (count >= 30) Projectile.friendly = true;

                if (Projectile.velocity.Y < 8) Projectile.velocity += new Vector2(0, 0.17f);
            }

            base.AI();
        }

        bool hadhit = false;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone) {
            if (Projectile.ai[0] != 2 && target.HasBuff(BuffID.OnFire) && !hadhit) {
                for (int i = 0; i < 3; i++) {
                    Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), target.Center,
                        new Vector2(0, -6).RotatedBy(Main.rand.NextFloat(0, MathHelper.Pi * 2) + (MathHelper.Pi * i * 2 / 3f)), ModContent.ProjectileType<MeteorProj>(), Projectile.damage / 2, 8, Owner.whoAmI, 2);
                }

                hadhit = true;
            }

            if (Projectile.ai[0] == 1) {
                if (Owner.magmaStone) target.AddBuff(BuffID.OnFire3, 300);
                else target.AddBuff(BuffID.OnFire, 300);
            }

            for (int i = 0; i < 24; i++) {
                int num = Dust.NewDust(target.Center + new Vector2(-Projectile.width / 2f, -Projectile.height / 2f), Projectile.width, Projectile.height, DustID.Flare, 0f, 0f, 10, Color.White, 3f);
                Main.dust[num].velocity = new Vector2(0, -3).RotatedBy(Main.rand.NextFloat(-0.7f, 0.7f));
                Main.dust[num].velocity *= Main.rand.NextFloat(0, 3f);
                Main.dust[num].noGravity = true;
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity) {
            if (Projectile.ai[0] == 2) {
                //反弹
                Projectile.penetrate -= 1;//反弹次数
                if (Projectile.penetrate <= 0) {
                    Projectile.Kill();
                }
                else {
                    Collision.HitTiles(Projectile.position, Projectile.velocity, Projectile.width, Projectile.height);

                    SoundEngine.PlaySound(SoundID.Dig, Projectile.position);

                    if (Math.Abs(Projectile.velocity.X - oldVelocity.X) > float.Epsilon) {
                        Projectile.velocity.X = -oldVelocity.X;
                    }

                    if (Math.Abs(Projectile.velocity.Y - oldVelocity.Y) > float.Epsilon) {
                        Projectile.velocity.Y = -oldVelocity.Y * 0.5f;
                    }
                }

                return false;
            }
            else return base.OnTileCollide(oldVelocity);
        }

        public override void OnKill(int timeLeft) {
            SoundEngine.PlaySound(SoundID.Dig, Projectile.Center);
            if (Projectile.ai[0] == 1) {
                //爆炸
                for (int j = 0; j < 52; j++) {
                    int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.InfernoFork, 0, 0, 0, Color.White, 2f);
                    Main.dust[num1].noGravity = true;
                    Main.dust[num1].velocity = new Vector2((float)Math.Sin(j * 12 / 100f), (float)Math.Cos(j * 12 / 100f)) * Main.rand.NextFloat(4f, 8f);

                    int num2 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.InfernoFork, 0, 0, 0, Color.White, 1f);
                    Main.dust[num2].noGravity = true;
                    Main.dust[num2].velocity = new Vector2((float)Math.Sin(j * 12 / 100f), (float)Math.Cos(j * 12 / 100f)) * Main.rand.NextFloat(8f, 12f);
                }
                Projectile.position = Projectile.Center;
                Projectile.width = Projectile.height = 160;
                Projectile.position.X = Projectile.position.X - Projectile.width / 2;
                Projectile.position.Y = Projectile.position.Y - Projectile.height / 2;
                Projectile.maxPenetrate = -1;
                Projectile.penetrate = -1;
                Projectile.usesLocalNPCImmunity = false;
                Projectile.usesIDStaticNPCImmunity = true;
                Projectile.idStaticNPCHitCooldown = 0;
                Projectile.Damage();
            }

            //基本爆炸粒子
            for (int i = 0; i < 12; i++) {
                int num = Dust.NewDust(new Vector2(base.Projectile.position.X, base.Projectile.position.Y), base.Projectile.width, base.Projectile.height, DustID.Flare, 0f, 0f, 50, Color.White, 2f);
                Main.dust[num].velocity *= 2f;
                if (Main.rand.NextBool(2)) {
                    Main.dust[num].scale = 1f;
                    Main.dust[num].noGravity = true;
                    Main.dust[num].fadeIn = 1f + Main.rand.Next(10) * 0.1f;
                }
            }
        }

        public override bool PreDraw(ref Color lightColor) {
            return false;
        }
    }
}
