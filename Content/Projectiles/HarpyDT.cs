using GloryofGuardian.Common;
using System.Collections.Generic;
using System;
using GloryofGuardian.Content.ParentClasses;
using Microsoft.Xna.Framework.Graphics;
using Terraria.DataStructures;
using Terraria.ID;

namespace GloryofGuardian.Content.Projectiles {
    public class HarpyDT : GOGDT {
        public override string Texture => GOGConstant.nulls;

        public override void SetProperty() {
            Projectile.width = 64;
            Projectile.friendly = false;
            Projectile.penetrate = -1;

            OtherHeight = 34;

            count0 = 9;

            exdust = DustID.Cloud;
            Drop = false;
        }

        Player Owner => Main.player[Projectile.owner];

        public override void AI() {
            AttackPos = Projectile.Center + new Vector2(-2, 0);
            Projectile.Center += new Vector2(0, (float)Math.Sin(drawcount / 30f) * 0.5f);

            if (!Main.raining) {
                //Todo
            }

            //todo 粒子动画
            base.AI();
        }

        protected override List<Projectile> Attack1() {
            List<Projectile> projlist = new List<Projectile>();

            for (int i = 0; i < 1; i++) {
                //Terraria.Audio.SoundEngine.PlaySound(SoundID.NPCDeath13, Projectile.Center);
                Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile),
                    AttackPos + new Vector2(Main.rand.NextFloat(-8, 8), 0), new Vector2(0, 12).RotatedBy(Main.rand.NextFloat(-0.2f, 0.2f)).RotatedBy(-Main.windSpeedCurrent / 4f), ModContent.ProjectileType<HarpyProj>(), lastdamage, 6, Owner.whoAmI);

                projlist.Add(proj1);
            }

            FinishAttack = true;
            return projlist;
        }

        protected override List<Projectile> Attack2() {
            List<Projectile> projlist = new List<Projectile>();

            for (int i = 0; i < 1; i++) {
               //Vector2 vel = new Vector2(0, -16);
               //
               //for (int j = 0; j < 25; j++) {
               //    int num = Dust.NewDust(AttackPos + new Vector2(0, 16), 0, 0, DustID.Crimson, 0f, 0f, 50, Color.Red, 1.5f);
               //    int num1 = Dust.NewDust(AttackPos + new Vector2(0, 16), 0, 0, DustID.Crimson, 0f, 0f, 50, Color.Red, 0.6f);
               //    int num2 = Dust.NewDust(AttackPos + new Vector2(0, 16), 0, 0, DustID.Crimson, 0f, 0f, 50, Color.Black, 1f);
               //
               //    if (Main.rand.NextBool(2)) Main.dust[num].velocity += Main.dust[num1].velocity = Main.dust[num2].velocity
               //        = new Vector2(0, -2).RotatedBy(MathHelper.PiOver4 * 0.8f) * 1.5f;
               //    else Main.dust[num].velocity += Main.dust[num1].velocity = Main.dust[num2].velocity
               //        = new Vector2(0, -2).RotatedBy(-MathHelper.PiOver4 * 0.8f) * 1.5f;
               //
               //    Main.dust[num].velocity *= 2f;
               //    Main.dust[num].noGravity = true;
               //    Main.dust[num1].velocity *= 1f;
               //    Main.dust[num1].noGravity = false;
               //    Main.dust[num2].velocity *= 1.2f;
               //    Main.dust[num2].noGravity = true;
               //}
               //
               //Terraria.Audio.SoundEngine.PlaySound(SoundID.NPCDeath13, Projectile.Center);
               //Projectile proj1 = Projectile.NewProjectileDirect(new EntitySource_Parent(Projectile), AttackPos + new Vector2(0, 16), vel, ModContent.ProjectileType<BloodyProj2>(), lastdamage, 8, Owner.whoAmI);
               //
               //projlist.Add(proj1);
            }

            FinishAttack = true;
            return projlist;
        }

        public override bool PreDraw(ref Color lightColor) {
            Texture2D texture0 = ModContent.Request<Texture2D>(GOGConstant.Projectiles + "HarpyDT").Value;

            Vector2 drawPosition0 = Projectile.Center - Main.screenPosition + new Vector2(0, -16);
            Main.EntitySpriteDraw(texture0, drawPosition0, null, lightColor, Projectile.rotation, texture0.Size() * 0.5f, Projectile.scale, SpriteEffects.None, 0);

            return false;
        }
    }
}
