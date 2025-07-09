using GloryofGuardian.Common;
using System.Collections.Generic;
using System;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;

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
}
