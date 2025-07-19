using GloryofGuardian.Common;
using System.Collections.Generic;
using System;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Audio;

namespace GloryofGuardian.Content.Projectiles {
    public class MeteorDT : GOGDT {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 52;
            Projectile.friendly = false;
            Projectile.penetrate = -1;

            OtherHeight = 80;

            count0 = 60;

            findboss = true;
            exdust = DustID.Flare;
        }

        Player Owner => Main.player[Projectile.owner];
        Vector2 AttackPos2 = Vector2.Zero;

        public override void AI() {
            AttackPos = Projectile.Center + new Vector2(-4, -80);
            breath = (float)Math.Sin(drawcount / 18f);
            AttackPos2 = AttackPos + new Vector2(0, -4) * breath;

            if (!Projectile.wet) {
                if (count % 1 == 0) {
                    for (int j = 0; j < 1; j++) {
                        int num1 = Dust.NewDust(AttackPos2 + new Vector2(-12, 4), 22, 16, DustID.Flare, 0f, 0f, 10, Color.White, 2f);
                        Main.dust[num1].noGravity = true;
                        Main.dust[num1].velocity = new Vector2(0, -1).RotatedByRandom(1f) * 1.5f;
                    }
                }

                if (count % 3 == 0) {
                    for (int j = 0; j < 1; j++) {
                        int num1 = Dust.NewDust(AttackPos2 + new Vector2(-2, 4), 9, 16, DustID.Flare, 0f, 0f, 10, Color.White, 3f);
                        Main.dust[num1].noGravity = true;
                        Main.dust[num1].velocity = new Vector2(0, -1) * 3;
                    }

                }

                if (count % 1 == 0) {
                    for (int j = 0; j < 1; j++) {
                        int num1 = Dust.NewDust(Projectile.Center + new Vector2(-24, -40), 36, 24, DustID.Flare, 0f, 0f, 10, Color.White, 2f);
                        Main.dust[num1].noGravity = true;
                        Main.dust[num1].velocity = new Vector2(0, -1).RotatedByRandom(1f) * Main.rand.NextFloat(1, 1.5f);
                    }
                }

                //灼烧
                foreach (NPC npc in Main.npc) {
                    if (npc != null && npc.active && Vector2.Distance(Projectile.Center, npc.Center) < 120) {
                        if (Owner.magmaStone) npc.AddBuff(BuffID.OnFire3, 60);
                        else npc.AddBuff(BuffID.OnFire, 60);
                    }
                }
            }

            base.AI();
        }

        protected override List<Projectile> Attack1() {
            List<Projectile> projlist = new List<Projectile>();

            if (Projectile.wet) return projlist;

            for (int i = 0; i < 1; i++) {
                Vector2 vel = AttackPos2.Toz(target0.Center) * 12f;
                
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), AttackPos, vel, ModContent.ProjectileType<MeteorProj>(), lastdamage, 8, Owner.whoAmI);
                
                projlist.Add(proj1);
            }

            FinishAttack = true;
            return projlist;
        }

        protected override List<Projectile> Attack2() {
            List<Projectile> projlist = new List<Projectile>();

            if (Projectile.wet) return projlist;

            for (int i = 0; i < 1; i++) {
                Vector2 vel = AttackPos2.Toz(target0.Center) * 12f;
                
                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item20, Projectile.Center);
                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), AttackPos, vel, ModContent.ProjectileType<MeteorProj>(), lastdamage, 8, Owner.whoAmI, 1);

                projlist.Add(proj1);
            }

            FinishAttack = true;
            return projlist;
        }

        float breath = 0;
        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "MeteorDT").Value;
            Texture2D texture1 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "MeteorDT2").Value;

            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, -22);
            Vector2 drawPosition1 = Projectile.Center - Main.screenPosition + new Vector2(12, -64) + new Vector2(0, -4) * breath;

            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);
            Main.EntitySpriteDraw(texture1, drawPosition1, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }

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
