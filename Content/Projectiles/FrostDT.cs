using System;
using System.Collections.Generic;
using GloryofGuardian.Common;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles {
    public class FrostDT : GOGDT {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 40;
            Projectile.friendly = false;
            Projectile.penetrate = -1;

            OtherHeight = 46;

            count0 = 45;

            findboss = true;
            exdust = DustID.SnowSpray;
        }

        Player Owner => Main.player[Projectile.owner];
        Vector2 AttackPos2 = Vector2.Zero;

        public override void AI() {
            AttackPos = Projectile.Center + new Vector2(-3, -64);
            //if(NPC.downedDeerclops)

            float breath = (float)Math.Sin(drawcount / 32f);
            AttackPos2 = AttackPos + new Vector2(0, -8) * breath;

            if (count % 1 == 0) {
                for (int j = 0; j < 3; j++) {
                    int num1 = Dust.NewDust(AttackPos2 + new Vector2(0, -8 * Main.rand.NextFloat(0, 1)).RotatedBy(Main.rand.NextFloat(0, MathHelper.Pi * 2)), 0, 0, DustID.SnowflakeIce, 0f, 0f, 10, Color.White, 0.8f);
                    Main.dust[num1].velocity = Main.dust[num1].position.Toz(AttackPos2).RotatedBy(MathHelper.PiOver2) * 1f;
                    Main.dust[num1].velocity *= 1f;
                    Main.dust[num1].noGravity = true;
                    int num2 = Dust.NewDust(AttackPos2 + new Vector2(0, -8 * Main.rand.NextFloat(0, 1)).RotatedBy(Main.rand.NextFloat(0, MathHelper.Pi * 2)), 0, 0, DustID.SnowSpray, 0f, 0f, 10, Color.White, 0.8f);
                    Main.dust[num2].velocity = Main.dust[num2].position.Toz(AttackPos2).RotatedBy(MathHelper.PiOver2) * 1f;
                    Main.dust[num2].velocity *= 1f;
                    Main.dust[num2].noGravity = true;
                }
            }

            if (Owner.ZoneSnow && count % 1 == 0) {
                for (int j = 0; j < 1; j++) {
                    int num1 = Dust.NewDust(Projectile.Center + new Vector2(-24, -72), 42, 24, DustID.SnowSpray, 0f, 0f, 10, Color.White, 0.4f);
                    Main.dust[num1].noGravity = false;
                    Main.dust[num1].velocity = new Vector2(0, 1f) * Main.rand.NextFloat(1, 1.5f);
                }
            }

            base.AI();
        }

        protected override List<Projectile> Attack1() {
            List<Projectile> projlist = new List<Projectile>();

            for (int i = 0; i < 1; i++) {
                Vector2 vel = AttackPos2.Toz(target0.Center) * 8f;
                if (Owner.ZoneSnow) vel *= 1.5f;

                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);
                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), AttackPos2, vel, ModContent.ProjectileType<FrostProj>(), lastdamage, 8, Owner.whoAmI);
            
                projlist.Add(proj1);
            }

            FinishAttack = true;
            return projlist;
        }

        protected override List<Projectile> Attack2() {
            List<Projectile> projlist = new List<Projectile>();

            for (int i = 0; i < 1; i++) {
                Vector2 vel = AttackPos2.Toz(target0.Center) * 8f;
                if (Owner.ZoneSnow) vel *= 1.5f;

                Terraria.Audio.SoundEngine.PlaySound(SoundID.Item8, Projectile.Center);
                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), AttackPos2, vel, ModContent.ProjectileType<FrostProj>(), lastdamage, 8, Owner.whoAmI, 1);
            
                projlist.Add(proj1);
            }

            FinishAttack = true;
            return projlist;
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "FrostDT").Value;
            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, -23);
            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
